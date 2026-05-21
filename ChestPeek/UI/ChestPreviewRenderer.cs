using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ChestPeek.UI;

/// <summary>Draws a compact translucent chest preview card on the HUD.</summary>
public sealed class ChestPreviewRenderer
{
    private const int DefaultItemsPerRow = 10;

    public void Draw(SpriteBatch spriteBatch, ChestPreviewData data, ModConfig config)
    {
        float uiScale = this.GetManualScale(config);
        int itemsPerRow = Math.Clamp(config.MaxItemsPerRow <= 0 ? DefaultItemsPerRow : config.MaxItemsPerRow, 1, 20);

        int padding = this.Scale(config.PanelPadding, uiScale);
        int panelGap = this.Scale(config.PanelGap, uiScale);
        int iconSize = this.Scale(config.IconSize, uiScale);
        int containerIconSize = this.Scale(config.ContainerIconSize, uiScale);
        int cellWidth = this.Scale(config.CellWidth, uiScale);
        int cellHeight = this.Scale(config.CellHeight, uiScale);
        int rowGap = this.Scale(config.RowGap, uiScale);
        int columnGap = this.Scale(config.ColumnGap, uiScale);

        int columns = data.IsEmpty ? 1 : Math.Min(data.Items.Count, itemsPerRow);
        int rows = data.IsEmpty ? 1 : (int)Math.Ceiling(data.Items.Count / (double)columns);
        int gridWidth = data.IsEmpty
            ? this.Scale(config.EmptyContentWidth, uiScale)
            : columns * cellWidth + Math.Max(0, columns - 1) * columnGap;
        int gridHeight = data.IsEmpty
            ? this.Scale(config.EmptyContentHeight, uiScale)
            : rows * cellHeight + Math.Max(0, rows - 1) * rowGap;

        int mainPanelWidth = gridWidth + padding * 2;
        int mainPanelHeight = gridHeight + padding * 2;
        int containerPanelSize = containerIconSize + padding * 2;
        bool showContainerPanel = config.ShowContainerPanel || config.ShowContainerIcon;
        int groupWidth = mainPanelWidth + (showContainerPanel ? containerPanelSize + panelGap : 0);
        int groupHeight = Math.Max(mainPanelHeight, showContainerPanel ? containerPanelSize : 0);
        Point groupPosition = this.GetCardPosition(groupWidth, groupHeight, config, uiScale);
        int mainPanelX = groupPosition.X;
        if (showContainerPanel && !config.ContainerPanelOnRight)
            mainPanelX += containerPanelSize + panelGap;

        Rectangle mainPanel = new(mainPanelX, groupPosition.Y, mainPanelWidth, mainPanelHeight);
        int containerPanelX = config.ContainerPanelOnRight
            ? mainPanel.Right + panelGap
            : groupPosition.X;
        Rectangle containerPanel = new(containerPanelX, groupPosition.Y, containerPanelSize, containerPanelSize);

        if (config.ShowMainPanel)
            this.DrawPanel(spriteBatch, mainPanel, config);

        if (showContainerPanel && config.ShowContainerPanel)
            this.DrawPanel(spriteBatch, containerPanel, config);

        if (showContainerPanel && config.ShowContainerIcon)
            this.DrawContainerIcon(spriteBatch, data, containerPanel, containerIconSize, config, uiScale);

        int gridX = mainPanel.X + padding;
        int gridY = mainPanel.Y + padding;

        if (data.IsEmpty)
        {
            float emptyTextScale = Math.Max(0.1f, this.GetIconScale(iconSize) * Math.Max(0.1f, config.EmptyTextScaleMultiplier));
            Color emptyTextColor = this.GetColor(config.EmptyTextRed, config.EmptyTextGreen, config.EmptyTextBlue);

            this.DrawText(spriteBatch, config.EmptyText, new Vector2(gridX, gridY), emptyTextScale, emptyTextColor, config);
            return;
        }

        for (int i = 0; i < data.Items.Count; i++)
        {
            ChestPreviewItemData itemData = data.Items[i];
            Point cellPosition = this.GetCellPosition(i, columns, gridX, gridY, cellWidth, cellHeight, columnGap, rowGap);
            this.DrawItemCell(spriteBatch, itemData, cellPosition.X, cellPosition.Y, cellWidth, iconSize, config, uiScale);
        }
    }

    private float GetManualScale(ModConfig config)
    {
        float gameScale = config.FollowGameUiScale
            ? Math.Clamp(Game1.options.uiScale, 0.75f, 2f)
            : 1f;
        float previewScale = Math.Clamp(config.PreviewScale, 0.25f, 4f);

        return gameScale * previewScale;
    }

    private int Scale(int value, float uiScale)
    {
        return Math.Max(1, (int)Math.Round(value * uiScale));
    }

    private Point GetCardPosition(int cardWidth, int cardHeight, ModConfig config, float uiScale)
    {
        int margin = this.Scale(config.ScreenMargin, uiScale);
        int x = config.PreviewPosition switch
        {
            PreviewPosition.TopLeft or PreviewPosition.CenterLeft or PreviewPosition.BottomLeft => margin,
            PreviewPosition.TopRight or PreviewPosition.CenterRight or PreviewPosition.BottomRight => Game1.uiViewport.Width - cardWidth - margin,
            _ => (Game1.uiViewport.Width - cardWidth) / 2
        };
        int y = config.PreviewPosition switch
        {
            PreviewPosition.TopLeft or PreviewPosition.TopCenter or PreviewPosition.TopRight => margin,
            PreviewPosition.BottomLeft or PreviewPosition.BottomCenter or PreviewPosition.BottomRight => Game1.uiViewport.Height - cardHeight - margin,
            _ => (Game1.uiViewport.Height - cardHeight) / 2
        };

        x += this.ScaleSigned(config.PreviewOffsetX, uiScale);
        y += this.ScaleSigned(config.PreviewOffsetY, uiScale);

        if (!config.ClampToScreen)
            return new Point(x, y);

        int maxX = Math.Max(margin, Game1.uiViewport.Width - cardWidth - margin);
        int maxY = Math.Max(margin, Game1.uiViewport.Height - cardHeight - margin);

        return new Point(Math.Clamp(x, margin, maxX), Math.Clamp(y, margin, maxY));
    }

