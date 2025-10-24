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

namespace X4PlayerShipTradeAnalyzer.ViewModels;

public abstract class StatsShipsLoadBaseModel : INotifyPropertyChanged
{
  public ObservableCollection<ISeries> Series { get; } = new();
  public ObservableCollection<LegendItem> Legend { get; } = new();
  public ObservableCollection<RectangularSection> Sections { get; } = new();
  public ObservableCollection<GraphBucketItem> ShipBucketsList { get; set; } = new();
  public string PressedShipName { get; set; } = string.Empty;
  public string PressedShipTotal
  {
    get
    {
      if (string.IsNullOrWhiteSpace(PressedShipName) || ShipBucketsList.Count == 0)
        return string.Empty;
      return ShipBucketsList.Sum(b => b.Percent).ToString("N2") + "%";
    }
  }

  public Axis[] XAxes { get; }
  public Axis[] YAxes { get; }

  private bool _showToolTip = true;
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

  public double ChartMinWidth => _shipLabels.Count * 56 + 200;

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

  protected List<string> _shipLabels = new();
  public IReadOnlyList<string> Labels => _shipLabels;

  protected StatsShipsLoadBaseModel(SKColor? foreground = null, SKColor? background = null)
  {
    XAxes = new[]
    {
      new Axis
      {
        Name = "Ships",
        LabelsRotation = 90,
        NamePaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        LabelsPaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        SeparatorsPaint = new SolidColorPaint(background ?? SKColors.White) { StrokeThickness = 1 },
      },
    };
    YAxes = new[]
    {
      new Axis
      {
        Name = "Cargo Load Utilization Distribution Across Entries",
        Labeler = v => v.ToString("N2"),
        MinLimit = 0,
        MaxLimit = 100,
        NamePaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        LabelsPaint = new SolidColorPaint(foreground ?? SKColors.Black), // or any color you want
        SeparatorsPaint = new SolidColorPaint(background ?? SKColors.White) { StrokeThickness = 1 },
      },
    };
    Reload();
  }

  public void Refresh() => Reload();

  protected abstract IEnumerable<(long ShipId, string ShipName, int BucketIndex)> LoadEntries();

  protected void Reload()
  {
    var entries = LoadEntries().ToList();
    if (!Dispatcher.UIThread.CheckAccess())
    {
      var capturedEntries = entries;
      Dispatcher.UIThread.Post(() => ApplyReload(capturedEntries));
      return;
    }

    ApplyReload(entries);
  }

