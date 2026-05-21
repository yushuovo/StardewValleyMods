using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using ChestPeek.Integration;
using ChestPeek.UI;

namespace ChestPeek;

/// <summary>Main SMAPI entry point for Chest Peek.</summary>
public sealed class ModEntry : Mod
{
    private ModConfig config = null!;
    private PreviewModeService previewModeService = null!;
    private PausePreviewService pausePreviewService = null!;
    private ChestHoverService chestHoverService = null!;
    private ChestPreviewDataService chestPreviewDataService = null!;
    private ChestPreviewRenderer chestPreviewRenderer = null!;
    private ChestPreviewData? currentPreviewData;
    private string? lastHoverKey;

    public override void Entry(IModHelper helper)
    {
        this.chestPreviewRenderer = new ChestPreviewRenderer();
        this.chestHoverService = new ChestHoverService(helper);
        this.chestPreviewDataService = new ChestPreviewDataService();
        this.ApplyConfig(helper.ReadConfig<ModConfig>());
        helper.WriteConfig(this.config);

        // UpdateTicked tracks which chest is currently hovered; RenderedHud draws the latest snapshot.
        helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Display.RenderedHud += this.OnRenderedHud;
        helper.ConsoleCommands.Add(
            "chestpeek_reload_config",
            helper.Translation.Get("console.reload_config.description"),
            this.OnReloadConfigCommand);

        this.Monitor.Log("Chest Peek loaded.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        bool registered = GmcmIntegration.Register(
            this.Helper,
            this.ModManifest,
            () => this.config,
            this.ApplyConfig);

        this.Monitor.Log(
            registered
                ? "Generic Mod Config Menu integration registered."
                : "Generic Mod Config Menu not installed; using config.json only.",
            LogLevel.Trace);
    }

    private void OnReloadConfigCommand(string command, string[] args)
    {
        this.ApplyConfig(this.Helper.ReadConfig<ModConfig>());
        this.Helper.WriteConfig(this.config);
        this.currentPreviewData = null;

        this.Monitor.Log("Chest Peek config reloaded from config.json.", LogLevel.Info);
    }

    private void ApplyConfig(ModConfig config)
    {
        this.config = config;
        this.previewModeService = new PreviewModeService(this.config);
        this.pausePreviewService = new PausePreviewService(this.config, this.chestHoverService, this.chestPreviewDataService, this.chestPreviewRenderer);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            this.currentPreviewData = null;
            return;
        }

        if (this.pausePreviewService.Update())
        {
            this.currentPreviewData = null;
            return;
        }

        if (!this.previewModeService.CanPreview())
        {
            this.currentPreviewData = null;
            return;
        }

        ChestHoverResult? hoverResult = this.chestHoverService.GetHoveredChest();
        if (hoverResult is null)
        {
            this.currentPreviewData = null;
            return;
        }

        string hoverKey = $"{Game1.currentLocation.Name}:{hoverResult.Tile.X},{hoverResult.Tile.Y}";
        bool changedTile = hoverKey != this.lastHoverKey;

        bool hasChest = hoverResult.Chest is not null;
        if (changedTile)
        {
            this.lastHoverKey = hoverKey;
            // Trace-level tile logging is useful while tuning hover detection without flooding normal logs.
            this.Monitor.Log($"Hover tile: {hoverResult.Tile.X:0}, {hoverResult.Tile.Y:0} | Chest: {(hasChest ? "yes" : "no")}", LogLevel.Trace);
        }

        if (hoverResult.Chest is null)
        {
            this.currentPreviewData = null;
            return;
        }

        this.currentPreviewData = this.chestPreviewDataService.GetPreviewData(hoverResult.Chest);
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (!Context.IsWorldReady || this.currentPreviewData is null || this.pausePreviewService.IsPausePreviewActive)
            return;

        // Drawing happens in the HUD event so the preview appears above the world and normal UI layer.
        this.chestPreviewRenderer.Draw(e.SpriteBatch, this.currentPreviewData, this.config);
    }
}
