using Terraria; 
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Materials; 
using CalamityMod.Rarities;
using FargowiltasSouls;
using Fargowiltas.Content.Items.Tiles;

namespace FargowiltasEternalBoss.Content.Items.CraftingStations
{
    public class ForgeCosmos : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Content.Tiles.CraftingStations.ForgeCosmosTile>();

            Item.rare = ModContent.RarityType<Violet>();
            Item.value = Item.sellPrice(platinum: 35);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DraedonsForge>().
                AddIngredient<CrucibleCosmos>().
                AddIngredient<ShadowspecBar>(5).
                Register();
        }
    }
}