  private void ApplyReload(List<(long ShipId, string ShipName, int BucketIndex)> entries)
  {
    // Aggregate per ship: total count and weighted average load using bucket midpoints
    var perShip = entries
      .GroupBy(e => (e.ShipId, e.ShipName))
      .Select(g => new ShipAgg(
        g.Key.ShipId,
        g.Key.ShipName,
        g.Count(),
        g.Average(x => (x.BucketIndex * 10 + 5)) // midpoint (5,15,...,95)
      ))
      .OrderByDescending(x => x.Count)
      .ToList();

    // Order selected ships by average load descending (best->worst) unless reversed
    var ordered = _reverseSort
      ? perShip.OrderBy(x => x.AvgLoad).ThenByDescending(x => x.Count)
      : perShip.OrderByDescending(x => x.AvgLoad).ThenByDescending(x => x.Count);

    var shipOrder = ordered.Select(x => x.ShipName).Take((int)TopN).ToList();

    // Filter entries to those ships
    entries = entries.Where(e => shipOrder.Contains(e.ShipName)).OrderBy(e => shipOrder.IndexOf(e.ShipName)).ToList();

    _shipLabels = shipOrder;
    Series.Clear();
    Legend.Clear();
    Sections.Clear();

    // Precompute per ship total counts
    var totalsByShip = entries.GroupBy(e => e.ShipName).ToDictionary(g => g.Key, g => g.Count());

    // Prepare 10 bucket names
    var bucketNames = Enumerable.Range(0, 10).Select(i => $"{i * 10}-{(i + 1) * 10}%").ToList();

    for (int bucket = 0; bucket < 10; bucket++)
    {
      var values = new List<double?>(shipOrder.Count);
      foreach (var ship in shipOrder)
      {
        if (totalsByShip.TryGetValue(ship, out var total) && total > 0)
        {
          var count = entries.Count(e => e.ShipName == ship && e.BucketIndex == bucket);
          double pct = count * 100.0 / total;
          values.Add(count == 0 ? null : pct);
        }
        else
        {
          values.Add(null);
        }
      }

      // Skip bucket if no ship has data
      if (values.All(v => v is null))
        continue;

      var color = GetBucketColor(bucket);
      var series = new StackedColumnSeries<double?>
      {
        Name = bucketNames[bucket],
        Values = values,
        Stroke = null,
        Fill = new SolidColorPaint(color),
        MaxBarWidth = 48,
        YToolTipLabelFormatter = cp => cp.Coordinate.PrimaryValue.ToString("N2") + "%",
        XToolTipLabelFormatter = cp =>
        {
          var idx = (int)Math.Round(cp.Coordinate.SecondaryValue);
          return (idx >= 0 && idx < shipOrder.Count) ? shipOrder[idx] : string.Empty;
        },
      };
      Series.Add(series);
      var avColor = Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
      Legend.Add(new LegendItem(bucketNames[bucket], new SolidColorBrush(avColor)));
    }

    XAxes[0].Labels = shipOrder;
    PressedShipName = string.Empty;
    ShipBucketsList.Clear();
    OnPropertyChanged(nameof(Series));
    OnPropertyChanged(nameof(Legend));
    OnPropertyChanged(nameof(Labels));
    OnPropertyChanged(nameof(ChartMinWidth));
    OnPropertyChanged(nameof(PressedShipName));
    OnPropertyChanged(nameof(PressedShipTotal));
    OnPropertyChanged(nameof(ShipBucketsList));
  }

  protected static SKColor GetBucketColor(int bucket) => Utils.LoadPercentPalette.GetSkColor(bucket);

  public void OnChartPointPressed(int shipIndex)
  {
    if (shipIndex < 0 || shipIndex >= Labels.Count)
      return;
    PressedShipName = Labels[shipIndex];
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
    List<GraphBucketItem> bucketList = new();
    // Iterate series from top so higher buckets appear first similar to other stats sidebars
    for (int i = Series.Count - 1; i >= 0; i--)
    {
      if (Series[i] is StackedColumnSeries<double?> scs && scs.Values is IList<double?> valueList && shipIndex < valueList.Count)
      {
        var val = valueList[shipIndex];
        if (val.HasValue)
        {
          // Series.Name already like "0-10%"
          var legendBrush = (i < Legend.Count) ? Legend[i].Brush : null;
          IBrush? intensified = null;
          if (legendBrush is ISolidColorBrush solid)
          {
            var color = solid.Color;
            var intensifiedColor = Color.FromArgb(255, color.R, color.G, color.B);
            intensified = new SolidColorBrush(intensifiedColor);
          }
          bucketList.Add(
            new GraphBucketItem
            {
              Name = scs.Name ?? string.Empty,
              Percent = (decimal)val.Value,
              GraphBrush = intensified,
              FontWeight = FontWeight.Normal,
            }
          );
        }
      }
    }
    ShipBucketsList = new ObservableCollection<GraphBucketItem>(bucketList);
    OnPropertyChanged(nameof(PressedShipName));
    OnPropertyChanged(nameof(PressedShipTotal));
    OnPropertyChanged(nameof(ShipBucketsList));
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

  public sealed record LegendItem(string Name, ISolidColorBrush Brush);

  private sealed record ShipAgg(long ShipId, string ShipName, int Count, double AvgLoad);
}
