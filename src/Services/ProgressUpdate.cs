namespace X4PlayerShipTradeAnalyzer.Services;

public sealed class ProgressUpdate
{
  public string? Status { get; init; }
  public string? CurrentPackage { get; init; }
  public int? ProcessedFiles { get; init; }
  public int? Languages { get; init; }
  public int? CurrentPage { get; init; }
  public int? TItemsInPage { get; init; }
  public int? StoredTItems { get; init; }
  public int? WaresProcessed { get; init; }
  public int? FactionsProcessed { get; init; }
  public int? ClusterSectorNamesProcessed { get; init; }
  public int? StoragesProcessed { get; init; }
  public int? ShipStoragesProcessed { get; init; }
  public int? ShipTypesProcessed { get; init; }

  // Save import metrics
  public int? ElementsProcessed { get; init; }
  public int? SectorsProcessed { get; init; }
  public int? GatesProcessed { get; init; }
  public int? SuperhighwaysProcessed { get; init; }
  public int? StationsProcessed { get; init; }
  public int? ShipsProcessed { get; init; }
  public int? SubordinatesProcessed { get; init; }
  public int? RemovedProcessed { get; init; }
  public int? TradesProcessed { get; init; }
}
