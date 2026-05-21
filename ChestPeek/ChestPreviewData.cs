using StardewValley.Objects;

namespace ChestPeek;

/// <summary>Immutable snapshot of the chest contents currently shown by the HUD preview.</summary>
public sealed record ChestPreviewData(Chest Container, IReadOnlyList<ChestPreviewItemData> Items)
{
    /// <summary>Whether the hovered chest has no visible item stacks.</summary>
    public bool IsEmpty => this.Items.Count == 0;
}
