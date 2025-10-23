

public class PrimordialLightBeamProjectile : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 520;
        Projectile.height = 1000;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 60;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 1.2f, 1.1f, 0.8f);
        if (Main.rand.NextBool(3))
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame);
    }
}