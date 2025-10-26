using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm.Minions
{
    public class Noxus : ModNPC
    {
        private float appearanceTimer = 0f;
        private float attackTimer = 0f;
        private bool hasActivatedDistortion = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 283;
            NPC.height = 257;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.lavaImmune = true;
            NPC.friendly = false;
            NPC.hide = true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.active = false;
                return;
            }

            NPC.TargetClosest(false);
            NPC.rotation = 0f;
            appearanceTimer++;

            Vector2 targetPos = player.Center + new Vector2(0, -600f);
            NPC.alpha = (int)(255 * (1f - fadeIn));

            attackTimer++;
            if (attackTimer % 90 == 0)
            {
                if (!Main.dedServ)
        {
            (!Filters.Scene["WeBall:Distortion"].IsActivate())
            Filters.Scene["WeBall:Distortion"].Activate();

            var shader = Filters.Scene["WeBall:Distortion"].GetShader();
            shader.UseTargetPosition(Main.screenPosition);
            shader.Shader.Parameters["time"].SetValue((float)Main.GlobalTimeWrappedHourly);
            shader.Shader.Parameters["intensity"].SetValue(2f);

            Filters.Scene["WeBall:Distortion"].Deactivation(0.4f);
        }

               Vector2 dir = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
               Projectile.NewProjectileDirect(
                   NPC.GetSource_FromAI(),
                   NPC.Center,
                   dir * 8f,
                   ProjectileID.DD2ExplosiveTrapT3Explosion,
                   0, 0f, Main.myPlayer
               );

               SoundEngine.PlaySound(SoundID.Zombie104 with { Volume = 1.3f, Pitch = -0.6f }, NPC.Center);
            }

            if (appearanceTimer > 600)
            {
                NPC.alpha = (int)MathHelper.Lerp(NPC.alpha, 255, 0.1f);
                if (NPC.alpha >= 250)
                {
                    NPC.active = false;
                }
            }
        }

        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player player, Item item) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                    NPC.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.NPC[Type].Value;
            Vector2 origin = texture.Bounds.Size.ToVector2() / 2f;
            float fade = MathHelper.Clamp(appearanceTimer  / 120f, 0f, 1f);

            spriteBatch.Draw(
                texture,
                NPC.Center - screenPos,
                NPC.frame,
                Color.White * fade * 0.8f,
                NPC.rotation,
                origin,
                NPC.scale  * 1.2f,
                SpriteEffects.None,
                0f
            );
        }
    }
}