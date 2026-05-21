using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace ChestPeek.UI;

/// <summary>A transparent menu that lets vanilla single-player menu pause rules freeze the world.</summary>
public sealed class PausePreviewMenu : IClickableMenu
{
    private readonly ModConfig config;
    private readonly ChestHoverService chestHoverService;
    private readonly ChestPreviewDataService chestPreviewDataService;
    private readonly ChestPreviewRenderer chestPreviewRenderer;
    private ChestPreviewData? currentPreviewData;

    public PausePreviewMenu(
        ModConfig config,
        ChestHoverService chestHoverService,
        ChestPreviewDataService chestPreviewDataService,
        ChestPreviewRenderer chestPreviewRenderer)
    {
        this.config = config;
        this.chestHoverService = chestHoverService;
        this.chestPreviewDataService = chestPreviewDataService;
        this.chestPreviewRenderer = chestPreviewRenderer;
    }

    public override void update(GameTime time)
    {
        base.update(time);
        this.UpdatePreviewData();
    }

    public override void draw(SpriteBatch b)
    {
        if (this.currentPreviewData is not null)
            this.chestPreviewRenderer.Draw(b, this.currentPreviewData, this.config);

        this.drawMouse(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        // Intentionally swallow clicks so pause preview cannot open or interact with chests.
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        // Intentionally swallow right-click interactions while the world is paused.
    }

    private void UpdatePreviewData()
    {
        ChestHoverResult? hoverResult = this.chestHoverService.GetHoveredChest();

        if (hoverResult?.Chest is null)
        {
            this.currentPreviewData = null;
            return;
        }

        this.currentPreviewData = this.chestPreviewDataService.GetPreviewData(hoverResult.Chest);
    }
}
