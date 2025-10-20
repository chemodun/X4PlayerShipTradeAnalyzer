using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Shapes;
using X4PlayerShipTradeAnalyzer.Services;
using X4PlayerShipTradeAnalyzer.Views;

namespace X4PlayerShipTradeAnalyzer.Models;

public class Transaction
{
  public long ShipId { get; set; }
  public string ShipClass { get; set; } = string.Empty; // normalized (S,M,L,XL,...)
  public long RawTime { get; set; }
  public string FullName { get; set; } = string.Empty; // for display only
  public string Operation { get; set; } = string.Empty;
  public string Ware { get; set; } = string.Empty;
  public string Product { get; set; } = string.Empty;
  public string Transport { get; set; } = string.Empty;
  public string Station { get; set; } = string.Empty;
  public string StationOwner { get; set; } = string.Empty;
  public string StationCode { get; set; } = string.Empty;
  public string Sector { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public int Quantity { get; set; }
  public decimal Total { get; set; }
  public decimal EstimatedProfit { get; set; } // sign shows direction
  public string Time => TimeFormatter.FormatHms(RawTime);
  public int MaxQuantity { get; set; }
  public int Distance { get; set; } // distance from previous transaction sector, 0 if unknown

  public static void GetAllTransactions(ref List<Transaction> transactions)
  { //MIC-510
    var conn = MainWindow.GameData.Connection;
    List<Transaction> trans = transactions;
    if (conn == null)
      return;
    using var cmd = conn.CreateCommand();
    cmd.CommandText =
      "SELECT id, class, full_name, time, sector, sector_macro, station, counterpart_faction, counterpart_code, operation, ware, ware_name, transport, price, volume, trade_sum, profit, cargo_volume FROM player_ships_transactions_log ORDER BY id, time;";
    using var rdr = cmd.ExecuteReader();
    trans.Clear();
    long shipIdCurrent = 0;
    string lastSectorMacro = string.Empty;
    SectorToSectorConnection.ClearCaches();
    while (rdr.Read())
    {
      long shipId = Convert.ToInt64(rdr["id"]);
      string rawClass = rdr["class"].ToString() ?? string.Empty;
      string normClass = ShipClassFilterUtil.Normalize(rawClass);
      string sectorMacro = rdr["sector_macro"].ToString() ?? string.Empty;
      int distance = 0;
      if (shipIdCurrent != shipId)
      {
        shipIdCurrent = shipId;
        lastSectorMacro = string.Empty;
      }
      if (!string.IsNullOrEmpty(lastSectorMacro))
      {
        distance = SectorToSectorConnection.GetSectorDistance(lastSectorMacro, sectorMacro);
      }
      lastSectorMacro = sectorMacro;
      try
      {
        trans.Add(
          new Transaction
          {
            ShipId = shipId,
            ShipClass = normClass,
            RawTime = Convert.ToInt64(rdr["time"]),
            FullName = rdr["full_name"].ToString() ?? string.Empty,
            Sector = rdr["sector"].ToString() ?? string.Empty,
            Station = rdr["station"].ToString() ?? string.Empty,
            StationOwner = rdr["counterpart_faction"].ToString() ?? string.Empty,
            StationCode = rdr["counterpart_code"].ToString() ?? string.Empty,
            Operation = rdr["operation"].ToString() ?? string.Empty,
            Ware = rdr["ware"].ToString() ?? string.Empty,
            Product = rdr["ware_name"].ToString() ?? string.Empty,
            Transport = rdr["transport"].ToString() ?? string.Empty,
            Price = Convert.ToDecimal(rdr["price"]),
            Quantity = Convert.ToInt32(rdr["volume"]),
            Total = Convert.ToDecimal(rdr["trade_sum"]),
            EstimatedProfit = Convert.ToDecimal(rdr["profit"]),
            MaxQuantity = Convert.ToInt32(rdr["cargo_volume"]),
            Distance = distance,
          }
        );
      }
      catch
      {
        // skip invalid record
      }
    }
  }
}

public enum TransportFilter
{
  All,
  Container,
  Solid,
  Liquid,
}
