namespace ChestPeek;

/// <summary>Decides whether the preview should be active for the current input state.</summary>
public sealed class PreviewModeService
{
    private readonly ModConfig config;

    public PreviewModeService(ModConfig config)
    {
        this.config = config;
    }

    public bool CanPreview()
    {
        if (!this.config.UseHotkeyPreview)
            return true;

        // Hold-to-preview keeps the preview transient and avoids persistent UI state.
        return this.config.PreviewKey.IsDown();
    }
}
