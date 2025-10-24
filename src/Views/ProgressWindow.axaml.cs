using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using X4PlayerShipTradeAnalyzer.Services;

namespace X4PlayerShipTradeAnalyzer.Views;

public partial class ProgressWindow : Window
{
  public enum ProgressMode
  {
    Unknown,
    GameData,
    SaveData,
    OnStartup,
  }

  private ProgressMode _mode = ProgressMode.Unknown;

  public ProgressWindow()
  {
    InitializeComponent();
    // Hide all counters until we detect the mode from the first update
    HideAllCounters();
  }

  private void InitializeComponent()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public void SetMessage(string message)
  {
    if (this.FindControl<TextBlock>("MessageText") is { } tb)
    {
      tb.Text = message;
    }
  }

  public void SetProgress(ProgressUpdate update)
  {
    void Set(string name, int? value)
    {
      if (value is null)
        return;
      if (this.FindControl<TextBlock>(name) is { } tb)
      {
        tb.Text = value.Value.ToString();
      }
    }

    void SetText(string name, string? value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return;
      if (this.FindControl<TextBlock>(name) is { } tb)
      {
        tb.Text = value;
      }
    }

    void SetStatus(string? status)
    {
      if (string.IsNullOrWhiteSpace(status))
        return;
      if (this.FindControl<TextBlock>("MessageText") is { } tb)
      {
        tb.Text = status;
      }
    }

    if (!Dispatcher.UIThread.CheckAccess())
    {
      Dispatcher.UIThread.Post(() => SetProgress(update));
      return;
    }

    SetStatus(update.Status);
    SetText("CurrentPackage", update.CurrentPackage);
    Set("ProcessedFiles", update.ProcessedFiles);
    Set("Languages", update.Languages);
    Set("CurrentPage", update.CurrentPage);
    Set("TItemsInPage", update.TItemsInPage);
    Set("StoredTItems", update.StoredTItems);
    Set("WaresProcessed", update.WaresProcessed);
    Set("FactionsProcessed", update.FactionsProcessed);
    Set("StoragesProcessed", update.StoragesProcessed);
    Set("ShipStoragesProcessed", update.ShipStoragesProcessed);
    Set("ShipTypesProcessed", update.ShipTypesProcessed);
    Set("ClusterSectorNamesProcessed", update.ClusterSectorNamesProcessed);
    Set("ElementsProcessed", update.ElementsProcessed);
    Set("SectorsProcessed", update.SectorsProcessed);
    Set("GatesProcessed", update.GatesProcessed);
    Set("SuperhighwaysProcessed", update.SuperhighwaysProcessed);
    Set("StationsProcessed", update.StationsProcessed);
    Set("ShipsProcessed", update.ShipsProcessed);
    Set("SubordinatesProcessed", update.SubordinatesProcessed);
    Set("RemovedProcessed", update.RemovedProcessed);
    Set("TradesProcessed", update.TradesProcessed);
  }

  public void ApplyMode(ProgressMode mode)
  {
    _mode = mode;

    // Helpers
    void SetVisibility(bool visible, params string[] names)
    {
      foreach (var n in names)
      {
        if (this.FindControl<Control>(n) is { } ctrl)
        {
          ctrl.IsVisible = visible;
          continue;
        }
        if (this.FindControl<TextBlock>(n) is { } tb)
        {
          tb.IsVisible = visible;
        }
      }
    }
    void Show(params string[] names) => SetVisibility(true, names);
    void Hide(params string[] names) => SetVisibility(false, names);

    var gameLabels = new[]
    {
      "LblCurrentPackage",
      "CurrentPackage",
      "LblProcessedFiles",
      "ProcessedFiles",
      "LblLanguages",
      "Languages",
      "LblCurrentPage",
      "CurrentPage",
      "LblTItemsInPage",
      "TItemsInPage",
      "LblStoredTItems",
      "StoredTItems",
      "LblWaresProcessed",
      "WaresProcessed",
      "LblFactionsProcessed",
      "FactionsProcessed",
      "LblStoragesProcessed",
      "StoragesProcessed",
      "LblShipStoragesProcessed",
      "ShipStoragesProcessed",
      "LblShipTypesProcessed",
      "ShipTypesProcessed",
      "LblClusterSectorNamesProcessed",
      "ClusterSectorNamesProcessed",
      "grGameCounters",
    };

    var saveLabels = new[]
    {
      "LblElementsProcessed",
      "ElementsProcessed",
      "LblSectorsProcessed",
      "SectorsProcessed",
      "LblGatesProcessed",
      "GatesProcessed",
      "LblSuperhighwaysProcessed",
      "SuperhighwaysProcessed",
      "LblStationsProcessed",
      "StationsProcessed",
      "LblShipsProcessed",
      "ShipsProcessed",
      "LblSubordinatesProcessed",
      "SubordinatesProcessed",
      "LblRemovedProcessed",
      "RemovedProcessed",
      "LblTradesProcessed",
      "TradesProcessed",
      "grSaveCounters",
    };

    var sharedContainers = new[] { "brTopDelimiter", "brBottomDelimiter", "spCounters" };

    if (mode == ProgressMode.GameData)
    {
      Show(sharedContainers);
      Show(gameLabels);
      Hide(saveLabels);
    }
    else if (mode == ProgressMode.SaveData)
    {
      Show(sharedContainers);
      Show(saveLabels);
      Hide(gameLabels);
    }
    else
    {
      Hide(sharedContainers);
      Hide(gameLabels);
      Hide(saveLabels);
    }
  }

  private void HideAllCounters()
  {
    ApplyMode(ProgressMode.Unknown);
  }
}
