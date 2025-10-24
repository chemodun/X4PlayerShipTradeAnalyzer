using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using X4PlayerShipTradeAnalyzer.Models;
using X4PlayerShipTradeAnalyzer.Utils;

namespace X4PlayerShipTradeAnalyzer.ViewModels;

public abstract class StatsWaresShipsBaseModel : INotifyPropertyChanged
{
  public ObservableCollection<ISeries> Series { get; } = new();
  public ObservableCollection<LegendItem> Legend { get; } = new();
  public ObservableCollection<RectangularSection> Sections { get; } = new();

  public Axis[] XAxes { get; }
  public Axis[] YAxes { get; }

  private bool _showToolTip; // controls whether data labels (tool tips) are shown
  public bool ShowToolTip
  {
    get => _showToolTip;
    set
    {
      if (_showToolTip == value)
        return;
      _showToolTip = value;
      OnPropertyChanged();
    }
  }

  public ObservableCollection<GraphShipItem> WareShipsList { get; set; } = new();
  public string PressedWareName { get; set; } = string.Empty;
  public string PressedWareTotal
  {
    get
    {
      if (string.IsNullOrWhiteSpace(PressedWareName) || WareShipsList.Count == 0)
        return string.Empty;
      return WareShipsList.Sum(w => w.EstimatedProfit).ToString("N2");
    }
  }
  protected List<string> _labels = new();
  public IReadOnlyList<string> Labels => _labels;

#pragma warning disable CA1822
  public IEnumerable<TopNFilter> TopNOptions => Enum.GetValues(typeof(TopNFilter)).Cast<TopNFilter>();
#pragma warning restore CA1822

  private TopNFilter _topN = TopNFilter.Top25;
  public TopNFilter TopN
  {
    get => _topN;
    set
    {
      if (_topN == value)
        return;
      _topN = value;
      OnPropertyChanged();
      Reload();
    }
  }

  private bool _reverseSort; // default false

  public bool ReverseSort
  {
    get => _reverseSort;
    set
    {
      if (_reverseSort == value)
        return;
      _reverseSort = value;
      OnPropertyChanged();
      Reload();
    }
  }

  public double ChartMinWidth => Labels.Count * 56 + 200;

  // Ship class filtering
  private string _selectedShipClass = "All";
  public string SelectedShipClass
  {
    get => _selectedShipClass;
    set
    {
      if (_selectedShipClass == value)
        return;
      _selectedShipClass = value;
      OnPropertyChanged();
      Reload();
    }
  }

  // Expose stations as a property (binding requires property, not field)
  public List<StationShort> Stations { get; } = StationShort.StationList;
  private StationShort? _selectedStation = StationShort.StationList.FirstOrDefault();
  public StationShort? SelectedStation
  {
    get => _selectedStation;
    set
    {
      if (_selectedStation == value)
        return;
      _selectedStation = value;
      OnPropertyChanged();
      Reload();
    }
  }
#pragma warning disable CA1822
  public IEnumerable<string> ShipClassOptions => ShipClassFilterUtil.GetShipClassOptions();
#pragma warning restore CA1822

  protected StatsWaresShipsBaseModel(SKColor? foreground = null, SKColor? background = null)
  {
    XAxes = new[]
    {
      new Axis
      {
        Name = "Wares",
        NamePaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        LabelsPaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        SeparatorsPaint = new SolidColorPaint(background ?? SKColors.White) { StrokeThickness = 1 },
        LabelsRotation = 90,
      },
    };

    YAxes = new[]
    {
      new Axis
      {
        Name = "Total Profit",
        Labeler = v => v.ToString("N2"),
        NamePaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        LabelsPaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        SeparatorsPaint = new SolidColorPaint(background ?? SKColors.White) { StrokeThickness = 1 },
      },
    };

    Reload();
  }

  public void Refresh() => Reload();

  protected abstract List<(long ShipId, string ShipName, string WareId, string WareName, double Profit)> LoadData();

  protected void Reload()
  {
    var rows = LoadData();

    if (!Dispatcher.UIThread.CheckAccess())
    {
      var capturedRows = rows;
      Dispatcher.UIThread.Post(() => ApplyReload(capturedRows));
      return;
    }

    ApplyReload(rows);
  }

