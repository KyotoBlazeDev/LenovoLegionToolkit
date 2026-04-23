using LenovoLegionToolkit.Lib.Station.Core;
using LenovoLegionToolkit.Lib.Station.Logging;
using LenovoLegionToolkit.Lib.Station.Services;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Station.Core;

public sealed class ExtensionContext : IExtensionContext
{
    private static readonly string PluginsBasePath = System.IO.Path.Combine(Folders.AppData, "PluginConfigs");

    public ExtensionContext(INavigationService navigation, IUiDispatcher uiDispatcher, IExtensionLogger logger)
    {
        Navigation = navigation;
        UiDispatcher = uiDispatcher;
        Logger = logger;
    }

    public INavigationService Navigation { get; }
    public IUiDispatcher UiDispatcher { get; }
    public IExtensionLogger Logger { get; }

    public string GetPluginStoragePath(string pluginId)
    {
        var path = System.IO.Path.Combine(PluginsBasePath, pluginId);
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
        return path;
    }

    public bool TryGetSetting<T>(string key, out T value)
    {
        value = default!;
        return false;
    }

    public bool TrySetSetting<T>(string key, T value)
    {
        return false;
    }
}
