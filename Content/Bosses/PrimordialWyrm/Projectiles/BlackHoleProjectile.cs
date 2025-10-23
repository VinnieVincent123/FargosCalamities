

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm.Projectiles
{
public class BlackHoleProjectile : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.timeLeft = 540;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Projectile.rotation += 0.05f;
        Lighting.AddLight(Projectile.Center, 0.3f, 0.1f, 0.4f);

        foreach (Projectile proj in Main.projectile)
        {
            if (proj.active && proj.friendly && !proj.minion && proj != Projectile)
            {
                Vector2 pull = Projectile.Center - proj.Center;
                float dist = pull.Length();
                if (dist < 600f)
                {
                    pull.Normalize();
                    proj.velocity += pull * MathHelper.Lerp(0.5f, 3f, 1f - dist / 600f);
                }
            }
        }

        foreach (Projectile minion in Main.projectile)
        {
            if (minion.active && minion.minion)
            {
                Vector2 pull = Projectile.Center - minion.Center;
                float dist = pull.Length();
                if (dist < 700f)
                {
                    pull.Normalize();
                    minion.velocity += pull * 0.4f;
                }
            }
        }

        if (Projectile.timeLeft == 60)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 40; i++)
                Dust.NewDustPerfect(Projectile.Center,
                    DustID.PurpleTorch, Main.rand.NextVector2Circular(8, 8), 100, Color.Purple, 2f);
        }

        if (Projectile.timeLeft == 1 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                Projectile.Center, Vector2.Zero, ProjectileID.BlackHoleExplosion,
                Projectile.damage * 3, 8f, Main.myPlayer);
        }
    }
}
}