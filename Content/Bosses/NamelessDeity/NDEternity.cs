using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasSouls;
using NoxusBoss;
using NoxusBoss.Content.NPCs.Bosses.NamelessDeity;
using FargowiltasCrossmod;
using FargowiltasCrossmod.Core.Calamity.Globals;

namespace FargowiltasEternalBoss.Content.Bosses.NamelessDeity
{
    public class NDEvernity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<NamelessDeityBoss>();
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 1.25f);
            NPC.damage = 75;
        }
    }
}