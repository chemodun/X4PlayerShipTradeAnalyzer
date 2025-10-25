using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LiveChartsCore.SkiaSharpView.Avalonia;
using MarkdownViewer.Core.Controls;
using X4PlayerShipTradeAnalyzer.Services;
using X4PlayerShipTradeAnalyzer.ViewModels;

namespace X4PlayerShipTradeAnalyzer.Views;

public partial class MainWindow : Window
{
  private static GameData? _gameData;
  public static GameData GameData => _gameData ??= new GameData();
  private TabItem? _currentTab;
  private bool _didStartupStatsCheck;
  private TabItem? _byTransactionsTab;
  private TabItem? _transactionsTab;
  private TabItem? _transactionsGraphsTab;
  private TabItem? _transactionsStatsShipsWaresTab;
  private TabItem? _transactionsStatsWaresShipsTab;
  private TabItem? _transactionsStatsShipsLoadTab;
  private TabItem? _byTradesTab;
  private TabItem? _tradesTab;
  private TabItem? _tradesGraphsTab;
  private TabItem? _tradesStatsShipsWaresTab;
  private TabItem? _tradesStatsWaresShipsTab;
  private TabItem? _tradesStatsShipsLoadTab;
  private TabItem? _configurationTab;
  private TabItem? _readmeTab;

  public static string Version
  {
    get
    {
      try
      {
        var asm = Assembly.GetExecutingAssembly();
        var rawInfoVersion =
          asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? asm.GetName().Version?.ToString();
        var infoVersion = SanitizeVersion(rawInfoVersion);
        return infoVersion ?? "(unknown)";
      }
      catch
      {
        return "(unknown)";
      }
    }
  }

  public MainWindow()
  {
    InitializeComponent();
#if DEBUG
    this.AttachDevTools();
#endif

    DataContext = new MainViewModel();

    // React to configuration changes affecting tab enablement
    if (DataContext is MainViewModel vm)
    {
      vm.RegisterCharts(this.FindControl<CartesianChart>);
      if (vm.Configuration != null)
        vm.Configuration.PropertyChanged += Configuration_PropertyChanged;
    }

    // Set base title from assembly metadata (Product + Version)
    try
    {
      var asm = Assembly.GetExecutingAssembly();
      var product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
      if (!string.IsNullOrWhiteSpace(product))
      {
        Title = string.IsNullOrWhiteSpace(Version) ? product : $"{product} v{Version}";
      }
    }
    catch { }

    // Run a one-time check on first open to direct user to Configuration if stats look empty
    this.Opened += MainWindow_Opened;

    _currentTab = this.FindControl<TabItem>("ByTransactionsTab");
    _byTransactionsTab = this.FindControl<TabItem>("ByTransactionsTab");
    _transactionsTab = this.FindControl<TabItem>("TransactionsTab");
    _transactionsGraphsTab = this.FindControl<TabItem>("TransactionsGraphsTab");
    _transactionsStatsShipsWaresTab = this.FindControl<TabItem>("TransactionsStatsShipsWaresTab");
    _transactionsStatsShipsLoadTab = this.FindControl<TabItem>("TransactionsStatsShipsLoadTab");
    _transactionsStatsWaresShipsTab = this.FindControl<TabItem>("TransactionsStatsWaresShipsTab");
    _byTradesTab = this.FindControl<TabItem>("ByTradesTab");
    _tradesTab = this.FindControl<TabItem>("TradesTab");
    _tradesGraphsTab = this.FindControl<TabItem>("TradesGraphsTab");
    _tradesStatsShipsWaresTab = this.FindControl<TabItem>("TradesStatsShipsWaresTab");
    _tradesStatsShipsLoadTab = this.FindControl<TabItem>("TradesStatsShipsLoadTab");
    _tradesStatsWaresShipsTab = this.FindControl<TabItem>("TradesStatsWaresShipsTab");
    _configurationTab = this.FindControl<TabItem>("ConfigurationTab");
    _readmeTab = this.FindControl<TabItem>("ReadmeTab");
    this.Opened += (_, __) => LoadReadme();
    ApplyThemeOnCharts();
  }

  private void InitializeComponent()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public CartesianChart? FindChart(string name) => this.FindControl<CartesianChart>(name);

  public void ApplyThemeOnCharts()
  {
    if (DataContext is MainViewModel vm)
    {
      if (_configurationTab == null)
        return;
      vm.ApplyThemeOnCharts(
        _configurationTab.Foreground as Avalonia.Media.SolidColorBrush,
        _configurationTab.Background as Avalonia.Media.SolidColorBrush
      );
    }
  }