    private Point GetCellPosition(int index, int columns, int gridX, int gridY, int cellWidth, int cellHeight, int columnGap, int rowGap)
    {
        int column = index % columns;
        int row = index / columns;

        return new Point(gridX + column * (cellWidth + columnGap), gridY + row * (cellHeight + rowGap));
    }

    private void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds, ModConfig config)
    {
        if (config.UseVanillaTextureBox)
        {
            Color textureColor = this.GetColor(config.TextureBoxRed, config.TextureBoxGreen, config.TextureBoxBlue)
                * Math.Clamp(config.TextureBoxOpacity, 0f, 1f);

            IClickableMenu.drawTextureBox(spriteBatch, bounds.X, bounds.Y, bounds.Width, bounds.Height, textureColor);
            return;
        }

        Color panelColor = this.GetColor(config.PanelRed, config.PanelGreen, config.PanelBlue);
        float opacity = Math.Clamp(config.PanelOpacity, 0f, 1f);
        spriteBatch.Draw(Game1.staminaRect, bounds, panelColor * opacity);
    }

    private void DrawContainerIcon(SpriteBatch spriteBatch, ChestPreviewData data, Rectangle panel, int iconSize, ModConfig config, float uiScale)
    {
        float iconScale = this.GetIconScale(iconSize);
        Vector2 position = this.GetCenteredIconPosition(
            panel.X,
            panel.Y,
            panel.Width,
            panel.Height,
            iconSize,
            config.ContainerIconNudgeX,
            config.ContainerIconNudgeY,
            uiScale);

        data.Container.drawInMenu(
            spriteBatch,
            position,
            iconScale,
            Math.Clamp(config.ContainerIconOpacity, 0f, 1f),
            0.95f,
            StackDrawType.Hide,
            Color.White,
            config.DrawContainerIconShadow);
    }

    private void DrawItemCell(SpriteBatch spriteBatch, ChestPreviewItemData itemData, int cellX, int cellY, int cellWidth, int iconSize, ModConfig config, float uiScale)
    {
        float iconScale = this.GetIconScale(iconSize);
        Vector2 iconPosition = this.GetCenteredIconPosition(
            cellX,
            cellY,
            cellWidth,
            iconSize,
            iconSize,
            config.ItemIconNudgeX,
            config.ItemIconNudgeY,
            uiScale);

        itemData.Item.drawInMenu(
            spriteBatch,
            iconPosition,
            iconScale,
            Math.Clamp(config.ItemIconOpacity, 0f, 1f),
            0.9f,
            StackDrawType.Hide,
            Color.White,
            config.DrawItemIconShadow);

        if (config.ShowStackText && (itemData.Stack > 1 || config.ShowStackTextForSingleItems))
            this.DrawStackNumber(spriteBatch, itemData.Stack.ToString(), cellX, cellY + iconSize, cellWidth, iconScale, config, uiScale);
    }

    private Vector2 GetCenteredIconPosition(int areaX, int areaY, int areaWidth, int areaHeight, int iconSize, int nudgeX, int nudgeY, float uiScale)
    {
        return new Vector2(
            areaX + (areaWidth - iconSize) / 2f + nudgeX * uiScale,
            areaY + (areaHeight - iconSize) / 2f + nudgeY * uiScale);
    }

    private float GetIconScale(int iconSize)
    {
        return iconSize / 64f;
    }

    private int ScaleSigned(int value, float uiScale)
    {
        return (int)Math.Round(value * uiScale);
    }

    private Color GetColor(int red, int green, int blue)
    {
        return new Color(
            Math.Clamp(red, 0, 255),
            Math.Clamp(green, 0, 255),
            Math.Clamp(blue, 0, 255));
    }

    private void DrawStackNumber(SpriteBatch spriteBatch, string text, int cellX, int numberY, int cellWidth, float iconScale, ModConfig config, float uiScale)
    {
        float textScale = Math.Max(config.MinStackTextScale, iconScale * Math.Max(0.1f, config.StackTextScaleMultiplier));
        Vector2 textSize = Game1.smallFont.MeasureString(text) * textScale;
        Vector2 position = new(
            cellX + (cellWidth - textSize.X) / 2f + config.StackTextNudgeX * uiScale,
            numberY + config.StackTextNudgeY * uiScale);

        Color stackTextColor = this.GetColor(config.StackTextRed, config.StackTextGreen, config.StackTextBlue);

        this.DrawText(spriteBatch, text, position, textScale, stackTextColor, config);
    }

    private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, float scale, Color color, ModConfig config)
    {
        if (config.DrawTextShadow)
        {
            Color shadowColor = this.GetColor(config.TextShadowRed, config.TextShadowGreen, config.TextShadowBlue)
                * Math.Clamp(config.TextShadowOpacity, 0f, 1f);
            Vector2 shadowOffset = new(config.TextShadowOffsetX * scale, config.TextShadowOffsetY * scale);

            spriteBatch.DrawString(Game1.smallFont, text, position + shadowOffset, shadowColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.91f);
        }

        spriteBatch.DrawString(Game1.smallFont, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.92f);
    }
}
