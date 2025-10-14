using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasSouls;
using NoxusBoss;
using CalamityMod.NPCs.PrimordialWyrm;
using FargowiltasCrossmod;
using FargowiltasCrossmod.Core.Calamity.Globals;

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm
{
    public class Jared : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<PrimordialWyrmHead>();
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 0.65f);
            NPC.damage = 75;
            NPC.Calamity().DR = 0.5f;
        }
    }
}
