using StardewValley;

namespace ChestPeek;

/// <summary>Display-ready data for one item stack in a chest preview.</summary>
public sealed record ChestPreviewItemData(Item Item, string DisplayName, int Stack);
