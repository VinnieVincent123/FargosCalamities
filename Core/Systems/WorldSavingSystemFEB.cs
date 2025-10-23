using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasEternalBoss.Core.Systems
{
    public class WorldSavingSystemFEB : ModSystem
    {
        public enum downed
        {
            ChallengerGale,
            ChallengerElements,
            ChallengerDevastation,
            ChallengerPestilence,
            ChallengerWar,
            ChallengerFamine,
            ChallengerGreed,
            ChampionGale,
            ChampionElements,
            ChampionDevastation,
            ChampionPestilence,
            ChampionWar,
            ChampionFamine,
            ChampionGreed,
            ChampionGods
        }
    }
}