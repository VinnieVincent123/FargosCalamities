using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using FargowiltasSouls;
using FargowiltasCrossmod;
using FargowiltasEternalBoss.Content.Items.Accessories.Eternity;

namespace FargowiltasEternalBoss.Content.Projectiles.Accessories.IlmereanCrest
{
    public class JellyFriends : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Generic;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
           ModContent.BuffType<GalvanicCorrosion>(), 240;
        }
    }
}
