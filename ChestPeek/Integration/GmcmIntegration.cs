using StardewModdingAPI;

namespace ChestPeek.Integration;

/// <summary>Registers Chest Peek options with Generic Mod Config Menu when it is installed.</summary>
public static class GmcmIntegration
{
    public static bool Register(IModHelper helper, IManifest manifest, Func<ModConfig> getConfig, Action<ModConfig> setConfig)
    {
        IGenericModConfigMenuApi? gmcm = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (gmcm is null)
            return false;

        string Text(string key)
        {
            return helper.Translation.Get(key);
        }

        gmcm.Register(
            manifest,
            reset: () =>
            {
                ModConfig config = new();
                setConfig(config);
                helper.WriteConfig(config);
            },
            save: () => helper.WriteConfig(getConfig()));

        gmcm.AddBoolOption(
            manifest,
            getValue: () => getConfig().UseHotkeyPreview,
            setValue: value => getConfig().UseHotkeyPreview = value,
            name: () => Text("config.use_hotkey_preview.name"),
            tooltip: () => Text("config.use_hotkey_preview.tooltip"),
            fieldId: nameof(ModConfig.UseHotkeyPreview));

        gmcm.AddKeybindList(
            manifest,
            getValue: () => getConfig().PreviewKey,
            setValue: value => getConfig().PreviewKey = value,
            name: () => Text("config.preview_key.name"),
            tooltip: () => Text("config.preview_key.tooltip"),
            fieldId: nameof(ModConfig.PreviewKey));

        gmcm.AddBoolOption(
            manifest,
            getValue: () => getConfig().EnablePausePreview,
            setValue: value => getConfig().EnablePausePreview = value,
            name: () => Text("config.enable_pause_preview.name"),
            tooltip: () => Text("config.enable_pause_preview.tooltip"),
            fieldId: nameof(ModConfig.EnablePausePreview));

        return true;
    }
}
