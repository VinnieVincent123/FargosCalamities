using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.ItemDropRules;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasCrossmod;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Potions;
using NoxusBoss;
using HeavenlyArsenal;
using Luminance;
using Daybreak;
using Microsoft;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic;

namespace FargowiltasEternalBoss.Content.Bosses.Champions.ChampOne
{
    public class ChampionGale : ModNPC
    {
        public const int SpawnNoContactTimer = 60 * 4;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);

            NPC.AddDebuffImmunities(
            [    
                BuffID.Confused,
                BuffID.Chilled,
                BuffID.OnFire,
                BuffID.Suffocation,
                ModContent.BuffType<LethargicBuff>(),
                ModContent.BuffType<AstralInfectionDebuff>(),
                ModContent.BuffType<BanishingFire>(),
                ModContent.BuffType<BrimstoneFlames>(),
                ModContent.BuffType<GodEaterBuff>(),
                ModContent.BuffType<Dragonfire>(),
                ModContent.BuffType<ElementalMix>(),
                ModContent.BuffType<GodSlayerInferno>(),
                ModContent.BuffType<MiracleBlight>()
            ]);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(16 * 4, 16 * 9),
                PortraitPositionXOverride = 16,
                PortraitPositionYOverride = 16 * 7
            });
            
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.FargowiltasEternalBoss.Bestiary.ChampionGale")
            ]);
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 234;
            NPC.damage = 110;
            NPC.defense = 50;
            NPC.lifeMax = 2500000;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(4);
            NPC.boss = true;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (SpawnNoContactTimer > 0)
                return false;
            CooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DesertProwlerEnchant>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SnowRuffianEnchant>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StatigelEnchant>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SulphurEnchant>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VictideEnchant>(), 5));
        }
    }
}