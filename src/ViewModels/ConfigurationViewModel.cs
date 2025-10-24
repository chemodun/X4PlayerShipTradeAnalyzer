using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Threading;
using X4PlayerShipTradeAnalyzer.Services;
using X4PlayerShipTradeAnalyzer.Utils; // added for ResilientFileWatcher
using X4PlayerShipTradeAnalyzer.Views;

namespace X4PlayerShipTradeAnalyzer.ViewModels;

public sealed class ConfigurationViewModel : INotifyPropertyChanged
{
  private const string NotCheckedYetLabel = "(not checked yet)";
  private const string LastCheckFailedLabel = "(last check failed)";
  private const string LastCheckCancelledLabel = "(cancelled)";
  private const string MetadataMissingLabel = "(metadata missing)";
  private const string UnknownVersionLabel = "(unknown)";

  private readonly ConfigurationService _cfg = ConfigurationService.Instance;
  private ResilientFileWatcher? _saveWatcher; // watcher for auto reload
  private DateTime _lastAutoLoadUtc = DateTime.MinValue; // debounce auto loads
  private string? _lastLoadedFile; // track last loaded save file
  private readonly TimeSpan _autoLoadDebounce = TimeSpan.FromSeconds(2);
  private bool _isCheckingForUpdates;
  private readonly string _currentVersion;
  private string _lastCheckedRemoteVersion = NotCheckedYetLabel;
  private bool _checkForUpdatesOnStartup;
  private bool _enableFileLogging;
  private LogLevel _minimumLogLevel;
  private bool _includeAvaloniaLogs;

  private string? _gameFolderExePath;
  public string? GameFolderExePath
  {
    get => _gameFolderExePath;
    set
    {
      if (_gameFolderExePath == value)
        return;
      _gameFolderExePath = value;
      _cfg.GameFolderExePath = value;
      _cfg.Save();
      OnPropertyChanged();
      OnPropertyChanged(nameof(CanReloadGameData));
    }
  }

  private string? _GameSavePath;
  public string? GameSavePath
  {
    get => _GameSavePath;
    set
    {
      if (_GameSavePath == value)
        return;
      var oldFolder = string.IsNullOrWhiteSpace(_GameSavePath) ? null : Path.GetDirectoryName(_GameSavePath);
      _GameSavePath = value;
      _cfg.GameSavePath = value;
      _cfg.Save();
      OnPropertyChanged();
      OnPropertyChanged(nameof(CanReloadSaveData));
      if (oldFolder != Path.GetDirectoryName(_GameSavePath) && _autoReloadMode != AutoReloadGameSaveMode.None)
      {
        // Save folder changed; restart watcher if needed
        RestartSaveWatcherIfNeeded();
      }
    }
  }

  private bool _loadOnlyGameLanguage;
  public bool LoadOnlyGameLanguage
  {
    get => _loadOnlyGameLanguage;
    set
    {
      if (_loadOnlyGameLanguage == value)
        return;
      _loadOnlyGameLanguage = value;
      _cfg.LoadOnlyGameLanguage = value;
      _cfg.Save();
      OnPropertyChanged();
    }
  }

  private bool _loadRemovedObjects;
  public bool LoadRemovedObjects
  {
    get => _loadRemovedObjects;
    set
    {
      if (_loadRemovedObjects == value)
        return;
      _loadRemovedObjects = value;
      _cfg.LoadRemovedObjects = value;
      _cfg.Save();
      OnPropertyChanged();
      // Visual gating may depend on stats treated differently; update CanReloadSaveData
      OnPropertyChanged(nameof(CanReloadSaveData));
    }
  }

  private string _appTheme;
  public string AppTheme
  {
    get => _appTheme;
    set
    {
      if (_appTheme == value)
        return;
      _appTheme = value;
      _cfg.AppTheme = value;
      _cfg.Save();
      ApplyTheme(value);
      OnPropertyChanged();
    }
  }

  public Array LogLevels { get; } = Enum.GetValues(typeof(LogLevel));

