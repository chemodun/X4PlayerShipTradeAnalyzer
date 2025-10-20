using System.ComponentModel;
using X4PlayerShipTradeAnalyzer.Views;

namespace X4PlayerShipTradeAnalyzer.Models;

public class StationShort
{
  public long Id { get; set; }
  public string? Name { get; set; }
  public string? Sector { get; set; }
  public static List<StationShort> StationList = new(
    [
      new StationShort
      {
        Id = 0,
        Name = "-- Any / None --",
        Sector = "",
      },
    ]
  );

  public static void RefreshStationsWithTradeOrMiningSubordinates()
  {
    StationList.RemoveRange(1, StationList.Count - 1);
    var conn = MainWindow.GameData.Connection;
    if (conn == null)
      return;
    var sql =
      @"
SELECT DISTINCT cp.id AS id,
  CASE
    WHEN cp.owner = 'player'
      THEN cp.name || ' (' || cp.code || ')'
      ELSE CASE
        WHEN f.shortname IS NULL
          THEN ''
          ELSE f.shortname
        END
        || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
  END AS name,
  CASE
    WHEN sn.name IS NULL
      THEN ''
      ELSE sn.name
    END AS sector
  FROM subordinate sub
  JOIN component AS cp
    ON sub.commander_id = cp.id
  LEFT JOIN faction AS f
    ON cp.owner = f.id
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  WHERE sub.assignment IN ('trade', 'mining')
  ORDER BY cp.name;
";

    using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
      var id = Convert.ToInt64(reader["id"]);
      var name = reader["name"].ToString() ?? string.Empty;
      var sector = reader["sector"].ToString() ?? string.Empty;
      StationList.Add(
        new StationShort
        {
          Id = id,
          Name = name,
          Sector = sector,
        }
      );
    }
  }
}
