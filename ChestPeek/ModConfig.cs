using StardewModdingAPI.Utilities;

namespace ChestPeek;

/// <summary>Screen anchor used by the preview card.</summary>
internal enum PreviewPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}

/// <summary>Persistent player-facing options for Chest Peek.</summary>
public sealed class ModConfig
{
    /// <summary>Whether previewing should require holding the configured keybind.</summary>
    public bool UseHotkeyPreview { get; set; } = true;

    /// <summary>The keybind that gates previewing when hotkey preview is enabled.</summary>
    public KeybindList PreviewKey { get; set; } = KeybindList.Parse("LeftShift");

    /// <summary>Whether holding the preview key should open a lightweight pause menu in single-player.</summary>
    public bool EnablePausePreview { get; set; } = true;

    /// <summary>Maximum item slots to draw before wrapping to the next row.</summary>
    internal int MaxItemsPerRow { get; set; } = 20;

    /// <summary>Text shown when the hovered container has no visible item stacks.</summary>
    internal string EmptyText { get; set; } = "Empty";

    /// <summary>
    /// Whether to manually multiply preview dimensions by Stardew Valley's UI scale.
    /// Leave disabled because HUD drawing already follows the game's UI scale as a whole.
    /// </summary>
    internal bool FollowGameUiScale { get; set; } = false;

    /// <summary>Extra scale multiplier applied to the whole preview UI.</summary>
    internal float PreviewScale { get; set; } = 1f;

    /// <summary>Where the preview card should be anchored on screen.</summary>
    internal PreviewPosition PreviewPosition { get; set; } = PreviewPosition.TopCenter;

    /// <summary>Horizontal offset applied after the preview position is calculated.</summary>
    internal int PreviewOffsetX { get; set; } = 0;

    /// <summary>Vertical offset applied after the preview position is calculated.</summary>
    internal int PreviewOffsetY { get; set; } = 0;

    /// <summary>Whether the preview should be clamped inside the visible UI viewport.</summary>
    internal bool ClampToScreen { get; set; } = true;

    /// <summary>Whether to draw the main item panel background.</summary>
    internal bool ShowMainPanel { get; set; } = true;

    /// <summary>Whether to draw panels using Stardew Valley's vanilla menu border.</summary>
    internal bool UseVanillaTextureBox { get; set; } = false;

    /// <summary>Preview panel background color.</summary>
    internal int PanelRed { get; set; } = 238;

    /// <summary>Preview panel background color.</summary>
    internal int PanelGreen { get; set; } = 211;

    /// <summary>Preview panel background color.</summary>
    internal int PanelBlue { get; set; } = 162;

    /// <summary>Preview panel opacity from 0 to 1.</summary>
    internal float PanelOpacity { get; set; } = 0.88f;

    /// <summary>Vanilla texture box tint color.</summary>
    internal int TextureBoxRed { get; set; } = 255;

    /// <summary>Vanilla texture box tint color.</summary>
    internal int TextureBoxGreen { get; set; } = 255;

    /// <summary>Vanilla texture box tint color.</summary>
    internal int TextureBoxBlue { get; set; } = 255;

    /// <summary>Vanilla texture box opacity from 0 to 1.</summary>
    internal float TextureBoxOpacity { get; set; } = 1f;

    internal int PanelPadding { get; set; } = 10;

    internal int PanelGap { get; set; } = 8;

    internal int ScreenMargin { get; set; } = 20;

    internal int IconSize { get; set; } = 36;

    internal int ContainerIconSize { get; set; } = 38;

    internal int CellWidth { get; set; } = 54;

    internal int CellHeight { get; set; } = 62;

    internal int RowGap { get; set; } = 10;

    internal int ColumnGap { get; set; } = 6;

    internal int EmptyContentWidth { get; set; } = 120;

    internal int EmptyContentHeight { get; set; } = 28;

    /// <summary>Whether to draw the separate container icon panel.</summary>
    internal bool ShowContainerPanel { get; set; } = true;

    /// <summary>Whether to draw the hovered container icon.</summary>
    internal bool ShowContainerIcon { get; set; } = true;

    /// <summary>Whether the container panel should appear to the right of the item panel.</summary>
    internal bool ContainerPanelOnRight { get; set; } = false;

    /// <summary>Opacity for the item icons from 0 to 1.</summary>
    internal float ItemIconOpacity { get; set; } = 1f;

    /// <summary>Opacity for the container icon from 0 to 1.</summary>
    internal float ContainerIconOpacity { get; set; } = 1f;

    /// <summary>Whether item icons draw their menu shadow.</summary>
    internal bool DrawItemIconShadow { get; set; } = true;

    /// <summary>Whether the container icon draws its menu shadow.</summary>
    internal bool DrawContainerIconShadow { get; set; } = true;

    /// <summary>Empty container text color.</summary>
    internal int EmptyTextRed { get; set; } = 86;

    /// <summary>Empty container text color.</summary>
    internal int EmptyTextGreen { get; set; } = 43;

    /// <summary>Empty container text color.</summary>
    internal int EmptyTextBlue { get; set; } = 28;

    /// <summary>Multiplier applied to empty container text size.</summary>
    internal float EmptyTextScaleMultiplier { get; set; } = 1f;

    /// <summary>Smallest allowed item stack number scale.</summary>
    internal float MinStackTextScale { get; set; } = 0.5f;

    /// <summary>Multiplier applied to item stack number size.</summary>
    internal float StackTextScaleMultiplier { get; set; } = 1.8f;

    /// <summary>Item stack number text color.</summary>
    internal int StackTextRed { get; set; } = 76;

    /// <summary>Item stack number text color.</summary>
    internal int StackTextGreen { get; set; } = 45;

    /// <summary>Item stack number text color.</summary>
    internal int StackTextBlue { get; set; } = 25;

    /// <summary>Whether item stack numbers are drawn.</summary>
    internal bool ShowStackText { get; set; } = true;

    /// <summary>Whether item stack number 1 should be drawn too.</summary>
    internal bool ShowStackTextForSingleItems { get; set; } = false;

    internal int StackTextNudgeX { get; set; } = 4;

    internal int StackTextNudgeY { get; set; } = 4;

    /// <summary>Whether text should draw a shadow before the main text.</summary>
    internal bool DrawTextShadow { get; set; } = true;

    /// <summary>Text shadow color.</summary>
    internal int TextShadowRed { get; set; } = 255;

    /// <summary>Text shadow color.</summary>
    internal int TextShadowGreen { get; set; } = 242;

    /// <summary>Text shadow color.</summary>
    internal int TextShadowBlue { get; set; } = 210;

    /// <summary>Text shadow opacity from 0 to 1.</summary>
    internal float TextShadowOpacity { get; set; } = 0.35f;

    internal int TextShadowOffsetX { get; set; } = 1;

    internal int TextShadowOffsetY { get; set; } = 1;

    internal int ItemIconNudgeX { get; set; } = -10;

    internal int ItemIconNudgeY { get; set; } = -10;

    internal int ContainerIconNudgeX { get; set; } = -10;

    internal int ContainerIconNudgeY { get; set; } = -10;
}