  public bool EnableFileLogging
  {
    get => _enableFileLogging;
    set
    {
      if (_enableFileLogging == value)
        return;
      _enableFileLogging = value;
      _cfg.EnableFileLogging = value;
      _cfg.Save();
      UpdateLoggingConfiguration();
      OnPropertyChanged();
    }
  }

  public LogLevel MinimumLogLevel
  {
    get => _minimumLogLevel;
    set
    {
      if (_minimumLogLevel == value)
        return;
      _minimumLogLevel = value;
      _cfg.MinimumLogLevel = value;
      _cfg.Save();
      UpdateLoggingConfiguration();
      OnPropertyChanged();
    }
  }

  public bool IncludeAvaloniaLogs
  {
    get => _includeAvaloniaLogs;
    set
    {
      if (_includeAvaloniaLogs == value)
        return;
      _includeAvaloniaLogs = value;
      _cfg.IncludeAvaloniaLogs = value;
      _cfg.Save();
      UpdateLoggingConfiguration();
      OnPropertyChanged();
    }
  }

  public void RefreshStats()
  {
    TryUpdateStats(MainWindow.GameData);
  }

  public bool CanReloadGameData => !string.IsNullOrWhiteSpace(GameFolderExePath);

  // Enable only if save path is set AND base game data is loaded (key stats > 0)
  public bool CanReloadSaveData => !string.IsNullOrWhiteSpace(GameSavePath) && GameDataStatsReady;

  public bool CanCheckForUpdates => !_isCheckingForUpdates;

  public string CurrentVersion => _currentVersion;

  public string LastCheckedRemoteVersion
  {
    get => _lastCheckedRemoteVersion;
    private set
    {
      if (_lastCheckedRemoteVersion == value)
        return;
      _lastCheckedRemoteVersion = value;
      OnPropertyChanged();
    }
  }

  public bool CheckForUpdatesOnStartup
  {
    get => _checkForUpdatesOnStartup;
    set
    {
      if (_checkForUpdatesOnStartup == value)
        return;
      _checkForUpdatesOnStartup = value;
      _cfg.CheckForUpdatesOnStartup = value;
      _cfg.Save();
      OnPropertyChanged();
    }
  }

  private bool GameDataStatsReady =>
    WaresCount > 0
    && FactionsCount > 0
    && ClusterSectorNamesCount > 0
    && StoragesCount > 0
    && ShipStoragesCount > 0
    && ShipTypesCount > 0
    && LanguagesCount > 0
    && CurrentLanguageId > 0
    && CurrentLanguageTextCount > 0;

  private void SetIsCheckingForUpdates(bool value)
  {
    if (_isCheckingForUpdates == value)
      return;
    _isCheckingForUpdates = value;
    OnPropertyChanged(nameof(CanCheckForUpdates));
  }

