using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.World;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls;
using ReLogic.Content;
using CalamityMod.Projectiles.Summon;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using Terraria.ID;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.NPCs;
using Terraria.ModLoader.IO;
using System.IO;
using CalamityMod.Particles;
using FargowiltasSouls.Core.Systems;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PWEternity : CalDLCEmodeBehavior
    {
        public static bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<PrimordialWyrmHead>();

        private const int PrimordialWyrmHeadTypePlaceholder = -1;

        private const int ReaverSharkTypePlaceholder = -1;

        private const int PWIllusionTypePlaceholder = -1;

        private const int DevouredSharkProjectilePlaceholder = -1;

        private enum PWAttack : byte
        {
            None = 0,
            Illusion = 1,
            SharkDevour = 2,
            SilvaVines = 3,
            RadianceShield = 4, 
            XerocRage = 5,
            Susanoo = 6,
            YamiYamiNoMi = 7,
            AbyssalKnights = 8,
            //
            NoxusDistortion = 10,
            Desperation = 11
        }

        private readonly Dictionary<PWAttack, Action<NPC, Player>> attackHandlers = new();

        private int attackIndex;
        private int[] attackCycle = new int[] { 0, 1, 0, 2};
        private int attackTimer;
        private bool inSpecialAttack;
        private PWAttack currentAttack = PWAttack.None;

        private int attackPhaseTimer;
        private int chaseGrace = 0;

        private bool IsMasochist() => WorldSavingSystem.MasochistModeReal;

        private const byte NET_SYNC_ATTACK = 1;

        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 2f);
            NPC.damage = (int)Math.Round(NPC.damage * 1.8f);
        }

        public override void Load()
        {
            attackHandlers[PWAttack.Illusion] = RunIllusionPhase;
            attackHandlers[PWAttack.SharkDevour] = RunSharkDevourPhase;
            attackHandlers[PWAttack.SilvaVines] = RunSilvaVinesPhase;
            attackHandlers[PWAttack.RadianceShield] = RunRadianceShieldPhase;
            attackHandlers[PWAttack.XerocRage] = RunXerocRagePhase;
            attackHandlers[PWAttack.Susanoo] = RunSusanooPhase;
            attackHandlers[PWAttack.YamiYamiNoMi] = RunYamiYamiNoMiPhase;
            attackHandlers[PWAttack.AbyssalKnights] = RunAbyssalKnightsPhase;
            //
            attackHandlers[PWAttack.NoxusDistortion] = RunNoxusDistortionPhase;
            attackHandlers[PWAttack.Desperation] = StartDesperationPhase;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)currentAttack);
            writer.Write(attackIndex);
            writer.Write(attackTimer);
            writer.Write(inSpecialAttack);
            writer.Write(attackPhaseTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentAttack = (PWAttack)reader.ReadByte();
            attackIndex = reader.ReadInt32();
            attackTimer = reader.ReadInt32();
            inSpecialAttack = reader.ReadBoolean();
            attackPhaseTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            if (!target.active || target.dead)
            {
                NPC.TargetClosest(false);
                NPC.velocity.Y += 0.2f;
                if (NPC.timeLeft > 10)
                    NPC.timeLeft > 10;
                return;    
            }

            if (chaseGrace > 0) chaseGrace--;

            if (!inSpecialAttack && chaseGrace == 0)
            {
                attackTimer++;
                int triggerTime = IsMasochist() ? 360 : 540;

                if (attackTimer >= triggerTime + Main.rand.Next(-120, 121))
                {
                    attackTimer = 0;

                    attackIndex = (attackIndex + 1) % attackCycle.Length;
                    int pick = attackCycle[attackIndex];

                    if (pick == 1)
                        StartIllusionPhase();
                    else if (pick == 2)
                        StartSharkDevourPhase();    
                }
            }

            if (inSpecialAttack && attackHandlers.TryGetValue(currentAttack, out var handler))
                handler.Invoke(npc, target);
            else
            {
                ChasePlayer(npc, target);
            }

            if (Main.rand.NextBool(12))
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, 0f);
                d.scale = 0.6f;
                d.noGravity = true;
            }
        }

        private void ChasePlayer(Player target)
        {
            NPC.TargetClosest(true);

            Vector2 toTarget = target.Center - NPC.Center;
            float distance = toTarget.Length();
            Vector2 dir = toTarget.SafeNormalize(Vector2.Zero);

            float baseSpeed = 1.35f;
            if (IsMasochist())
                baseSpeed *= 1.6f;

            if (distance < 80f)
                baseSpeed *= 0.6f;

            Vector2 desiredVelocity = dir * baseSpeed;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 0.06f);    
        }

        private void StartIllusionPhase()
        {
            inSpecialAttack = true;
            currentAttack = PWAttack.Illusion;
            attackPhaseTimer = 0;

            NPC.velocity *= 0.1f;

            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.NPCHit4, NPC.Center);
            }
            NPC.netUpdate = true;
        }

        private void RunIllusionPhase(Player target)
        {
            attackPhaseTimer++;

            if (attackPhaseTimer <= 30)
            {
                NPC.alpha = Math.Min(255, NPC.alpha + 10);
                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (Main.rand.NextBool(3))
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 150, default, 0.6f);
            }

            else if (attackPhaseTimer <= 150)
            {
                if (attackPhaseTimer == 60)
                {
                    if (IsMasochist() && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int extra = 2;
                        for (int j = 0; j < extra; j++)
                        {
                            int idx = NPC.NewNPC(NPC.GetSource_FromAI(),
                            (int)(NPC.Center.X + Main.rand.NextFloat(-600, 600)),
                            (int)(NPC.Center.Y + Main.rand.NextFloat(-400, 400)),
                            PWIllusionTypePlaceholder);
                            if (Main.npc.IndexInRange(idx))
                                Main.npc[idx].damage = (int)(NPC.damage * 0.4f);
                        }
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int clones = 2 + Main.rand.Next(2);
                        for (int i = 0; i < clones; i++)
                        {
                            int npcIndex = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + Main.rand.NextFloat(-400, 400)), (int)(NPC.Center.Y + Main.rand.NextFloat(-200, 200)),PWIllusionTypePlaceholder);
                            if (Main.npc.IndexInRange(npcIndx))
                            {
                                Main.npc[npcIndex].velocity = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-1f, 1f));
                                Main.npc[npcIndex].ai[0] = NPC.whoAmI;
                            }
                        }
                    }
                }

                Vector2 drift = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 6f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, drift, 0.02f);
            }

            else if (attackPhaseTimer <= 210)
            {
                if (NPC.alpha > 0)
                    NPC.alpha = Math.Max(0, NPC.alpha - 20);

                if (attackPhaseTimer == 180)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }    

                if (attackPhaseTimer >= 180)
                {
                    Vector2 chargeDir = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    float chargeSpeed = 32f;
                    NPC.velocity = chargeDir * chargeSpeed;

                    if (IsMasochist() && attackPhaseTimer % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 projVel = chargeDir.RotatedByRandom(MathHelper.ToRadians(25)) * 10f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel,
                            ProjectileID.CursedFlameFriendly, NPC.damage / 6, 2f, Main.myPlayer);
                    }
                }
            }
            else
            {
                EndSpecialAttack();
            }
        }

        private void StartSharkDevourPhase()
        {
            inSpecialAttack = true;
            currentAttack = PWAttack.SharkDevour;
            attackPhaseTimer = 0;
            NPC. velocity *= 0.1f;
            NPC.netUpdate = true;
            if (Main.netMode != NetmodeID.Server)
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
        }

        private void RunSharkDevourPhase(Player target)
        {
            attackPhaseTimer++;

            if (attackPhaseTimer == 1)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int summonCount = 4 + Main.rand.Next(2);
                    for (int i = 0; i < summonCount; i++)
                    {
                        Vector2 spawnPos = NPC.Center + new Vector2(Main.rand.NextFloat(-500, 500), Main.rand.NextFloat(-300, 300));
                        int idx = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPos.Y, ReaverSharkTypePlaceholder);
                        if (Main.npc.IndexInRange(idx))
                        {
                            Main.npc[idx].velocity = (target.Center - Main.npc[idx].Center.SafeNormalize(Vector2.Zero) * (6f + Main.rand.NextFloat(0f, 3f)));
                        }
                    }
                }
            }

            else if (attackPhaseTimer < 180)
            {
                Vector2 approach = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 6f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, approach, 0.04f);
            }

            else if (attackPhaseTimer == 180)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC other = Main.npc[i];
                        if (!other.active) continue;
                        if (other.type != ReaverSharkTypePlaceholder) continue;


                        Vector2 dir = (target.Center - other.Center).SafeNormalize(Vector2.Zero);
                        float speed = 12f + Main.rand.NextFloat(-2f, 2f);
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), other.Center, dir * speed, DevouredSharkProjectilePlaceholder, NPC.damage / 4, 3f, Main.myPlayer);

                        other.active = false;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, other.whoAmI);
                    }
                }

                SoundEngine.PlaySound(SoundID.Item122, NPC.Center);

                if (IsMasochist() && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(300, 300);
                        Vector2 vel = (target.Center - (target.Center + offset)).SafeNormalize(Vector2.Zero) * 10f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + offset, vel,
                            ProjectileID.CursedFlameFriendly, NPC.damage / 5, 2f, Main.myPlayer);
                    }
                }
            }

            else if (attackPhaseTimer < 240)
            {
                NPC.velocity *= 0.96f;
            }
            else
            {
                EndSpecialAttack();
            }
        }

        private void RunSilvaVinesPhase(NPC npc, Player target)
        {
            attackPhaseTimer++;

            if (attackPhaseTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Grass, npc.Center);

                for (int i = 0; i < 12; i++)
                {
                    float rot = MathHelper.ToRadians(30 * i);
                    Vector2 vel = rot.ToRotationVector2() * 3f;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center,
                        vel, ModContent.ProjectileType<SilvaVineProj>(), 0, 0, Main.myPlayer, npc.whoAmI);
                }
            }

            npc.velocity *= 0.9f;

            if (attackPhaseTimer > 300)
                EndSpecialAttack(npc);
        }

        private void RunRadianceShieldPhase(NPC npc, Player target)
        {
            attackPhaseTimer++;

            if (attackPhaseTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, npc.Center);
                npc.dontTakeDamage = true;
                npc.alpha = 0;
            }

            float radius = 600f;
            float rotSpeed = 0.025f;
            Vector2 offset = new Vector2(radius, 0).RotatedBy(attackPhaseTimer * rotSpeed);
            Vector2 desiredPos = target.Center + offset;
            npc.Center = Vector2.Lerp(npc.Center, desiredPos, 0.1f);

            if (attackPhaseTimer % 12 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 dir = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, dir * 14f,
                    ProjectileID.CultistBossFireball, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 2f, Main.myPlayer);
            }

            if (attackPhaseTimer > 360)
            {
                npc.dontTakeDamage = false;
                npc.alpha = 0;
                EndSpecialAttack(npc);
            }
        }

        private void RunXerocRagePhase(NPC npc, Player target)
        {
            attackPhaseTimer++;

            int duration = WorldSavingSystem.MasochistModeReal ? 1200 : 600;

            if (attackPhaseTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                npc.color = Color.MediumPurple;
                npc.localAI[0] = 1;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int xeroc = Projectile.NewProjectile(npc.GetSource_FromAI(),
                    npc.Center + new Vector2(0, -800f),
                    Vector2.Zero,
                    ModContent.ProjectileType<XerocEyeProjectile>(),
                    0, 0f, Main.myPlayer);
                    npc.ai[3] = xeroc;
                }
            }
            
            if (WorldSavingSystem.MasochistModeReal)
            {
                if (attackPhaseTimer % 6 == 0)
                {
                    Projectile xerocProj = Main.projectile[(int)npc.ai[3]];
                    if (xerocProj.active)
                    {
                        Vector2 dirToEye = (xerocProj.Center - npc.Center).SafeNormalize(Vector2.UnitY);
                        Vector2 spread = dirToEye.RotatedByRandom(MathHelper.ToRadians(45));
                        float speed = 22f + Main.rand.NextFloat(-5f, 6f);

                        int[] masochistProjectiles = new int[]
                        {
                          ProjectileID.PhantasmalBolt,
                          ProjectileID.CultistBossFireball,
                          ProjectileID.PhantasmalEye,
                          ProjectileID.CultistBossLightningOrbArc
                        };
                        int projType = Main.rand.Next(masochistProjectiles);   

                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, spread * speed,
                            projType, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer);
                }
                } 

                if (attackPhaseTimer % 180 == 0)
                {
                    Vector2 beamDir = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, beamDir * 25f,
                        ProjectileID.DeathLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage) * 2, 6f, Main.myPlayer);
                }
            }

            if (attackPhaseTimer % 12 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile xerocProj = Main.projectile[(int)npc.ai[3]];
                if (xerocProj.active)
                {
                    Vector2 dirToEye = (xerocProj.Center - npc.Center).SafeNormalize(Vector2.UnitY);
                    Vector2 spread = dirToEye.RotatedByRandom(MathHelper.ToRadians(35));
                    float speed = 20f + Main.rand.NextFloat(-4f, 4f);

                    int projType = Main.rand.NextBool()
                        ? ProjectileID.PhantasmalBolt
                        : ProjectileID.CultistBossFireball;

                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, spread * speed,
                        projType, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer);
                }
            }

            if (attackPhaseTimer % 90 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 burstDir = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 6; i++)
                {
                    Vector2 spread = burstDir.RotatedByRandom(MathHelper.ToRadians(15));
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, spread * 18f,
                        ProjectileID.CultistBossLightningOrbArc,
                        FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 3f, Main.myPlayer);
                }
            }

            npc.velocity = npc.velocity.RotatedByRandom(MathHelper.ToRadians(6)) * 1.05f;

            if (attackPhaseTimer > duration)
            {
                npc.localAI[0] = 0;
                npc.color = Color.White;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int xerocIdx = (int)npc.ai[3];
                    if (xerocIdx >= 0 && xerocIdx < Main.maxProjectiles)
                        Main.projectile[xerocIdx].Kill();
                }
                EndSpecialAttack(npc);
            }  
        }

        private void RunSusanooPhase(NPC npc, Player target)
        {
            attackPhaseTimer++;
            int duration = WorldSavingSystem.MasochistModeReal ? 960 : 720;

            if (attackPhaseTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, npc.Center);
                Main.NewText("The Wrath of the forgotten Dragon fills the Abyss!", Color.Cyan);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 pos = npc.Center + Main.rand.NextVector2Circular(800, 600);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), pos, Vector2.Zero,
                        ProjectileID.CultistBossLightningOrbArc,
                        FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage) / 2,
                        2f, Main.myPlayer);
                }
            }

            if (attackPhaseTimer % 90 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 12; i++)
                {
                    float angle = MathHelper.ToRadians(30 * i);
                    Vector2 velocity = angle.ToRotationVector2() * 5f;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center,
                        velocity, ProjectileID.Typhoon, npc.damage / 6, 1f, Main.myPlayer);
                }
            }

            if (attackPhaseTimer % 45 == 0 && Main.netMode == NetmodeID.MultiplayerClient)
            {
                Vector2 strikePos = target.Center + Main.rand.NextVector2Circular(400, 400);
                Projectile.NewProjectile(npc.GetSource_FromAI(), strikePos, Vector2.Zero,
                    ProjectileID.CultistBossLightningOrbArc,
                    npc.damage / 5, 3f, Main.myPlayer);
            }

            Vector2 wind = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 0.6f;
            target.velocity += wind;

            if (WorldSavingSystem.MasochistModeReal)
            {
                if (attackPhaseTimer % 120 == 0)
                {
                    target.gravity *= -1;
                }
            }

            if (attackPhaseTimer > duration)
            {
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                EndSpecialAttack(npc);
            }

        }

        private void RunYamiYamiNoMiPhase(NPC npc, Player target)
        {
            attackPhaseTimer++;
            int duration = WorldSavingSystem.MasochistModeReal ? 600 : 540;

            if (attackPhaseTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                Main.NewText("Zehahahaha", Color.DarkViolet);

                if (Main.netMode != NetModeID.MultiplayerClient)
                {
                    Vector2 spawn = npc.Center;
                    int singularity = Projectile.NewProjectile(
                        npc.GetSource_FromAI(), spawn, Vector2.Zero,
                        ModContent.ProjectileType<BlackHoleProjectile>(),
                        FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage) / 2, 0f, Main.myPlayer, npc.whoAmI);
                    npc.ai[3] = singularity;
                }
            }

            if (attackPhaseTimer > 60 && attackPhaseTimer < duration - 60)
            {
                Projectile core = Main.projectile[(int)npc.ai[3]];
                if (core.active)
                {
                    Vector2 pul = (core.Center - target.Center);
                    float dist = pull.Length();
                    if (dist < 1200f)
                    {
                        pull.Normalize();
                        float strength = MathHelper.Lerp(0.3f, 2.0f, 1f - dist / 1200f);
                        target.velocity += pull * strength * 0.2f;
                    }
                }
            }

            if (WorldSavingSystem.MasochistModeReal)
            {
                pullStrength *= 1.5f;

                if (attackPhaseTimer % 120 == 0)
                    Projectile.NewProjectile(npc.GetSource_FromAI(),
                        core.Center, Vector2.Zero, ProjectileID.ShadowBeamHostile,
                        FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage) / 3, 3f, Main.myPlayer);
            }

            if (attackPhaseTimer > duration)
                EndSpecialAttack(npc);
        }

        private void DoAbyssalKnightsPhase(NPC npc, Player target)
        {
            attackPhaseTimer++;
            int duration = WorldSavingSystem.MasochistModeReal ? 1200: 900;

            if (attackPhaseTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                Main.NewText("The Abyssal Knights heed their master's call!", Color.MediumPurple);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int count = WorldSavingSystem.MasochistModeReal ? 4 : 2;
                    for (int i = 0; i < count; i++)
                    {
                        Vector2 spawn = npc.Center + Main.rand.NextVector2Circular(600, 400);
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)spawn.X, (int)spawn.Y,
                            ModContent.NPCType<AbyssalKnight>(), 0, npc.whoAmI);
                    }
                }
            }

            if (attackPhaseTimer % 60 == 0)
                Lighting.AddLight(npc.Center, 0.4f, 0.1f, 0.5f);

            if (attackPhaseTimer > duration)
                EndSpecialAttack(npc);    
        }

        private void EndSpecialAttack()
        {
            inSpecialAttack = false;
            currentAttack = PWAttack.None;
            attackPhaseTimer = 0;
            chaseGrace = 45;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.netUpdate = true;

            if (IsMasochist())
            {
                if (currentAttack == PWAttack.Illusion)
                    StartSharkDevourPhase();
                else if (currentAttack == PWAttack.SharkDevour && Main.rand.NextBool(2))
                    StartIllusionPhase();    
            }
        }

        public override void FindFrame(int frameHeight)
        {
            //
        }

        private void MaybeChangeCycleByHP()
        {
            float lifePercent = NPC.life / (float)NPC.lifeMax;

            if (lifePercent < 0.75f)
                attackCycle = new int[] { 0, 1, 2, 1 };
            if (lifePercent < 0.40f)
                attackCycle = new int[] { 1, 2, 1, 2 };
        }

        public override void CheckDead()
        {
            if (currentAttack != PWAttack.Desperation && NPC.life <= 1)
            {
                StartDesperationPhase();
                return false;
            }
            return true;
        }

        private void StartDesperationPhase()
        {
            inSpecialAttack = true;
            currentAttack = PWAttack.Desperation;
            attackPhaseTimer = 0;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            NPC.defense = int.MaxValue;
            NPC.knockBackResist = 0f;
            SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
            Main.NewText("The Abyss begins to boil...", Color.MediumPurple);
        }
        
        private void RunDesperationPhase(NPC npc, Player target)
        {
            attackPhaseTimer++;

            int duration = WorldSavingSystem.MasochistModeReal ? 7200 : 7200;

            float surfaceY = Main.worldSurface * 16f - 200f;

            if (attackPhaseTimer % 30 == 0)
                SoundEngine.PlaySound(SoundID.Item74, npc.Center);

            if (Main.rand.NextBool(6))
                Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(400, 300),
                    DustID.Torch, Vector2.Zero, 100, Color.OrangeRed, 1.4f);

            target.AddBuff(BuffID.OnFire, 2);
            Lighting.AddLight(target.Center, 1f, 0.3f, 0.1f);

            Vector2 goal = new Vector2(npc.Center.X, surfaceY);
            Vector2 toGoal = (goal - npc.Center).SafeNormalize(Vector2.Zero);
            npc.velocity = Vector2.Lerp(npc.velocity, toGoal * 25f, 0.05f);

            if (target.Center.Y > surfaceY)
                target.velocity.Y -= 0.3f;

            if (attackPhaseTimer % 90 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPos = npc.Center + new Vector2(Main.rand.Next(-600, 600), -800f);
                Vector2 vel = Vector2.UnitY * (15f + Main.rand.NextFloat(5f));
                Projectile.NewProjectile(GetSource_FromAI(), spawnPos, vel,
                    ProjectileID.FallingStar, 150, 0f, Main.myPlayer);
            }                

            if (npc.Center.Y < surfaceY + 100)
            {
                if (attackPhaseTimer == duration - 240)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int xeroc = Projectile.NewProjectile(npc.GetSource_FromAI(),
                            npc.Center + new Vector2(0, -1000f),
                            Vector2.Zero,
                            ModContent.ProjectileType<XerocEyeProjectile>(),
                            0, 0f, Main.myPlayer);
                        npc.ai[3] = xeroc;
                    }

                    Main.NewText("The Stolen Light will be unleashed on its harshest detractor", Color.White);
                    SoundEngine.PlaySound(SoundID.Roar, npc.Centet);
                }

                if (attackPhaseTimer == duration)
                {
                    SoundEngine.PlaySound(SoundID.Item122, npc.Center);

                    Projectile.NewProjectile(npc.GetSource_FromAI(),
                        npc.Center + new Vector2(0, -800f),
                        Vector2.UnitY * 40f,
                        ModContent.ProjectileType<PrimordialLightBeamProjectile>(),
                        9999999, 10f, Main.myPlayer);

                    PLBscene.TriggerFlash(90f);    

                    npc.StrikeInstantKill();    
                }
            }
        }

        public override void OnKill()
        {
            //
        }
    }
}
