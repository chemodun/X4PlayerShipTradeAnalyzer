using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Markup.Xaml.MarkupExtensions;
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

public abstract class StatsShipsWaresBaseModel : INotifyPropertyChanged
{
  public ObservableCollection<ISeries> Series { get; } = new();
  public ObservableCollection<LegendItem> Legend { get; } = new();

  // Sections allow us to highlight the last pressed stacked column (ship)
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

  public ObservableCollection<GraphWareItem> ShipWaresList { get; set; } = new();
  public string PressedShipName { get; set; } = string.Empty;
  public string PressedShipTotal
  {
    get
    {
      if (string.IsNullOrWhiteSpace(PressedShipName) || ShipWaresList.Count == 0)
        return string.Empty;
      return ShipWaresList.Sum(w => w.Money).ToString("N2");
    }
  }

  // Expose stations as a property (binding requires property, not field)
#pragma warning disable CA1822
  public ObservableCollection<StationShort> Stations => StationShort.StationList;
#pragma warning restore CA1822
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

  protected List<string> _labels = new();
  public IReadOnlyList<string> Labels => _labels;
#pragma warning disable CA1822 // Mark members as static
  public IEnumerable<TopNFilter> TopNOptions => Enum.GetValues(typeof(TopNFilter)).Cast<TopNFilter>();
#pragma warning restore CA1822 // Mark members as static
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

#pragma warning disable CA1822
  public IEnumerable<string> ShipClassOptions => ShipClassFilterUtil.GetShipClassOptions();
#pragma warning restore CA1822

  protected StatsShipsWaresBaseModel(SKColor? foreground = null, SKColor? background = null)
  {
    XAxes = new[]
    {
      new Axis
      {
        Name = "Ships",
        NamePaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        LabelsPaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        SeparatorsPaint = new SolidColorPaint(background ?? SKColors.White) { StrokeThickness = 1 },
        LabelsRotation = 90, // optional: rotate labels for readability
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
    // Select
    var shipOrder = rows.GroupBy(r => (r.ShipId, r.ShipName))
      .Select(g => new
      {
        g.Key.ShipId,
        g.Key.ShipName,
        TotalProfit = g.Sum(x => x.Profit),
      })
      .ToList();

    // Sort by total profit
    var ordered = _reverseSort ? shipOrder.OrderBy(x => x.TotalProfit) : shipOrder.OrderByDescending(x => x.TotalProfit);

    // Apply TopN filter
    var topShips = ordered.Take((int)TopN).Select(x => x.ShipName).ToList();

    // Filter rows to only those ships
    rows = rows.Where(r => topShips.Contains(r.ShipName))
      .OrderBy(r => topShips.IndexOf(r.ShipName))
      .ThenBy(r => r.WareName, StringComparer.OrdinalIgnoreCase)
      .ToList();
    // Distinct ship and ware lists
    var ships = rows.Select(r => r.ShipName).Distinct().ToList();
    var wares = rows.Select(r => (r.WareId, r.WareName)).Distinct().ToList();

    _labels = ships;

    Series.Clear();
    Legend.Clear();
    Sections.Clear();
    foreach (var (wareId, wareName) in wares)
    {
      var values = new List<double?>(ships.Count);

      foreach (var ship in ships)
      {
        var profit = rows.Where(r => r.ShipName == ship && r.WareId == wareId).Sum(r => r.Profit);

        values.Add(profit == 0 ? null : profit);
      }

      var sk = GetColorForWare(wareId);

      var series = new StackedColumnSeries<double?>
      {
        Name = wareName,
        Values = values,
        Stroke = null,
        Fill = new SolidColorPaint(sk),
        MaxBarWidth = 48,
      };

      Series.Add(series);
      Legend.Add(new LegendItem(wareName, new SolidColorBrush(Color.FromArgb(sk.Alpha, sk.Red, sk.Green, sk.Blue))));
    }

    // Update X axis labels to ship names
    XAxes[0].Labels = ships;

    PressedShipName = string.Empty;
    ShipWaresList.Clear();
    OnPropertyChanged(nameof(ChartMinWidth));
    OnPropertyChanged(nameof(Series));
    OnPropertyChanged(nameof(Legend));
    OnPropertyChanged(nameof(Labels));
    OnPropertyChanged(nameof(PressedShipName));
    OnPropertyChanged(nameof(PressedShipTotal));
    OnPropertyChanged(nameof(ShipWaresList));
  }

  public void OnChartPointPressed(int shipIndex)
  {
    if (shipIndex < 0 || shipIndex >= Labels.Count)
      return;
    // highlight selected bar via section spanning that X index
    Sections.Clear();
    Sections.Add(
      new RectangularSection
      {
        Xi = shipIndex - 0.5,
        Xj = shipIndex + 0.5,
        // Use a neutral grey highlight (semi-transparent fill, solid stroke)
        Fill = new SolidColorPaint(new SKColor(128, 128, 128, 60)),
        Stroke = new SolidColorPaint(new SKColor(96, 96, 96)) { StrokeThickness = 2 },
      }
    );
    List<GraphWareItem> wareList = new();
    PressedShipName = Labels[shipIndex];
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

            wareList.Add(
              new GraphWareItem
              {
                Name = scs.Name,
                Money = (decimal)val.Value,
                GraphBrush = new SolidColorBrush(intensifiedColor),
              }
            );
          }
        }
      }
    }
    ShipWaresList = new ObservableCollection<GraphWareItem>(wareList);
    OnPropertyChanged(nameof(PressedShipName));
    OnPropertyChanged(nameof(PressedShipTotal));
    OnPropertyChanged(nameof(ShipWaresList));
  }

  protected static SKColor GetColorForWare(string wareId)
  {
    var baseColor = ChartPalette.PickForString(wareId);
    return ChartPalette.WithAlpha(baseColor, 200);
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

  public sealed record LegendItem(string Name, ISolidColorBrush Brush);
}
