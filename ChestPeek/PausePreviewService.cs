using ChestPeek.UI;
using StardewModdingAPI;
using StardewValley;

namespace ChestPeek;

/// <summary>Opens and closes the lightweight menu used by single-player pause preview mode.</summary>
public sealed class PausePreviewService
{
    private readonly ModConfig config;
    private readonly ChestHoverService chestHoverService;
    private readonly ChestPreviewDataService chestPreviewDataService;
    private readonly ChestPreviewRenderer chestPreviewRenderer;

    public PausePreviewService(
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

    public bool IsPausePreviewActive => Game1.activeClickableMenu is PausePreviewMenu;

    public bool Update()
    {
        if (Game1.activeClickableMenu is PausePreviewMenu)
        {
            if (!this.ShouldKeepPausePreviewOpen())
                Game1.exitActiveMenu();

            return true;
        }

        if (!this.ShouldOpenPausePreview())
            return false;

        Game1.activeClickableMenu = new PausePreviewMenu(this.config, this.chestHoverService, this.chestPreviewDataService, this.chestPreviewRenderer);
        return true;
    }

    private bool ShouldOpenPausePreview()
    {
        if (!Context.IsWorldReady)
            return false;

        if (!this.config.UseHotkeyPreview || !this.config.EnablePausePreview)
            return false;

        if (Context.IsMultiplayer)
            return false;

        if (Game1.activeClickableMenu is not null)
            return false;

        return this.config.PreviewKey.IsDown();
    }

    private bool ShouldKeepPausePreviewOpen()
    {
        return Context.IsWorldReady && this.config.UseHotkeyPreview && this.config.EnablePausePreview && this.config.PreviewKey.IsDown();
    }
}
