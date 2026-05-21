using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace ChestPeek;

/// <summary>Represents the cursor tile and the chest found there, if one exists.</summary>
public sealed record ChestHoverResult(Chest? Chest, Vector2 Tile);
