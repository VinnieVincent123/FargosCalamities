using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasSouls;
using NoxusBoss;
using CalamityMod.NPCs.PrimordialWyrm;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Core.Calamity.Globals;

namespace FargowiltasEternalBoss.Content.Bosses.PrimordialWyrm
{
    public class JaredBody : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<PrimordialWyrmBody>();
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.5f);
            NPC.damage = 75;
            NPC.Calamity().DR = 0.5f;
        }
    }
}
