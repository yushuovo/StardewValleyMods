using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using xTile.Layers;
using SObject = StardewValley.Object;

namespace ChestPeek;

/// <summary>Finds the chest, if any, under the mouse cursor's actual map tile.</summary>
public sealed class ChestHoverService
{
    private readonly IModHelper helper;

    public ChestHoverService(IModHelper helper)
    {
        this.helper = helper;
    }

    public ChestHoverResult? GetHoveredChest()
    {
        if (!Context.IsWorldReady || Game1.currentLocation is null)
            return null;

        // Use Tile rather than GrabTile so the preview follows the cursor's visual tile,
        // not Stardew Valley's current interaction target.
        Vector2 tile = this.helper.Input.GetCursorPosition().Tile;

        if (Game1.currentLocation.objects.TryGetValue(tile, out SObject obj) && obj is Chest chest)
            return new ChestHoverResult(chest, tile);

        if (this.TryGetFarmHouseFridge(tile, out Chest? fridge))
            return new ChestHoverResult(fridge, tile);

        // Returning the tile even when it has no chest keeps hover debugging precise.
        return new ChestHoverResult(null, tile);
    }

    private bool TryGetFarmHouseFridge(Vector2 tile, out Chest? fridge)
    {
        fridge = null;

        if (Game1.currentLocation is not FarmHouse farmHouse)
            return false;

        if (!this.TileHasFridgeAction(tile))
            return false;

        fridge = farmHouse.fridge.Value;
        return fridge is not null;
    }

    private bool TileHasFridgeAction(Vector2 tile)
    {
        int x = (int)tile.X;
        int y = (int)tile.Y;

        foreach (Layer layer in Game1.currentLocation.map.Layers)
        {
            if (x < 0 || y < 0 || x >= layer.LayerWidth || y >= layer.LayerHeight)
                continue;

            string action = layer.Tiles[x, y]?.Properties.TryGetValue("Action", out xTile.ObjectModel.PropertyValue? value) == true
                ? value.ToString()
                : string.Empty;

            if (action.Contains("Fridge", StringComparison.OrdinalIgnoreCase) || action.Contains("Refrigerator", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
