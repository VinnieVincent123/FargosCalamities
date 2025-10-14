using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasEternalBoss.Content.Bosses.CalCloneHuman;

namespace FargowiltasEternalBoss.Core.Globals
{
    public class CalCloneGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private bool transformed;

        public override void AI(NPC npc)
        {
            if (!ModLoader.TryGetMod("CalamityMod", out var calamity))
            return;

            int calCloneType = calamity.Find<ModNPC>("CalamitasClone")?.Type ?? -1;
            if (calCloneType == -1 || npc.type != calCloneType)
            return;

            bool eternity = ModLoader.TryGetMod("FargowiltasSouls", out var fargo) &&
            fargo.Call("EternityModeActive") is bool e && e;

            if (!transformed && eternity &&  npc.life < npc.lifeMax * 0.2f)
            {
                TransformToMyNPC(npc);
                transformed = true;
            } 
        }

        private void TransformToMyNPC(NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

            Vector2 pos = npc.Center;
            float ratio = (float)npc.life / npc.lifeMax;

            npc.active = false;
            npc.netUpdate = true;

            int newType = ModContent.NPCType<Woman>();
            int id = NPC.NewNPC(npc.GetSource_FromAI(), (int)pos.X, (int)pos.Y, newType);
            if (id >= 0)
            {
                NPC newBoss = Main.npc[id];
                newBoss.life = (int)(newBoss.lifeMax * ratio);
                newBoss.TargetClosest(true);
                newBoss.netUpdate = true;

                SoundEngine.PlaySound(SoundID.Item14, pos);
                for (int i = 0; i < 50; i++)
                Dust.NewDust(pos - new Vector2(50, 50), 100, 100, DustID.FireworkFountain_Red,
                Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6));
                Main.NewText("The Cacoon broke!", Color.MediumVioletRed);
            }
        }
    }
}