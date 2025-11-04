using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using X4PlayerShipTradeAnalyzer.Models;

namespace X4PlayerShipTradeAnalyzer.ViewModels;

/// <summary>
/// Base view model for ship-centric views. Provides:
/// - Common ShipList with sorting (by name or profit)
/// - SelectedShip handling that triggers filtering in derived classes
/// - Transport filters (Container/Solid) and SQL WHERE helper
/// - INotifyPropertyChanged plumbing
/// </summary>
public abstract class ShipsDataBaseModel : INotifyPropertyChanged
{
  // Shared ship list and sorting
  public ObservableCollection<ShipInfo> ShipList { get; } = new();

  // summary fields
  protected string _timeInService = "-";
  public string TimeInService
  {
    get => _timeInService;
    protected set
    {
      if (_timeInService != value)
      {
        _timeInService = value;
        OnPropertyChanged();
      }
    }
  }

  protected string _itemsTraded = "0";
  public string ItemsTraded
  {
    get => _itemsTraded;
    protected set
    {
      if (_itemsTraded != value)
      {
        _itemsTraded = value;
        OnPropertyChanged();
      }
    }
  }

  protected string _totalProfit = "0";
  public string TotalProfit
  {
    get => _totalProfit;
    protected set
    {
      if (_totalProfit != value)
      {
        _totalProfit = value;
        OnPropertyChanged();
      }
    }
  }

  protected string _timeMin = "-";
  public string TimeMin
  {
    get => _timeMin;
    protected set
    {
      if (_timeMin != value)
      {
        _timeMin = value;
        OnPropertyChanged();
      }
    }
  }

  protected string _timeAvg = "-";
  public string TimeAvg
  {
    get => _timeAvg;
    protected set
    {
      if (_timeAvg != value)
      {
        _timeAvg = value;
        OnPropertyChanged();
      }
    }
  }

  protected string _timeMax = "-";
  public string TimeMax
  {
    get => _timeMax;
    protected set
    {
      if (_timeMax != value)
      {
        _timeMax = value;
        OnPropertyChanged();
      }
    }
  }

  protected ShipSortOrder _shipsSortOrder = ShipSortOrder.Name;
  public ShipSortOrder ShipsSortOrder
  {
    get => _shipsSortOrder;
    protected set
    {
      if (_shipsSortOrder == value)
        return;
      _shipsSortOrder = value;
      OnPropertyChanged();
      ResortShips();
    }
  }

  protected ShipInfo? _selectedShip;
  public ShipInfo? SelectedShip
  {
    get => _selectedShip;
    protected set
    {
      if (_selectedShip == value)
        return;
      _selectedShip = value;
      OnPropertyChanged();
      ApplyShipFilter();
    }
  }

  // Ship Class filter
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
      LoadData();
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
      LoadData();
    }
  }

#pragma warning disable CA1822
  public System.Collections.Generic.IEnumerable<string> ShipClassOptions => ShipClassFilterUtil.GetShipClassOptions();
#pragma warning restore CA1822

  protected void ResortShips()
  {
    if (ShipList.Count == 0)
      return;
    var snapshot = ShipList.ToList();
    ShipList.Clear();
    foreach (var ship in SortShips(snapshot))
      ShipList.Add(ship);
  }

  private protected virtual System.Collections.Generic.IEnumerable<ShipInfo> SortShips(
    System.Collections.Generic.IEnumerable<ShipInfo> ships
  )
  {
    return ShipsSortOrder switch
    {
      ShipSortOrder.Profit => ships.OrderByDescending(s => s.EstimatedProfit ?? 0m).ThenBy(s => s.ShipName, StringComparer.Ordinal),
      _ => ships.OrderBy(s => s.ShipName, StringComparer.Ordinal),
    };
  }

  public void Refresh() => LoadData();

  // Implementations must load data and apply per-ship filtering/summaries
  protected abstract void LoadData();
  protected abstract void ApplyShipFilter();

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
