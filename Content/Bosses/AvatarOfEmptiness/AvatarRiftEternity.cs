using System;
using ReLogic;
using Microsoft;
using Luminance;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using NoxusBoss;
using NoxusBoss.Content.NPCs.Bosses.Avatar.FirstPhaseForm;
using FargowiltasSouls;
using FargowiltasCrossmod;
using FargowiltasCrossmod.Core.Calamity.Globals;

namespace FargowiltasEternalBoss.Content.Bosses.AvatarOfEmptiness
{
    public class AvatarRiftEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<AvatarRift>();
        public override void SetDefaults()
        {
            NPC.lifeMax = (int)Math.Round(NPC.lifeMax * 0.8f);
            NPC.damage = 50;
        }
        public float[] drawInfo = [0, 200, 200, 0];
    }
}