  // Stats
  private int _waresCount;
  public int WaresCount
  {
    get => _waresCount;
    private set
    {
      if (_waresCount != value)
      {
        _waresCount = value;
        OnPropertyChanged();
      }
    }
  }
  private int _playerShipsCount;
  public int PlayerShipsCount
  {
    get => _playerShipsCount;
    private set
    {
      if (_playerShipsCount != value)
      {
        _playerShipsCount = value;
        OnPropertyChanged();
      }
    }
  }
  private int _stationsCount;
  public int StationsCount
  {
    get => _stationsCount;
    private set
    {
      if (_stationsCount != value)
      {
        _stationsCount = value;
        OnPropertyChanged();
      }
    }
  }
  private int _removedObjectCount;
  public int RemovedObjectCount
  {
    get => _removedObjectCount;
    private set
    {
      if (_removedObjectCount != value)
      {
        _removedObjectCount = value;
        OnPropertyChanged();
      }
    }
  }
  private int _tradesCount;
  public int TradesCount
  {
    get => _tradesCount;
    private set
    {
      if (_tradesCount != value)
      {
        _tradesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _factionsCount;
  public int FactionsCount
  {
    get => _factionsCount;
    private set
    {
      if (_factionsCount != value)
      {
        _factionsCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _gatesCount;
  public int GatesCount
  {
    get => _gatesCount;
    private set
    {
      if (_gatesCount != value)
      {
        _gatesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _subordinateCount;
  public int SubordinateCount
  {
    get => _subordinateCount;
    private set
    {
      if (_subordinateCount != value)
      {
        _subordinateCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _clusterSectorNamesCount;
  public int ClusterSectorNamesCount
  {
    get => _clusterSectorNamesCount;
    private set
    {
      if (_clusterSectorNamesCount != value)
      {
        _clusterSectorNamesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _languagesCount;
  public int LanguagesCount
  {
    get => _languagesCount;
    private set
    {
      if (_languagesCount != value)
      {
        _languagesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _storagesCount;
  public int StoragesCount
  {
    get => _storagesCount;
    private set
    {
      if (_storagesCount != value)
      {
        _storagesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _shipStoragesCount;
  public int ShipStoragesCount
  {
    get => _shipStoragesCount;
    private set
    {
      if (_shipStoragesCount != value)
      {
        _shipStoragesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _shipTypesCount;
  public int ShipTypesCount
  {
    get => _shipTypesCount;
    private set
    {
      if (_shipTypesCount != value)
      {
        _shipTypesCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _currentLanguageTextCount;
  public int CurrentLanguageTextCount
  {
    get => _currentLanguageTextCount;
    private set
    {
      if (_currentLanguageTextCount != value)
      {
        _currentLanguageTextCount = value;
        OnPropertyChanged();
      }
    }
  }

  private int _currentLanguageId;
  public int CurrentLanguageId
  {
    get => _currentLanguageId;
    private set
    {
      if (_currentLanguageId != value)
      {
        _currentLanguageId = value;
        OnPropertyChanged();
      }
    }
  }

  public ConfigurationViewModel(string version)
  {
    _currentVersion = version;
    GameFolderExePath = _cfg.GameFolderExePath;
    GameSavePath = _cfg.GameSavePath;
    LoadOnlyGameLanguage = _cfg.LoadOnlyGameLanguage;
    LoadRemovedObjects = _cfg.LoadRemovedObjects;
    _appTheme = _cfg.AppTheme;
    _autoReloadMode = _cfg.AutoReloadMode;
    _checkForUpdatesOnStartup = _cfg.CheckForUpdatesOnStartup;
    _enableFileLogging = _cfg.EnableFileLogging;
    _minimumLogLevel = _cfg.MinimumLogLevel;
    _includeAvaloniaLogs = _cfg.IncludeAvaloniaLogs;
    // apply saved theme on startup
    ApplyTheme(_appTheme);
    // Setup watcher if needed based on loaded config
    RestartSaveWatcherIfNeeded();
    OnPropertyChanged(nameof(EnableFileLogging));
    OnPropertyChanged(nameof(MinimumLogLevel));
    OnPropertyChanged(nameof(IncludeAvaloniaLogs));
  }

  private void UpdateLoggingConfiguration() =>
    LoggingService.ApplyConfiguration(_enableFileLogging, _minimumLogLevel, _includeAvaloniaLogs);

  // Helpers invoked from code-behind button clicks
  public void ReloadGameData(GameData gameData, Action<ProgressUpdate>? progress = null)
  {
    if (!CanReloadGameData || string.IsNullOrWhiteSpace(GameFolderExePath))
      return;
    try
    {
      LoggingService.Debug($"Reloading game data from: {GameFolderExePath}");
      gameData.LoadGameXmlFiles(progress);
      LoggingService.Debug("Clearing game save tables...");
      gameData.ClearTablesFromGameSave();
      LoggingService.Debug("Refreshing game data statistics after save import...");
      TryUpdateStats(gameData);
    }
    catch
    {
      // swallow for now (preliminary feature)
    }
  }

  public void ReloadSaveData(GameData gameData, Action<ProgressUpdate>? progress = null)
  {
    if (!CanReloadSaveData || string.IsNullOrWhiteSpace(GameSavePath))
      return;
    try
    {
      LoggingService.Debug($"Reloading save data from: {GameSavePath}");
      gameData.ImportSaveGame(progress);
      LoggingService.Debug("Reloaded save data successfully.");
      TryUpdateStats(gameData);
      LoggingService.Debug("Updated stats after reloading save data.");
    }
    catch
    {
      // swallow for now (preliminary feature)
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

  private void TryUpdateStats(GameData gameData)
  {
    try
    {
      WaresCount = gameData.Stats.WaresCount;
      PlayerShipsCount = gameData.Stats.PlayerShipsCount;
      StationsCount = gameData.Stats.StationsCount;
      RemovedObjectCount = gameData.Stats.RemovedObjectCount;
      TradesCount = gameData.Stats.TradesCount;
      GatesCount = gameData.Stats.GatesCount;
      SubordinateCount = gameData.Stats.SubordinateCount;
      FactionsCount = gameData.Stats.FactionsCount;
      ClusterSectorNamesCount = gameData.Stats.ClusterSectorNamesCount;
      StoragesCount = gameData.Stats.StoragesCount;
      ShipStoragesCount = gameData.Stats.ShipStoragesCount;
      ShipTypesCount = gameData.Stats.ShipTypesCount;
      LanguagesCount = gameData.Stats.LanguagesCount;
      CurrentLanguageTextCount = gameData.Stats.CurrentLanguageTextCount;
      CurrentLanguageId = gameData.Stats.CurrentLanguageId;
      // Stats affect CanReloadSaveData; notify binding to re-evaluate
      OnPropertyChanged(nameof(CanReloadSaveData));
    }
    catch
    { /* ignore */
    }
  }

  private static string ResolveCurrentVersion()
  {
    try
    {
      var assembly = Assembly.GetExecutingAssembly();
      if (assembly == null)
        return "unknown";

      var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
      if (!string.IsNullOrWhiteSpace(informational))
        return informational!;

      var version = assembly.GetName().Version;
      if (version != null)
      {
        var parts = new[] { version.Major, version.Minor, version.Build };
        var length = parts.Length;
        while (length > 2 && parts[length - 1] == 0)
          length--;
        return string.Join('.', parts.Take(length));
      }
    }
    catch
    {
      // ignored - fallback below
    }

    return "unknown";
  }

  private static void ApplyTheme(string theme)
  {
    // Valid: "System", "Light", "Dark" mapped to ThemeVariant
    var app = Application.Current;
    if (app is null)
      return;

    if (string.Equals(theme, "Light", StringComparison.OrdinalIgnoreCase))
    {
      app.RequestedThemeVariant = ThemeVariant.Light;
    }
    else if (string.Equals(theme, "Dark", StringComparison.OrdinalIgnoreCase))
    {
      app.RequestedThemeVariant = ThemeVariant.Dark;
    }
    else // System
    {
      app.RequestedThemeVariant = ThemeVariant.Default; // follow OS
    }

    // After switching theme, refresh README to ensure viewer picks up the new theme
    try
    {
      if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime life && life.MainWindow is MainWindow win)
      {
        // Ensure on UI thread
        Dispatcher.UIThread.Post(win.LoadReadme);
        Dispatcher.UIThread.Post(win.ApplyThemeOnCharts);
      }
    }
    catch { }
  }

  // Auto reload game save mode
  private AutoReloadGameSaveMode _autoReloadMode;
  public AutoReloadGameSaveMode AutoReloadMode
  {
    get => _autoReloadMode;
    set
    {
      if (_autoReloadMode == value)
        return;
      var oldMode = _autoReloadMode;
      _autoReloadMode = value;
      _cfg.AutoReloadMode = value;
      _cfg.Save();
      OnPropertyChanged();
      // update convenience properties
      OnPropertyChanged(nameof(AutoReloadNone));
      OnPropertyChanged(nameof(AutoReloadSelectedFile));
      OnPropertyChanged(nameof(AutoReloadAnyFile));
      // Restart watcher if needed based on new mode
      if ((oldMode == AutoReloadGameSaveMode.None || value == AutoReloadGameSaveMode.None) && _GameSavePath != null)
      {
        RestartSaveWatcherIfNeeded();
      }
    }
  }

  // Convenience boolean properties for RadioButtons binding
  public bool AutoReloadNone
  {
    get => AutoReloadMode == AutoReloadGameSaveMode.None;
    set
    {
      if (value)
        AutoReloadMode = AutoReloadGameSaveMode.None;
    }
  }
  public bool AutoReloadSelectedFile
  {
    get => AutoReloadMode == AutoReloadGameSaveMode.SelectedFile;
    set
    {
      if (value)
        AutoReloadMode = AutoReloadGameSaveMode.SelectedFile;
    }
  }
  public bool AutoReloadAnyFile
  {
    get => AutoReloadMode == AutoReloadGameSaveMode.AnyFile;
    set
    {
      if (value)
        AutoReloadMode = AutoReloadGameSaveMode.AnyFile;
    }
  }

  public async Task CheckForUpdatesAsync(Window owner, bool onStartup = false, CancellationToken cancellationToken = default)
  {
    if (_isCheckingForUpdates)
      return;

    SetIsCheckingForUpdates(true);
    try
    {
      var assembly = Assembly.GetExecutingAssembly();
      var metadata = assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .ToDictionary(attr => attr.Key, attr => attr.Value ?? string.Empty, StringComparer.OrdinalIgnoreCase);

      if (metadata.Count == 0)
      {
        LastCheckedRemoteVersion = MetadataMissingLabel;
        if (!onStartup)
          await DialogService.ShowErrorAsync(owner, "Update check", "No assembly metadata found.");
        return;
      }

      var repoUrl = metadata.TryGetValue("RepositoryUrl", out var repo) ? repo : string.Empty;
      var nexusUrl = metadata.TryGetValue("NexusModsUrl", out var nexus) ? nexus : string.Empty;

      if (string.IsNullOrWhiteSpace(repoUrl))
      {
        LastCheckedRemoteVersion = MetadataMissingLabel;
        if (!onStartup)
          await DialogService.ShowErrorAsync(owner, "Update check", "No related repository metadata found.");
        return;
      }

      using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      linkedCts.CancelAfter(TimeSpan.FromSeconds(20));

      if (onStartup)
      {
        try
        {
          await Task.Delay(2000, linkedCts.Token);
        }
        catch (TaskCanceledException)
        {
          // ignored - startup delay cancelled
        }
      }
      var result = await UpdateChecker.CheckForUpdatesAsync(repoUrl, assembly, linkedCts.Token);
      LastCheckedRemoteVersion = result.Success
        ? (!string.IsNullOrWhiteSpace(result.LatestVersion) ? result.LatestVersion! : UnknownVersionLabel)
        : LastCheckFailedLabel;
      if (!result.Success)
      {
        if (!onStartup)
          await DialogService.ShowErrorAsync(owner, "Update check", result.Message);
        return;
      }

      if (result.HasUpdate)
      {
        var message =
          $"A new version is available (current: {result.CurrentVersion}, latest: {result.LatestVersion}).\n\n"
          + "Open the Nexus Mods page now?";
        var open = await DialogService.ShowConfirmationAsync(owner, "Update available", message, "Open", "Later");
        if (open && !string.IsNullOrWhiteSpace(nexusUrl))
        {
          UpdateChecker.OpenUrlInBrowser(nexusUrl);
        }
      }
      else if (!onStartup)
      {
        var message = $"You are on the latest version ({result.CurrentVersion}).";
        await DialogService.ShowInfoAsync(owner, "Up to date", message);
      }
    }
    catch (OperationCanceledException)
    {
      LastCheckedRemoteVersion = LastCheckCancelledLabel;
      // ignored - cancellation is expected when shutting down
    }
    catch (Exception ex)
    {
      LastCheckedRemoteVersion = LastCheckFailedLabel;
      if (!onStartup)
        await DialogService.ShowErrorAsync(owner, "Update check failed", ex.Message);
    }
    finally
    {
      SetIsCheckingForUpdates(false);
    }
  }

  private void RestartSaveWatcherIfNeeded()
  {
    // Stop existing always first
    _saveWatcher?.Stop();
    _saveWatcher?.Dispose();
    _saveWatcher = null;

    if (_autoReloadMode == AutoReloadGameSaveMode.None)
      return; // nothing to watch

    if (string.IsNullOrWhiteSpace(GameSavePath))
      return; // no path

    try
    {
      var dir = Path.GetDirectoryName(GameSavePath);
      if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
        return;

      // watch *.xml.gz in save folder
      _saveWatcher = new ResilientFileWatcher(dir, "*.xml.gz", includeSubdirectories: false, debounceMilliseconds: 500);
      _saveWatcher.Renamed += OnSaveFileChanged;
      // _saveWatcher.Created += OnSaveFileChanged;
      // _saveWatcher.Changed += OnSaveFileChanged; // some systems modify in-place
      _saveWatcher.Start();
    }
    catch
    {
      // swallow - watcher optional
    }
  }

  private void OnSaveFileChanged(object? sender, FileSystemEventArgs e)
  {
    // Ensure we are allowed to reload and avoid rapid re-imports
    if (!CanReloadSaveData)
      return;

    // We only care about .xml.gz already by filter, but double-check
    if (!e.FullPath.EndsWith(".xml.gz", StringComparison.OrdinalIgnoreCase))
      return;

    var fileName = Path.GetFileName(e.FullPath);

    // Determine if this file should trigger reload
    bool shouldReload = false;

    if (_autoReloadMode == AutoReloadGameSaveMode.SelectedFile)
    {
      var target = Path.GetFileName(GameSavePath ?? string.Empty);
      if (!string.IsNullOrEmpty(target) && string.Equals(target, fileName, StringComparison.OrdinalIgnoreCase))
        shouldReload = true;
    }
    else if (_autoReloadMode == AutoReloadGameSaveMode.AnyFile)
    {
      // quicksave.xml.gz OR autosave_01..03.xml.gz OR save_001..010.xml.gz
      if (string.Equals(fileName, "quicksave.xml.gz", StringComparison.OrdinalIgnoreCase))
      {
        shouldReload = true;
      }
      else if (
        fileName.StartsWith("autosave_", StringComparison.OrdinalIgnoreCase)
        && fileName.EndsWith(".xml.gz", StringComparison.OrdinalIgnoreCase)
      )
      {
        // extract number between autosave_ and .xml.gz
        var middle = fileName.Substring(9, fileName.Length - 9 - 7); // autosave_ = 9 chars, .xml.gz = 7
        if (int.TryParse(middle, out var autoNum) && autoNum >= 1 && autoNum <= 3)
          shouldReload = true;
      }
      else if (
        fileName.StartsWith("save_", StringComparison.OrdinalIgnoreCase) && fileName.EndsWith(".xml.gz", StringComparison.OrdinalIgnoreCase)
      )
      {
        var middle = fileName.Substring(5, fileName.Length - 5 - 7); // save_ = 5
        if (int.TryParse(middle, out var saveNum) && saveNum >= 1 && saveNum <= 10)
          shouldReload = true;
      }

      if (shouldReload)
      {
        // Update GameSavePath if a different file triggered it
        if (!string.Equals(GameSavePath, e.FullPath, StringComparison.OrdinalIgnoreCase))
        {
          // Set without recursion causing duplicate watcher restart (watcher already will be recreated but safe)
          GameSavePath = e.FullPath; // this will persist config
        }
      }
    }

    if (!shouldReload)
      return;

    var now = DateTime.UtcNow;
    if (_lastLoadedFile == e.FullPath && (now - _lastAutoLoadUtc) < _autoLoadDebounce)
      return; // debounce same file rapid events

    _lastLoadedFile = e.FullPath;
    _lastAutoLoadUtc = now;

    // Perform reload on UI thread
    Dispatcher.UIThread.Post(() =>
    {
      try
      {
        if (!CanReloadSaveData)
          return; // re-check after dispatch delay
        // Call the MainWindow handler to reuse progress UI logic
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime life && life.MainWindow is MainWindow win)
        {
          // Simulate button click pathway (sender null, event args empty)
          var mi = typeof(MainWindow).GetMethod(
            "ReloadSaveData_Click",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
          );
          mi?.Invoke(win, new object?[] { null, new Avalonia.Interactivity.RoutedEventArgs() });
        }
        else
        {
          // Fallback: direct reload (should rarely execute)
          ReloadSaveData(MainWindow.GameData, null);
        }
      }
      catch { }
    });
  }
}
