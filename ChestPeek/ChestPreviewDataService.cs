using StardewValley;
using StardewValley.Objects;

namespace ChestPeek;

/// <summary>Converts a Stardew Valley chest into preview data for logging and HUD drawing.</summary>
public sealed class ChestPreviewDataService
{
    public ChestPreviewData GetPreviewData(Chest chest)
    {
        List<ChestPreviewItemData> items = new();

        // Chest.Items may contain empty slots; the preview only renders real item stacks.
        foreach (Item? item in chest.Items)
        {
            if (item is null)
                continue;

            items.Add(new ChestPreviewItemData(item, item.DisplayName, item.Stack));
        }

        return new ChestPreviewData(chest, items);
    }
}