  private void ApplyReload(List<(long ShipId, string ShipName, string WareId, string WareName, double Profit)> rows)
  {
    // Group by ware
    var wareOrder = rows.GroupBy(r => (r.WareId, r.WareName))
      .Select(g => new
      {
        g.Key.WareId,
        g.Key.WareName,
        TotalProfit = g.Sum(x => x.Profit),
      })
      .ToList();

    // Sort by total profit
    var ordered = _reverseSort ? wareOrder.OrderBy(x => x.TotalProfit) : wareOrder.OrderByDescending(x => x.TotalProfit);

    // Apply TopN filter
    var topWares = ordered.Take((int)TopN).Select(x => x.WareName).ToList();

    // Filter rows to only those wares
    rows = rows.Where(r => topWares.Contains(r.WareName))
      .OrderBy(r => topWares.IndexOf(r.WareName))
      .ThenBy(r => r.ShipName, StringComparer.OrdinalIgnoreCase)
      .ToList();

    var wares = rows.Select(r => r.WareName).Distinct().ToList();
    var ships = rows.Select(r => (r.ShipId, r.ShipName)).Distinct().ToList();

    _labels = wares;

    Series.Clear();
    Legend.Clear();
    Sections.Clear();

    foreach (var (shipId, shipName) in ships)
    {
      var values = new List<double?>(wares.Count);

      foreach (var ware in wares)
      {
        var profit = rows.Where(r => r.ShipName == shipName && r.WareName == ware).Sum(r => r.Profit);

        values.Add(profit == 0 ? null : profit);
      }

      // Skip ship if it contributes nothing
      if (values.All(v => v is null))
        continue;

      var sk = GetColorForShip(shipId);

      var series = new StackedColumnSeries<double?>
      {
        Name = shipName,
        Values = values,
        Stroke = null,
        Fill = new SolidColorPaint(sk),
        MaxBarWidth = 48,

        XToolTipLabelFormatter = cp =>
        {
          var wareIdx = (int)Math.Round(cp.Coordinate.SecondaryValue);
          return (wareIdx >= 0 && wareIdx < wares.Count) ? wares[wareIdx] : string.Empty;
        },
        YToolTipLabelFormatter = cp => cp.Coordinate.PrimaryValue.ToString("N2"),
      };

      Series.Add(series);
      Legend.Add(new LegendItem(shipName, new SolidColorBrush(Color.FromArgb(sk.Alpha, sk.Red, sk.Green, sk.Blue))));
    }

    XAxes[0].Labels = wares;

    PressedWareName = string.Empty;
    WareShipsList.Clear();
    OnPropertyChanged(nameof(ChartMinWidth));
    OnPropertyChanged(nameof(Series));
    OnPropertyChanged(nameof(Legend));
    OnPropertyChanged(nameof(Labels));
    OnPropertyChanged(nameof(PressedWareName));
    OnPropertyChanged(nameof(PressedWareTotal));
    OnPropertyChanged(nameof(WareShipsList));
  }

  public void OnChartPointPressed(int shipIndex)
  {
    if (shipIndex < 0 || shipIndex >= Labels.Count)
      return;
    List<GraphShipItem> shipList = new();
    PressedWareName = Labels[shipIndex];
    Sections.Clear();
    Sections.Add(
      new RectangularSection
      {
        Xi = shipIndex - 0.5,
        Xj = shipIndex + 0.5,
        // Neutral grey highlight
        Fill = new SolidColorPaint(new SKColor(128, 128, 128, 60)),
        Stroke = new SolidColorPaint(new SKColor(96, 96, 96)) { StrokeThickness = 2 },
      }
    );
    for (int i = Series.Count - 1; i >= 0; i--)
    {
      var series = Series[i];
      if (
        series is StackedColumnSeries<double?> scs
        && scs.Name != null
        && scs.Values != null
        && scs.Values is IList<double?> valueList
        && shipIndex < valueList.Count
        && i < Legend.Count
      )
      {
        var val = scs.Values.ToList()[shipIndex];
        if (val.HasValue)
        {
          var originalBrush = Legend[i].Brush;
          if (originalBrush is ISolidColorBrush solidBrush)
          {
            var color = solidBrush.Color;

            // Boost alpha by 20%, capped at 255
            byte newAlpha = (byte)Math.Min(255, color.A * 1.2);

            var intensifiedColor = Color.FromArgb(255, color.R, color.G, color.B);

            shipList.Add(
              new GraphShipItem
              {
                ShipName = scs.Name,
                EstimatedProfit = (decimal)val.Value,
                GraphBrush = new SolidColorBrush(intensifiedColor),
              }
            );
          }
        }
      }
    }
    WareShipsList = new ObservableCollection<GraphShipItem>(shipList);
    OnPropertyChanged(nameof(PressedWareName));
    OnPropertyChanged(nameof(PressedWareTotal));
    OnPropertyChanged(nameof(WareShipsList));
  }

  protected static SKColor GetColorForShip(long shipId)
  {
    var baseColor = ChartPalette.PickForLong(shipId);
    return ChartPalette.WithAlpha(baseColor, 200);
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

  public sealed record LegendItem(string Name, ISolidColorBrush Brush);
}
