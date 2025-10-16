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
using FargowiltasSouls.Common.Utilities;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasCrossmod;
using FargowiltasEternalBoss;
using Luminance.Core.Graphics;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasEternalBoss.Content.Bosses.Champions.ChampSix
{
    public class ChampionFamine : ModNPC
    {

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
    }
}
