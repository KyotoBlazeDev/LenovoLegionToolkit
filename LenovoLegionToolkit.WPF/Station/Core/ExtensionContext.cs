using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LenovoLegionToolkit.Lib.Station.Core;
using LenovoLegionToolkit.Lib.Station.Logging;
using LenovoLegionToolkit.Lib.Station.Services;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Station.Core;

public sealed class ExtensionContext : IExtensionContext
{
    private static readonly string PluginsBasePath = Path.Combine(Folders.AppData, "Plugins", "Configs");

    private readonly string _pluginId;
    private Dictionary<string, JsonElement> _settings = [];
    private bool _settingsLoaded;

    public ExtensionContext(string pluginId, INavigationService navigation, IUiDispatcher uiDispatcher, IExtensionLogger logger)
    {
        _pluginId = pluginId;
        Navigation = navigation;
        UiDispatcher = uiDispatcher;
        Logger = logger;
    }

    public INavigationService Navigation { get; }
    public IUiDispatcher UiDispatcher { get; }
    public IExtensionLogger Logger { get; }

    public string GetPluginStoragePath(string pluginId)
    {
        var path = Path.Combine(PluginsBasePath, pluginId);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public bool TryGetSetting<T>(string key, out T value)
    {
        EnsureSettingsLoaded();

        if (_settings.TryGetValue(key, out var element))
        {
            try
            {
                var result = element.Deserialize<T>();
                if (result is not null)
                {
                    value = result;
                    return true;
                }
            }
            catch
            {

            }
        }

        value = default!;
        return false;
    }

    public bool TrySetSetting<T>(string key, T value)
    {
        EnsureSettingsLoaded();

        try
        {
            var element = JsonSerializer.SerializeToElement(value);
            _settings[key] = element;
            SaveSettings();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void EnsureSettingsLoaded()
    {
        if (_settingsLoaded)
            return;

        _settingsLoaded = true;

        var settingsFile = GetSettingsFilePath();
        if (!File.Exists(settingsFile))
            return;

        try
        {
            var json = File.ReadAllText(settingsFile);
            _settings = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];
        }
        catch
        {
            _settings = [];
        }
    }

    private void SaveSettings()
    {
        var settingsFile = GetSettingsFilePath();
        var dir = Path.GetDirectoryName(settingsFile)!;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(settingsFile, json);
    }

    private string GetSettingsFilePath()
    {
        return Path.Combine(PluginsBasePath, _pluginId, "plugin.json");
    }
}
