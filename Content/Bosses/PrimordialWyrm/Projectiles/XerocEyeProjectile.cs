

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm.Projectiles
{
    public class XerocEyeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
            {
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                if (owner.active)
                {
                    Projectile.Center = owner.Center + new Vector2(0, -800f);
                }
            }

            Projectile.rotation += 0.005f;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.1f, 0.6f);
        }
    }
}