  private void MainWindow_Opened(object? sender, System.EventArgs e)
  {
    if (_didStartupStatsCheck)
      return;
    _didStartupStatsCheck = true;

    if (DataContext is not MainViewModel vm)
      return;

    try
    {
      GameData.RefreshStats();
    }
    catch { }
    vm.Configuration?.RefreshStats();

    bool anyZero = AnyStatsMissing();

    UpdateTabsEnabled();

    if (anyZero)
    {
      var tabs = this.FindControl<TabControl>("MainTabs");
      if (tabs != null && _configurationTab != null)
      {
        tabs.SelectedItem = _configurationTab;
      }
    }

    if (!vm.IsDataInitialized)
    {
      Dispatcher.UIThread.Post(() =>
      {
        EnsureInitialDataAsync(vm);
      });
    }

    if (vm.Configuration is { CheckForUpdatesOnStartup: true } cfg)
    {
      _ = Dispatcher.UIThread.InvokeAsync(() => cfg.CheckForUpdatesAsync(this, true));
    }
  }

  private static bool AnyStatsMissing()
  {
    var s = GameData.Stats;
    bool removedRequired = ConfigurationService.Instance.LoadRemovedObjects;
    return s.WaresCount == 0
      || s.FactionsCount == 0
      || s.ClusterSectorNamesCount == 0
      || s.PlayerShipsCount == 0
      || s.StationsCount == 0
      || s.GatesCount == 0
      || s.StoragesCount == 0
      || s.ShipStoragesCount == 0
      || s.ShipTypesCount == 0
      || (removedRequired && s.RemovedObjectCount == 0)
      || s.TradesCount == 0
      || s.LanguagesCount == 0
      || s.CurrentLanguageId == 0
      || s.CurrentLanguageTextCount == 0;
  }

  private void UpdateTabsEnabled()
  {
    bool dataReady = !AnyStatsMissing();
    // Keep Configuration always enabled; toggle other tabs
    if (_byTransactionsTab != null)
      _byTransactionsTab.IsEnabled = dataReady;
    if (_byTradesTab != null)
      _byTradesTab.IsEnabled = dataReady;
    if (_transactionsTab != null)
      _transactionsTab.IsEnabled = dataReady;
    if (_transactionsGraphsTab != null)
      _transactionsGraphsTab.IsEnabled = dataReady;
    if (_transactionsStatsShipsWaresTab != null)
      _transactionsStatsShipsWaresTab.IsEnabled = dataReady;
    if (_transactionsStatsShipsLoadTab != null)
      _transactionsStatsShipsLoadTab.IsEnabled = dataReady;
    if (_transactionsStatsWaresShipsTab != null)
      _transactionsStatsWaresShipsTab.IsEnabled = dataReady;
    if (_tradesGraphsTab != null)
      _tradesGraphsTab.IsEnabled = dataReady;
    if (_tradesStatsShipsWaresTab != null)
      _tradesStatsShipsWaresTab.IsEnabled = dataReady;
    if (_tradesStatsShipsLoadTab != null)
      _tradesStatsShipsLoadTab.IsEnabled = dataReady;
    if (_tradesStatsWaresShipsTab != null)
      _tradesStatsWaresShipsTab.IsEnabled = dataReady;
    if (_tradesTab != null)
      _tradesTab.IsEnabled = dataReady;
    if (_configurationTab != null)
      _configurationTab.IsEnabled = true;
    if (_readmeTab != null)
      _readmeTab.IsEnabled = true;
  }

