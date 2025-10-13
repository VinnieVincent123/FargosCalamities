using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Fargowiltas.Content.Items.Tiles;

namespace FargowiltasEternalBoss.Content.Tiles.CraftingStations
{
    public class ForgeCosmosTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4); // Uses 5x4 style, but reduces height to 3.
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.newTile.Origin = new Point16(2, 1);
            TileObjectData.addTile(Type);

            AnimationFrameHeight = 54;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Platinum; 

            AdjTiles = new int[] {
                TileID.WorkBenches,
                TileID.HeavyWorkBench, 
                TileID.Bottles, 
                TileID.Sawmill, 
                TileID.Loom, 
                TileID.CookingPots, 
                TileID.Sinks, 
                TileID.Kegs, 
                TileID.AlchemyTable,
                TileID.ImbuingStation, 
                TileID.DyeVat, 
                TileID.LivingLoom, 
                TileID.GlassKiln, 
                TileID.IceMachine, 
                TileID.HoneyDispenser, 
                TileID.SkyMill, 
                TileID.Solidifier, 
                TileID.BoneWelder, 
                TileID.Bookcases, 
                TileID.CrystalBall, 
                TileID.Autohammer,  
                TileID.LesionStation, 
                TileID.FleshCloningVat, 
                TileID.LihzahrdFurnace, 
                TileID.SteampunkBoiler, 
                TileID.Blendomatic, 
                TileID.MeatGrinder, 
                TileID.Tombstones, 
                ModContent.TileType<GoldenDippingVatSheet>(),
                TileID.Chairs,
                TileID.Tables,
                TileID.Anvils,
                TileID.MythrilAnvil,
                ModContent.TileType<CosmicAnvil>(),
                ModContent.TileType<DraedonsForge>(),
                ModContent.TileType<CrucibleCosmosSheet>(),
                TileID.Furnaces,
                TileID.Hellforge,
                TileID.AdamantiteForge,
                TileID.TinkerersWorkbench,
                TileID.LunarCraftingStation,
                TileID.DemonAltar,
            };
        }
    }
}
