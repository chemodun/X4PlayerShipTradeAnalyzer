using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using X4PlayerShipTradeAnalyzer.Models;

namespace X4PlayerShipTradeAnalyzer.ViewModels;

public abstract class ShipsGraphsBaseModel : INotifyPropertyChanged
{
  // List item used by graphs list with visual state
  public class GraphShipItem : INotifyPropertyChanged
  {
    public int ShipId { get; init; }
    public string ShipName { get; init; } = string.Empty;

    protected decimal _estimatedProfit;
    public decimal EstimatedProfit
    {
      get => _estimatedProfit;
      set
      {
        if (_estimatedProfit != value)
        {
          _estimatedProfit = value;
          OnPropertyChanged();
        }
      }
    }

    private bool _isActive;
    public bool IsActive
    {
      get => _isActive;
      set
      {
        if (_isActive != value)
        {
          _isActive = value;
          OnPropertyChanged();
        }
      }
    }

    private IBrush? _graphBrush;
    public IBrush? GraphBrush
    {
      get => _graphBrush;
      set
      {
        if (_graphBrush != value)
        {
          _graphBrush = value;
          OnPropertyChanged();
        }
      }
    }

    private FontWeight _fontWeight = FontWeight.Normal;
    public FontWeight FontWeight
    {
      get => _fontWeight;
      set
      {
        if (_fontWeight != value)
        {
          _fontWeight = value;
          OnPropertyChanged();
        }
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }

  public ObservableCollection<ISeries> Series { get; } = new();

  public Axis[] XAxes { get; set; }
  public Axis[] YAxes { get; set; }

  public ObservableCollection<GraphShipItem> ShipList { get; set; } = new();

  protected readonly HashSet<int> _activeShipIds = new();
  protected readonly Dictionary<int, ISeries> _seriesByShipId = new();
  protected readonly Dictionary<int, SKColor> _colorByShipId = new();
  protected readonly Dictionary<int, GraphShipItem> _shipItemsById = new();
  protected ShipSortOrder _shipsSortOrder = ShipSortOrder.Name;
  public ShipSortOrder ShipsSortOrder
  {
    get => _shipsSortOrder;
    set
    {
      if (_shipsSortOrder == value)
        return;
      _shipsSortOrder = value;
      OnPropertyChanged();
      ResortShips();
    }
  }

  public ShipsGraphsBaseModel()
  {
    XAxes = new[]
    {
      new Axis
      {
        Name = "Time",
        Labeler = value => FormatTimeLabel(value),
        LabelsRotation = 0,
        SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220)) { StrokeThickness = 1 },
      },
    };
    YAxes = new[]
    {
      new Axis
      {
        Name = "Cumulative Profit",
        Labeler = value => value.ToString("N2"),
        SeparatorsPaint = new SolidColorPaint(new SKColor(220, 220, 220)) { StrokeThickness = 1 },
      },
    };

    LoadShips();
  }

  protected void ResortShips()
  {
    if (ShipList.Count == 0)
      return;
    var snapshot = ShipList.ToList();
    ShipList.Clear();
    foreach (var s in SortShips(snapshot))
      ShipList.Add(s);
  }

  protected virtual System.Collections.Generic.IEnumerable<GraphShipItem> SortShips(
    System.Collections.Generic.IEnumerable<GraphShipItem> ships
  )
  {
    return ShipsSortOrder switch
    {
      ShipSortOrder.Profit => ships.OrderByDescending(s => s.EstimatedProfit).ThenBy(s => s.ShipName, StringComparer.Ordinal),
      _ => ships.OrderBy(s => s.ShipName, StringComparer.Ordinal),
    };
  }

  public void Refresh() => ReloadShipsAndRebuildActiveSeries();

  // Implemented by derived types
  protected void ReloadShipsAndRebuildActiveSeries()
  {
    LoadShips();
    var ids = _activeShipIds.ToList();
    Series.Clear();
    _seriesByShipId.Clear();
    _activeShipIds.Clear();
    foreach (var id in ids)
    {
      if (_shipItemsById.TryGetValue(id, out var ship))
      {
        ToggleShip(ship);
      }
    }
  }

  public void ToggleShip(GraphShipItem ship)
  {
    if (_activeShipIds.Contains(ship.ShipId))
    {
      if (_seriesByShipId.TryGetValue(ship.ShipId, out var s))
      {
        Series.Remove(s);
        _seriesByShipId.Remove(ship.ShipId);
      }
      _activeShipIds.Remove(ship.ShipId);
      ship.IsActive = false;
      ship.GraphBrush = null;
      ship.FontWeight = FontWeight.Normal;
      return;
    }

    var points = LoadCumulativeProfitPoints(ship.ShipId);
    // use the dot geometry color as the semantic color for the ship
    var dotColor = GetColorForShip(ship.ShipId);
    var lineColor = new SKColor(dotColor.Red, dotColor.Green, dotColor.Blue, 180);
    var line = new LineSeries<LiveChartsCore.Defaults.ObservablePoint>
    {
      Name = ship.ShipName,
      Values = points,
      GeometrySize = 6,
      Stroke = new SolidColorPaint(lineColor, 2),
      GeometryStroke = new SolidColorPaint(dotColor, 2),
      GeometryFill = new SolidColorPaint(new SKColor(dotColor.Red, dotColor.Green, dotColor.Blue, 160)),
      Fill = null,
    };
    Series.Add(line);
    _seriesByShipId[ship.ShipId] = line;
    _activeShipIds.Add(ship.ShipId);
    ship.IsActive = true;
    ship.GraphBrush = new SolidColorBrush(Color.FromRgb(dotColor.Red, dotColor.Green, dotColor.Blue));
    ship.FontWeight = FontWeight.Bold;
  }

  protected abstract List<LiveChartsCore.Defaults.ObservablePoint> LoadCumulativeProfitPoints(int shipId);

  protected abstract void LoadShips();

  public event PropertyChangedEventHandler? PropertyChanged;

  protected static string FormatTimeLabel(double value)
  {
    var ms = Math.Abs(value);
    var ts = TimeSpan.FromMilliseconds(ms);
    return $"-{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
  }

  protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

  protected SKColor GetColorForShip(int shipId)
  {
    if (_colorByShipId.TryGetValue(shipId, out var color))
      return color;
    var idx = Math.Abs(shipId.GetHashCode()) % MainViewModel.Palette.Length;
    color = MainViewModel.Palette[idx];
    _colorByShipId[shipId] = color;
    return color;
  }
}
