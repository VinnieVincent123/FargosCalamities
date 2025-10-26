using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm.Minions
{
    public class AbyssalKnight : ModNPC
    {
        private int swingTimer;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 64;
            NPC.damage = 700;
            NPC.defense = 120;
            NPC.lifeMax = 250000;
            NPC.knockBackResist = 0f;
            NPC.value = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            if (!target.active || target.dead)
            {
                NPC.TargetClosest();
                return;
            }

            float speed = 6f;
            Vector2 move = target.Center - NPC.Center;
            move.Normalize();
            NPC.velocity = Vector2.Lerp(NPC.velocity, move * speed, 0.1f);

            if (++swingTime >= 90)
            {
                swingTimer = 0;
                SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
                Vector2 dir = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir * 14f,
                        ModContent.ProjectileType<AbyssalSlash>(), 80, 3f, Main.myPlayer);
            }

            Lighting.AddLight(NPC.Center, 0.2f, 0.05f, 0.25f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.PurpleTorch,
                        Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-4, 4));
            }
        }
    }
}