using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod;
using NoxusBoss;
using NoxusBoss.Content.NPCs.Bosses.Avatar.SecondPhaseForm;
using FargowiltasSouls;
using FargowiltasCrossmod;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasEternalBoss.Content.Bosses.AvatarOfEmptiness
{
    public class AoEEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<AvatarOfEmptiness>();
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 0.85f);
            NPC.damage = 50;
        }
    }
}