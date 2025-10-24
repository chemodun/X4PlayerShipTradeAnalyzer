using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using X4PlayerShipTradeAnalyzer.Models;
using X4PlayerShipTradeAnalyzer.Services;
using X4PlayerShipTradeAnalyzer.Views;

namespace X4PlayerShipTradeAnalyzer.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
  public static List<Transaction> AllTransactions = new();
  public static List<FullTrade> AllTrades = new();
  public ObservableCollection<Ware> Wares { get; } = new();

  private ShipsDataTransactionsModel? _transactionsData;
  public ShipsDataTransactionsModel? TransactionsData
  {
    get => _transactionsData;
    set
    {
      _transactionsData = value;
      OnPropertyChanged();
    }
  }

  private ShipsGraphTransactionsModel? _transactionsGraphs;
  public ShipsGraphTransactionsModel? TransactionsGraphs
  {
    get => _transactionsGraphs;
    set
    {
      _transactionsGraphs = value;
      OnPropertyChanged();
    }
  }

  private StatsShipsWaresTransactionsModel? _transactionsShipsWaresStats;
  public StatsShipsWaresTransactionsModel? TransactionsStatsShipsWares
  {
    get => _transactionsShipsWaresStats;
    set
    {
      _transactionsShipsWaresStats = value;
      OnPropertyChanged();
    }
  }

  private StatsShipsLoadTransactionsModel? _transactionsShipsLoadStats;
  public StatsShipsLoadTransactionsModel? TransactionsStatsShipsLoad
  {
    get => _transactionsShipsLoadStats;
    set
    {
      _transactionsShipsLoadStats = value;
      OnPropertyChanged();
    }
  }

  private StatsWaresShipsTransactionsModel? _transactionsWaresShipsStats;
  public StatsWaresShipsTransactionsModel? TransactionsStatsWaresShips
  {
    get => _transactionsWaresShipsStats;
    set
    {
      _transactionsWaresShipsStats = value;
      OnPropertyChanged();
    }
  }

  private ShipsDataTradesModel? _tradesData;
  public ShipsDataTradesModel? TradesData
  {
    get => _tradesData;
    set
    {
      _tradesData = value;
      OnPropertyChanged();
    }
  }

  private ShipsGraphTradesModel? _tradesGraphs;
  public ShipsGraphTradesModel? TradesGraphs
  {
    get => _tradesGraphs;
    set
    {
      _tradesGraphs = value;
      OnPropertyChanged();
    }
  }

  private StatsShipsWaresTradesModel? _tradesShipsWaresStats;
  public StatsShipsWaresTradesModel? TradesStatsShipsWares
  {
    get => _tradesShipsWaresStats;
    set
    {
      _tradesShipsWaresStats = value;
      OnPropertyChanged();
    }
  }

  private StatsShipsLoadTradesModel? _tradesShipsLoadStats;
  public StatsShipsLoadTradesModel? TradesStatsShipsLoad
  {
    get => _tradesShipsLoadStats;
    set
    {
      _tradesShipsLoadStats = value;
      OnPropertyChanged();
    }
  }

  private StatsWaresShipsTradesModel? _tradesWaresShipsStats;
  public StatsWaresShipsTradesModel? TradesStatsWaresShips
  {
    get => _tradesWaresShipsStats;
    set
    {
      _tradesWaresShipsStats = value;
      OnPropertyChanged();
    }
  }

  private ConfigurationViewModel? _configuration;
  public ConfigurationViewModel? Configuration
  {
    get => _configuration;
    set
    {
      _configuration = value;
      OnPropertyChanged();
    }
  }

  public MainViewModel()
  {
    LoggingService.Debug($"Connection state: {MainWindow.GameData.Connection.State}");
    LoadData();
    TransactionsData = new ShipsDataTransactionsModel();
    TransactionsGraphs = new ShipsGraphTransactionsModel();
    TransactionsStatsShipsLoad = new StatsShipsLoadTransactionsModel();
    TransactionsStatsShipsWares = new StatsShipsWaresTransactionsModel();
    TransactionsStatsWaresShips = new StatsWaresShipsTransactionsModel();
    TradesData = new ShipsDataTradesModel();
    TradesGraphs = new ShipsGraphTradesModel();
    TradesStatsShipsLoad = new StatsShipsLoadTradesModel();
    TradesStatsShipsWares = new StatsShipsWaresTradesModel();
    TradesStatsWaresShips = new StatsWaresShipsTradesModel();
    Configuration = new ConfigurationViewModel(MainWindow.Version);
  }

  public void ApplyThemeOnCharts(Avalonia.Media.SolidColorBrush? foreground, Avalonia.Media.SolidColorBrush? background)
  {
    var colorForeground = SKColors.Black; // default
    if (foreground is Avalonia.Media.SolidColorBrush scbForeground)
      colorForeground = new SKColor(scbForeground.Color.R, scbForeground.Color.G, scbForeground.Color.B, scbForeground.Color.A);
    var colorBackground = SKColors.White; // default
    if (background is Avalonia.Media.SolidColorBrush scbBackground)
      colorBackground = new SKColor(scbBackground.Color.R, scbBackground.Color.G, scbBackground.Color.B, scbBackground.Color.A);
    TransactionsGraphs = new ShipsGraphTransactionsModel(colorForeground, colorBackground);
    TransactionsStatsShipsLoad = new StatsShipsLoadTransactionsModel(colorForeground, colorBackground);
    TransactionsStatsShipsWares = new StatsShipsWaresTransactionsModel(colorForeground, colorBackground);
    TransactionsStatsWaresShips = new StatsWaresShipsTransactionsModel(colorForeground, colorBackground);
    TradesGraphs = new ShipsGraphTradesModel(colorForeground, colorBackground);
    TradesStatsShipsLoad = new StatsShipsLoadTradesModel(colorForeground, colorBackground);
    TradesStatsShipsWares = new StatsShipsWaresTradesModel(colorForeground, colorBackground);
    TradesStatsWaresShips = new StatsWaresShipsTradesModel(colorForeground, colorBackground);
  }

  public static void LoadData(Action<ProgressUpdate>? progress = null)
  {
    LoggingService.Debug("Loading data into MainViewModel...");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Loaded Subordinates..." });
    Subordinate.LoadAllSubordinates();
    LoggingService.Debug("Loaded all subordinates.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing stations..." });
    StationShort.RefreshStationsWithTradeOrMiningSubordinates();
    LoggingService.Debug("Refreshed stations with trade or mining subordinates.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Loaded Transactions..." });
    Transaction.GetAllTransactions(ref AllTransactions);
    LoggingService.Debug("Loaded all transactions.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Loaded Full Trades..." });
    FullTrade.GetFullTrades(ref AllTrades, AllTransactions);
    LoggingService.Debug("Loaded all full trades.");
  }

  public void RegisterCharts(Func<string, CartesianChart?> getChart)
  {
    var chart = getChart("TransactionsShipsByWaresChart");
    if (chart != null)
      chart.PointerPressed += OnChartPointerPressed;
    chart = getChart("TransactionsShipsLoadChart");
    if (chart != null)
      chart.PointerPressed += OnChartPointerPressed;
    chart = getChart("TransactionsWaresByShipsChart");
    if (chart != null)
      chart.PointerPressed += OnChartPointerPressed;
    chart = getChart("TradesShipsByWaresChart");
    if (chart != null)
      chart.PointerPressed += OnChartPointerPressed;
    chart = getChart("TradesShipsLoadChart");
    if (chart != null)
      chart.PointerPressed += OnChartPointerPressed;
    chart = getChart("TradesWaresByShipsChart");
    if (chart != null)
      chart.PointerPressed += OnChartPointerPressed;
  }

  private void OnChartPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
  {
    if (sender is not CartesianChart chart)
      return;

    // Get Avalonia point
    var avaloniaPoint = e.GetPosition(chart);

    // Convert to LvcPointD
    var lvcPoint = new LvcPointD(avaloniaPoint.X, avaloniaPoint.Y);

    // Pass to ScalePixelsToData
    var data = chart.ScalePixelsToData(lvcPoint);

    var itemIndex = (int)Math.Round(data.X);
    switch (chart.Name)
    {
      case "TransactionsShipsByWaresChart":
        if (TransactionsStatsShipsWares != null)
          TransactionsStatsShipsWares.OnChartPointPressed(itemIndex);
        break;
      case "TransactionsShipsLoadChart":
        if (TransactionsStatsShipsLoad != null)
          TransactionsStatsShipsLoad?.OnChartPointPressed(itemIndex);
        break;
      case "TransactionsWaresByShipsChart":
        if (TransactionsStatsWaresShips != null)
          TransactionsStatsWaresShips.OnChartPointPressed(itemIndex);
        break;
      case "TradesShipsByWaresChart":
        if (TradesStatsShipsWares != null)
          TradesStatsShipsWares.OnChartPointPressed(itemIndex);
        break;
      case "TradesShipsLoadChart":
        if (TradesStatsShipsLoad != null)
          TradesStatsShipsLoad?.OnChartPointPressed(itemIndex);
        break;
      case "TradesWaresByShipsChart":
        if (TradesStatsWaresShips != null)
          TradesStatsWaresShips.OnChartPointPressed(itemIndex);
        break;
    }
  }

  public void Refresh(Action<ProgressUpdate>? progress = null)
  {
    // Reload data for all models
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Loaded data..." });
    LoadData(progress);
    LoggingService.Debug("Reloaded save data successfully.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Transactions Data..." });
    TransactionsData?.Refresh();
    LoggingService.Debug("Refreshed TransactionsData.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Transactions Graphs..." });
    TransactionsGraphs?.Refresh();
    LoggingService.Debug("Refreshed TransactionsGraphs.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Transactions Stats Ships Load..." });
    TransactionsStatsShipsLoad?.Refresh();
    LoggingService.Debug("Refreshed TransactionsStatsShipsLoad.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Transactions Stats Ships Wares..." });
    TransactionsStatsShipsWares?.Refresh();
    LoggingService.Debug("Refreshed TransactionsStatsShipsWares.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Transactions Stats Wares Ships..." });
    TransactionsStatsWaresShips?.Refresh();
    LoggingService.Debug("Refreshed TransactionsStatsWaresShips.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Trades Data..." });
    TradesData?.Refresh();
    LoggingService.Debug("Refreshed TradesData.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Trades Graphs..." });
    TradesGraphs?.Refresh();
    LoggingService.Debug("Refreshed TradesGraphs.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Trades Stats Ships Load..." });
    TradesStatsShipsLoad?.Refresh();
    LoggingService.Debug("Refreshed TradesStatsShipsLoad.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Trades Stats Ships Wares..." });
    TradesStatsShipsWares?.Refresh();
    LoggingService.Debug("Refreshed TradesStatsShipsWares.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Trades Stats Wares Ships..." });
    TradesStatsWaresShips?.Refresh();
    LoggingService.Debug("Refreshed TradesStatsWaresShips.");
    progress?.Invoke(new ProgressUpdate { Status = "Refreshing Configuration..." });
    Configuration?.RefreshStats();
    LoggingService.Debug("Refreshed Configuration.");
  }

  // minimal INotifyPropertyChanged
  public event PropertyChangedEventHandler? PropertyChanged;

  private void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public enum TopNFilter
{
  Top10 = 10,
  Top25 = 25,
  Top50 = 50,
  Top100 = 100,
}
