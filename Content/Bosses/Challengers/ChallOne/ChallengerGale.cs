using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using Fargowiltas;
using FargowiltasSouls;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasCrossmod;
using FargowiltasEternalBoss.Core.Systems;
using Luminance.Core.Graphics;
using Luminance.Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasEternalBoss.Content.Bosses.Challengers.ChallOne
{
    public class ChallengerGale : ModNPC
    {
        private Action<NPC> difficultyBehavior;
        private bool initialized;

        public static bool bossAlive;

        public override void SetStaticDefaults()
        {

            NPC.AddDebuffImmunities(
            [
                BuffID.Confused,
                    ModContent.BuffType<LethargicBuff>()
            ]);
        }

        public override void SetDefaults()
        {
            NPC.height = 403;
            NPC.width = 572;
            NPC.aiStyle = -1;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.Calamity().DR = 0.1f;
            NPC.boss = true;
            NPC.lifeMax = 6800;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ChallengerGale");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(NPC.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            NPC.scale = reader.ReadSingle();
        }

        public override void AI()
        {
           if (!initialized)
           {
               SelectDifficultyBehavior();
               initialized = true;
           }

           difficultyBehavior?.Invoke(NPC);
        }

        

        private void SelectDifficultyBehavior()
        {
            if (WorldSavingSystem.EternityMode)
                difficultyBehavior = EternityAI;
            else if (WorldSavingSystem.MasochistModeReal)
                difficultyBehavior = MasochistAI;
            else if (WorldSavingSystem.MasochistModeReal && Main.getGoodWorld)
                difficultyBehavior = JustDieAI;    
            else if (Main.getGoodWorld)
                difficultyBehavior = LegendaryAI;    
            else if (Main.masterMode)
                difficultyBehavior = MasterAI;
            else if (Main.expertMode)
                difficultyBehavior = ExpertAI;
            else
                difficultyBehavior = ClassicAI;                
        }

        private void ClassicAI(NPC npc)
        {
            //
        }

        private void ExpertAI(NPC npc)
        {
            //
        }

        private void MasterAI(NPC npc)
        {
            //
        }

        private void EternityAI(NPC npc)
        {
            //
        }

        private void MasochistAI(NPC npc)
        {
            //
        }

        private void LegendaryAI(NPC npc)
        {
            //
        }

        private void JustDieAI(NPC npc)
        {
            //
        }
    }
}
