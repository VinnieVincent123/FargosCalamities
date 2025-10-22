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
            SharkDevour = 2
            //Add more here later
        }

        private int attackIndex;
        private int[] attackCycle = new int[] { 0, 1, 0, 2};
        private int attackTimer;
        private bool inSpecialAttack;
        private PWAttack currentAttack = PWAttack.None;

        private int attackPhaseTimer;
        private int chaseGrace = 0;

        private const byte NET_SYNC_ATTACK = 1;

        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.5f);
            NPC.damage = (int)Math.Round(NPC.damage * 1.2f);
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

                if (attackTimer >= 540 + Main.rand.Next(-120, 121))
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

            if (inSpecialAttack)
            {
                switch (currentAttack)
                {
                    case PWAttack.Illusion:
                        RunIllusionPhase(target);
                        break;
                    case PWAttack.SharkDevour:
                        RunSharkDevourPhase(target);
                        break;
                    default:
                       ChasePlayer(target);
                       break;        
                }
            }
            else
            {
                ChasePlayer(target);
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

        private void EndSpecialAttack()
        {
            inSpecialAttack = false;
            currentAttack = PWAttack.None;
            attackPhaseTimer = 0;
            chaseGrace = 45;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.netUpdate = true;
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

        public override void OnKill()
        {

        }
    }
}