  private void Configuration_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(ConfigurationViewModel.LoadRemovedObjects))
    {
      UpdateTabsEnabled();
    }
  }

  // Toggle a ship's series on double-click in Ships Graphs list
  private void ShipGraphsList_DoubleTapped(object? sender, RoutedEventArgs e)
  {
    if (sender is not ListBox lb)
      return;
    if (lb.DataContext is not ShipsGraphsBaseModel model)
      return;
    if (e is not TappedEventArgs tea)
      return;

    if (tea.Source is Control c)
    {
      var container = c as ListBoxItem ?? c.FindAncestorOfType<ListBoxItem>();
      if (container?.DataContext is Models.GraphShipItem ship)
        model.ToggleShip(ship);
    }
  }

  // Configuration: Set Game Folder (select X4.exe)
  private async void SetGameFolder_Click(object? sender, RoutedEventArgs e)
  {
    if (DataContext is not MainViewModel vm)
      return;

    var options = new FilePickerOpenOptions
    {
      Title = "Select X4.exe",
      AllowMultiple = false,
      FileTypeFilter = new[] { new FilePickerFileType("X4 Executable") { Patterns = new[] { "X4.exe", "X4" } } },
    };

    // If we have an already set Game Folder, set its folder as the initial location
    if (!string.IsNullOrWhiteSpace(vm.Configuration?.GameFolderExePath))
    {
      var currentFolder = new DirectoryInfo(vm.Configuration.GameFolderExePath);
      if (currentFolder.Exists)
      {
        options.SuggestedStartLocation = await this.StorageProvider.TryGetFolderFromPathAsync(currentFolder.FullName);
      }
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
      if (Directory.Exists(startFolder))
      {
        options.SuggestedStartLocation = await this.StorageProvider.TryGetFolderFromPathAsync(startFolder);
      }
    }
    var files = await this.StorageProvider.OpenFilePickerAsync(options);
    if (files.Count > 0)
    {
      var selectedFile = files[0];
      var folderPath = Path.GetDirectoryName(selectedFile.Path.LocalPath);
      if (!string.IsNullOrWhiteSpace(folderPath) && vm.Configuration != null)
      {
        vm.Configuration.GameFolderExePath = folderPath;
      }
    }
  }

  // Configuration: Set Save Folder (select xml.gz)
  private async void SetSaveFolder_Click(object? sender, RoutedEventArgs e)
  {
    if (DataContext is not MainViewModel vm)
      return;

    var options = new FilePickerOpenOptions
    {
      Title = "Select xml.gz",
      AllowMultiple = false,
      FileTypeFilter = new[] { new FilePickerFileType("X4 Save Game") { Patterns = new[] { "*.xml.gz" } } },
    };

    // If we have a saved path, set its folder as the initial location
    if (!string.IsNullOrWhiteSpace(vm.Configuration?.GameSavePath))
    {
      var currentFile = new FileInfo(vm.Configuration.GameSavePath);
      if (currentFile.Exists || currentFile.Directory?.Exists == true)
      {
        options.SuggestedStartLocation = await this.StorageProvider.TryGetFolderFromPathAsync(currentFile.Directory!.FullName);
      }
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      if (Directory.Exists(startFolder))
      {
        if (Directory.Exists(Path.Combine(startFolder, "Egosoft")))
        {
          startFolder = Path.Combine(startFolder, "Egosoft");
          if (Directory.Exists(Path.Combine(startFolder, "X4")))
          {
            startFolder = Path.Combine(startFolder, "X4");
            options.SuggestedStartLocation = await this.StorageProvider.TryGetFolderFromPathAsync(startFolder);
          }
        }
      }
    }

    var files = await this.StorageProvider.OpenFilePickerAsync(options);
    if (files.Count > 0)
    {
      var selectedFile = files[0];
      if (!string.IsNullOrWhiteSpace(selectedFile.Path.LocalPath) && vm.Configuration != null)
      {
        vm.Configuration.GameSavePath = selectedFile.Path.LocalPath;
      }
    }
  }

  // Configuration: Reload Game Data (wares.xml)
  private async void ReloadGameData_Click(object? sender, RoutedEventArgs e)
  {
    if (DataContext is not MainViewModel vm)
      return;
    var progress = new ProgressWindow(Foreground as Avalonia.Media.SolidColorBrush, Background as Avalonia.Media.SolidColorBrush)
    {
      Title = "Loading game data...",
      CanResize = false,
    };
    progress.SetMessage("Loading wares and base data... This may take a minute.");
    progress.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    progress.Show(this);
    try
    {
      this.IsEnabled = false;
      LoggingService.Debug("Starting reload of game data...");
      progress.ApplyMode(ProgressWindow.ProgressMode.GameData);
      LoggingService.Debug("Calling ReloadGameData...");
      await Task.Run(() => vm.Configuration?.ReloadGameData(GameData, u => progress.SetProgress(u)));
      LoggingService.Debug("Refreshing game data statistics after save import...");
      GameData.RefreshStats();
      LoggingService.Debug("Updating tab enablement...");
      UpdateTabsEnabled();
      LoggingService.Debug("Reload game data complete.");
    }
    finally
    {
      await Dispatcher.UIThread.InvokeAsync(() =>
      {
        progress.Close();
        this.IsEnabled = true;
      });
    }
  }

  // Configuration: Reload Save Data (quicksave)
  private async void ReloadSaveData_Click(object? sender, RoutedEventArgs e)
  {
    if (DataContext is not MainViewModel vm)
      return;
    var progress = new ProgressWindow(Foreground as Avalonia.Media.SolidColorBrush, Background as Avalonia.Media.SolidColorBrush)
    {
      Title = "Loading save data...",
      CanResize = false,
    };
    progress.SetMessage("Importing savegame, please wait...");
    progress.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    progress.Show(this);
    try
    {
      this.IsEnabled = false;
      LoggingService.Debug("Starting reload of save data...");
      progress.ApplyMode(ProgressWindow.ProgressMode.SaveData);
      LoggingService.Debug("Calling ReloadSaveData...");

      await Task.Run(() =>
      {
        vm.Configuration?.ReloadSaveData(GameData, u => progress.SetProgress(u));
      });

      LoggingService.Debug("Refreshing view models after save import...");
      await Task.Run(() => vm.Refresh(u => progress.SetProgress(u)));

      LoggingService.Debug("Refreshing game data statistics after save import...");
      GameData.RefreshStats();

      LoggingService.Debug("Updating tab enablement after save import...");
      UpdateTabsEnabled();

      LoggingService.Debug("Reload save data complete.");
    }
    finally
    {
      await Dispatcher.UIThread.InvokeAsync(() =>
      {
        progress.Close();
        this.IsEnabled = true;
      });
    }
  }

  private async void EnsureInitialDataAsync(MainViewModel vm)
  {
    var progress = new ProgressWindow(Foreground as Avalonia.Media.SolidColorBrush, Background as Avalonia.Media.SolidColorBrush)
    {
      Title = "Preparing data...",
      CanResize = false,
    };
    progress.SetMessage("Preparing data for charts, please wait...");
    progress.ApplyMode(ProgressWindow.ProgressMode.OnStartup);
    progress.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    progress.Show(this);
    this.IsEnabled = false;

    try
    {
      await Task.Run(() => vm.Refresh(u => progress.SetProgress(u)));
      progress.SetProgress(new ProgressUpdate { Status = "Refreshing statistics..." });
      GameData.RefreshStats();
      progress.SetProgress(new ProgressUpdate { Status = "Updating interface..." });
      UpdateTabsEnabled();
      progress.SetProgress(new ProgressUpdate { Status = "Data preparation complete." });
    }
    finally
    {
      await Dispatcher.UIThread.InvokeAsync(() =>
      {
        progress.Close();
        this.IsEnabled = true;
      });
    }
  }

  private async void CheckForUpdates_Click(object? sender, RoutedEventArgs e)
  {
    if (DataContext is not MainViewModel vm || vm.Configuration is null)
      return;

    await vm.Configuration.CheckForUpdatesAsync(this);
  }

  // React on active tab change
  private void MainTabs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
  {
    if (DataContext is not MainViewModel vm)
      return;
    if (sender is not TabControl tc)
      return;
    if (tc.SelectedItem is not TabItem tab)
      return;

    if (_currentTab == tab)
      return;
    _currentTab = tab;
    var header = tab.Header?.ToString() ?? string.Empty;
    var name = tab.Name ?? string.Empty;

    // Keep base title (Product + Version) and append current tab
    string baseTitle = Title ?? "";
    int idx = baseTitle.IndexOf(" — ");
    if (idx >= 0)
      baseTitle = baseTitle.Substring(0, idx);
    Title = string.IsNullOrEmpty(header) ? baseTitle : $"{baseTitle} — {header}";

    switch (name)
    {
      case "ShipsTransactionsTab":
        // do not refresh
        break;
      case "ShipsTransactionsGraphsTab":
        // do not refresh
        break;
      case "ShipsTransactionsWaresStatsTab":
        // do not refresh
        break;
      case "ShipsTradesTab":
        // do not refresh
        break;
      case "ShipsTradesGraphsTab":
        // do not refresh
        break;
      case "ConfigurationTab":
        vm?.Configuration?.RefreshStats();
        break;
      case "ReadmeTab":
        // ensure content is present
        LoadReadme();
        break;
    }
  }

  public void LoadReadme()
  {
    try
    {
      // Remove any existing viewer
      var scroll = this.FindControl<ScrollViewer>("ReadmeScrollViewer");
      if (scroll == null)
        return; // UI not ready yet

      var exeDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
      var readmePath = Path.Combine(exeDir, "README.md");

      if (File.Exists(readmePath))
      {
        // Create a new MarkdownViewer instance
        var viewer = new MarkdownViewer.Core.Controls.MarkdownViewer { MarkdownText = File.ReadAllText(readmePath), IsEnabled = true };

        // Insert into the ScrollViewer
        scroll.Content = viewer;
      }
    }
    catch { }
  }

  private static string? SanitizeVersion(string? version)
  {
    if (string.IsNullOrWhiteSpace(version))
      return version;

    // Remove build metadata after '+' and any parenthetical suffix like " (abcdefg)"
    var v = version;
    var plus = v.IndexOf('+');
    if (plus >= 0)
      v = v.Substring(0, plus);
    var paren = v.IndexOf('(');
    if (paren > 0)
      v = v.Substring(0, paren).Trim();

    // Extract a SemVer-like pattern, optionally with a prerelease (but not build metadata)
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    var m = Regex.Match(v, "\\d+\\.\\d+(?:\\.\\d+)?(?:-[0-9A-Za-z.-]+)?");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    return m.Success ? m.Value : v;
  }
}
