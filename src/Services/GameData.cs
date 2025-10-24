using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using X4PlayerShipTradeAnalyzer.Models;
using X4Unpack;

namespace X4PlayerShipTradeAnalyzer.Services;

public sealed class GameDataStats : INotifyPropertyChanged
{
  int _waresCount;
  int _gatesCount;
  int _subordinateCount;
  public int WaresCount
  {
    get => _waresCount;
    set
    {
      if (_waresCount != value)
      {
        _waresCount = value;
        OnPropertyChanged(nameof(WaresCount));
      }
    }
  }

  public int GatesCount
  {
    get => _gatesCount;
    set
    {
      if (_gatesCount != value)
      {
        _gatesCount = value;
        OnPropertyChanged(nameof(GatesCount));
      }
    }
  }

  public int SubordinateCount
  {
    get => _subordinateCount;
    set
    {
      if (_subordinateCount != value)
      {
        _subordinateCount = value;
        OnPropertyChanged(nameof(SubordinateCount));
      }
    }
  }

  int _clusterSectorNamesCount;
  public int ClusterSectorNamesCount
  {
    get => _clusterSectorNamesCount;
    set
    {
      if (_clusterSectorNamesCount != value)
      {
        _clusterSectorNamesCount = value;
        OnPropertyChanged(nameof(ClusterSectorNamesCount));
      }
    }
  }

  int _factionsCount;
  public int FactionsCount
  {
    get => _factionsCount;
    set
    {
      if (_factionsCount != value)
      {
        _factionsCount = value;
        OnPropertyChanged(nameof(FactionsCount));
      }
    }
  }

  int _playerShipsCount;
  public int PlayerShipsCount
  {
    get => _playerShipsCount;
    set
    {
      if (_playerShipsCount != value)
      {
        _playerShipsCount = value;
        OnPropertyChanged(nameof(PlayerShipsCount));
      }
    }
  }

  int _stationsCount;
  public int StationsCount
  {
    get => _stationsCount;
    set
    {
      if (_stationsCount != value)
      {
        _stationsCount = value;
        OnPropertyChanged(nameof(StationsCount));
      }
    }
  }

  int _removedObjectCount;
  public int RemovedObjectCount
  {
    get => _removedObjectCount;
    set
    {
      if (_removedObjectCount != value)
      {
        _removedObjectCount = value;
        OnPropertyChanged(nameof(RemovedObjectCount));
      }
    }
  }

  int _tradesCount;
  public int TradesCount
  {
    get => _tradesCount;
    set
    {
      if (_tradesCount != value)
      {
        _tradesCount = value;
        OnPropertyChanged(nameof(TradesCount));
      }
    }
  }

  int _languagesCount;
  public int LanguagesCount
  {
    get => _languagesCount;
    set
    {
      if (_languagesCount != value)
      {
        _languagesCount = value;
        OnPropertyChanged(nameof(LanguagesCount));
      }
    }
  }

  int _storagesCount;
  public int StoragesCount
  {
    get => _storagesCount;
    set
    {
      if (_storagesCount != value)
      {
        _storagesCount = value;
        OnPropertyChanged(nameof(StoragesCount));
      }
    }
  }

  int _shipStoragesCount;
  public int ShipStoragesCount
  {
    get => _shipStoragesCount;
    set
    {
      if (_shipStoragesCount != value)
      {
        _shipStoragesCount = value;
        OnPropertyChanged(nameof(ShipStoragesCount));
      }
    }
  }

  int _shipTypesCount;
  public int ShipTypesCount
  {
    get => _shipTypesCount;
    set
    {
      if (_shipTypesCount != value)
      {
        _shipTypesCount = value;
        OnPropertyChanged(nameof(ShipTypesCount));
      }
    }
  }

  int _currentLanguageTextCount;
  public int CurrentLanguageTextCount
  {
    get => _currentLanguageTextCount;
    set
    {
      if (_currentLanguageTextCount != value)
      {
        _currentLanguageTextCount = value;
        OnPropertyChanged(nameof(CurrentLanguageTextCount));
      }
    }
  }

  int _currentLanguageId;
  public int CurrentLanguageId
  {
    get => _currentLanguageId;
    set
    {
      if (_currentLanguageId != value)
      {
        _currentLanguageId = value;
        OnPropertyChanged(nameof(CurrentLanguageId));
      }
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class GameData
{
  private static readonly List<string> _nameIndex = new()
  {
    "",
    " I",
    " II",
    " III",
    " IV",
    " V",
    " VI",
    " VII",
    " VIII",
    " IX",
    " X",
    " XI",
    " XII",
    " XIII",
    " XIV",
    " XV",
    " XVI",
    " XVII",
    " XVIII",
    " XIX",
    " XX",
    " XXI",
    " XXII",
    " XXIII",
    " XXIV",
    " XXV",
    " XXVI",
    " XXVII",
    " XXVIII",
    " XXIX",
    " XXX",
  };
  private const int _batchSize = 1000;
  private const string LogArea = "GameData.Load";
  private readonly string _gameDataDirectory;
  private readonly string _dbPath;
  private SQLiteConnection _conn;

  public SQLiteConnection Connection
  {
    get
    {
      ReOpenConnection();
      return _conn;
    }
  }

  // DLC list is now resolved via DlcResolver with its own cache

  public GameDataStats Stats { get; } = new GameDataStats();

  private int _waresProcessed;
  private int _factionsProcessed;
  private int _storagesProcessed;
  private int _shipStoragesProcessed;
  private int _shipTypesProcessed;
  private int _processedFiles;
  private int _clusterSectorNamesProcessed;
  private long _dbSchemaVersion = 7;

  public GameData()
  {
    string baseDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
    _gameDataDirectory = Path.Combine(baseDirectory, "GameData");
    Directory.CreateDirectory(_gameDataDirectory);
    _dbPath = Path.Combine(_gameDataDirectory, "GameData.db");
    var needsCreate = !File.Exists(_dbPath);
    _conn = CreateConnection();
    if (needsCreate)
    {
      CreateDBSchema();
      SetDBSchemaVersion(_dbSchemaVersion);
      SetInitialValues();
    }
    else
    {
      long currentVersion = GetDBSchemaVersion();
      if (currentVersion < _dbSchemaVersion)
      {
        UpdateDBSchema(currentVersion);
      }
    }
    RefreshStats();
  }

  private SQLiteConnection CreateConnection()
  {
    var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
    conn.Open();
    return conn;
  }

  private void ReOpenConnection()
  {
    try
    {
      if (_conn.State == System.Data.ConnectionState.Closed)
      {
        _conn.Open();
      }
    }
    catch (Exception ex)
    {
      LoggingService.Warning("Failed to reopen SQLite connection; creating a new instance.", ex);
      _conn = CreateConnection();
    }
  }

  private long GetDBSchemaVersion()
  {
    ReOpenConnection();
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "PRAGMA user_version;";
    var result = cmd.ExecuteScalar();
    if (result is long l)
    {
      return l;
    }
    return 0;
  }

  private void SetDBSchemaVersion(long version)
  {
    ReOpenConnection();
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = $"PRAGMA user_version = {version};";
    cmd.ExecuteNonQuery();
  }

  /// <summary>
  /// Returns relative paths (from the game root) to installed DLC folders under 'extensions',
  /// sorted so that items with no present dependencies come first, followed by their dependents, etc.
  /// The list is cached per GameFolderExePath; if the configured path hasn't changed, the cached list is returned.
  /// </summary>
  public static List<string> GetExtensionsSorted()
  {
    var configuredExePath = ConfigurationService.Instance.GameFolderExePath;
    return ExtensionResolver.GetExtensionsSorted(configuredExePath);
  }

  private void CreateDBSchema()
  {
    using var cmd = _conn.CreateCommand();
    cmd.CommandText =
      @"
CREATE TABLE settings (
    current_language INTEGER NOT NULL
);
-- Table component
CREATE TABLE component (
    id      INTEGER PRIMARY KEY,
    type    TEXT NOT NULL,
    class   TEXT NOT NULL,
    macro   TEXT NOT NULL,
    owner   TEXT NOT NULL,
    sector  TEXT NOT NULL,
    name    TEXT NOT NULL,
    nameindex TEXT NOT NULL,
    code    TEXT NOT NULL
);
CREATE INDEX idx_component_type           ON component(type);
CREATE INDEX idx_component_type_owner     ON component(type, owner);
CREATE INDEX idx_component_type_owner_id  ON component (type, owner, id);
-- Table subordinate
CREATE TABLE subordinate (
    id              INTEGER PRIMARY KEY,
    commander_id    INTEGER NOT NULL,
    subordinate_id  INTEGER NOT NULL,
    assignment      TEXT NOT NULL
);
CREATE INDEX idx_subordinate_commander_id ON subordinate(commander_id);
CREATE INDEX idx_subordinate_child_id     ON subordinate(subordinate_id);
CREATE INDEX idx_subordinate_assignment  ON subordinate(assignment);
-- Table trade
CREATE TABLE trade (
    id         INTEGER PRIMARY KEY,
    seller     INTEGER NOT NULL,
    buyer      INTEGER NOT NULL,
    ware       TEXT NOT NULL,
    price      INTEGER NOT NULL,
    volume     INTEGER NOT NULL,
    time       INTEGER NOT NULL,
    trade_sum   INTEGER GENERATED ALWAYS AS (price * volume) STORED
);
CREATE INDEX idx_trade_seller_time        ON trade(seller, time);
CREATE INDEX idx_trade_buyer_time         ON trade(buyer, time);
CREATE INDEX idx_trade_ware               ON trade(ware);
CREATE INDEX idx_trade_seller_time_ware   ON trade(seller, time, ware);
CREATE INDEX idx_trade_buyer_time_ware    ON trade(buyer, time, ware);
-- Table ware
CREATE TABLE ware (
    id         TEXT PRIMARY KEY,
    name       TEXT NOT NULL,
    group_of   TEXT NOT NULL,
    transport  TEXT NOT NULL,
    volume     INTEGER NOT NULL,
    price_min  INTEGER NOT NULL,
    price_avg  INTEGER NOT NULL,
    price_max  INTEGER NOT NULL,
    component_macro TEXT NOT NULL,
    text       TEXT NOT NULL
);
CREATE INDEX idx_ware_component_macro     ON ware(component_macro);
-- Table text
CREATE TABLE text (
    id_uniq    INTEGER PRIMARY KEY,
    language   INTEGER NOT NULL,
    page       INTEGER NOT NULL,
    id         INTEGER NOT NULL,
    text       TEXT NOT NULL
);
CREATE INDEX idx_text_language_page       ON text(language, page);
CREATE INDEX idx_text_language_page_id    ON text(language, page, id);
CREATE INDEX idx_text_id                  ON text(id);
CREATE INDEX idx_text_language_id         ON text(language, id);
-- Table faction
CREATE TABLE faction (
    id         TEXT PRIMARY KEY,
    name       TEXT NOT NULL,
    shortname  TEXT NOT NULL,
    prefixname TEXT NOT NULL
);
-- Table cluster_sector_name
CREATE TABLE cluster_sector_name (
    macro      TEXT PRIMARY KEY,
    name       TEXT NOT NULL
);
-- Table gate
CREATE TABLE gate (
    id         INTEGER PRIMARY KEY,
    gate_id    INTEGER NOT NULL,
    code       TEXT NOT NULL,
    sector     TEXT NOT NULL,
    connection INTEGER NOT NULL,
    connected  INTEGER NOT NULL
);
CREATE INDEX idx_gate_id                  ON gate(gate_id);
CREATE INDEX idx_gate_sector              ON gate(sector);
CREATE INDEX idx_gate_connection          ON gate(connection);
CREATE INDEX idx_gate_connected           ON gate(connected);
-- Table superhighway
CREATE TABLE superhighway (
    id         INTEGER PRIMARY KEY,
    macro      TEXT NOT NULL,
    entrygate  INTEGER NOT NULL,
    sector_from TEXT NOT NULL,
    exitgate   INTEGER NOT NULL,
    sector_to  TEXT NOT NULL
);
CREATE INDEX idx_superhighway_entrygate   ON superhighway(entrygate);
CREATE INDEX idx_superhighway_exitgate    ON superhighway(exitgate);
CREATE INDEX idx_superhighway_sector_from ON superhighway(sector_from);
CREATE INDEX idx_superhighway_sector_to   ON superhighway(sector_to);
-- Table storage
CREATE TABLE IF NOT EXISTS storage (
    id            INTEGER PRIMARY KEY,
    macro         STRING NOT NULL,
    transport     STRING NOT NULL,
    capacity      INTEGER NOT NULL
);
CREATE INDEX idx_storage_transport        ON storage(transport);
CREATE INDEX idx_storage_macro_transport  ON storage(macro, transport);
-- Table ship_storage
CREATE TABLE IF NOT EXISTS ship_storage (
    id          INTEGER PRIMARY KEY,
    ship_macro  STRING NOT NULL,
    storage_macro STRING NOT NULL
);
CREATE INDEX idx_ship_storage_ship_macro  ON ship_storage(ship_macro);
-- Table ship_type
CREATE TABLE IF NOT EXISTS ship_type (
    macro  STRING NOT NULL,
    type   STRING NOT NULL
);
-- View lang
CREATE VIEW lang AS
SELECT t.page, t.id, t.text
FROM text AS t
JOIN settings AS s
  ON t.language = s.current_language;
-- View ships_macro_transport_capacity
CREATE VIEW ships_macro_transport_capacity AS
SELECT
    ss.ship_macro,
    s.transport,
    SUM(s.capacity) AS total_capacity
FROM
    ship_storage ss
JOIN
    storage s ON ss.storage_macro = s.macro
GROUP BY
    ss.ship_macro,
    s.transport;
-- View stations
CREATE VIEW stations AS
SELECT *
FROM component
WHERE type = 'station';
CREATE VIEW player_ships AS
SELECT *
FROM component
WHERE type = 'ship' AND owner = 'player';
-- View player_ships_with_trades
CREATE VIEW player_ships_with_trades AS
SELECT
    c.id,
    c.type,
    c.class,
    c.owner,
    c.name,
    c.code,
    SUM(
        CASE
            WHEN c.id = t.seller THEN  t.trade_sum   -- sold → positive
            WHEN c.id = t.buyer  THEN -t.trade_sum   -- bought → negative
        END
    ) AS total_trade_sum
FROM component AS c
JOIN trade AS t
  ON t.seller = c.id
   OR t.buyer = c.id
WHERE c.type = 'ship'
  AND c.owner = 'player'
GROUP BY
    c.id, c.type, c.class, c.owner, c.name, c.code;
-- View player_ships_transactions_log
CREATE VIEW player_ships_transactions_log AS
SELECT *
FROM (
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      sn.macro  AS sector_macro,
      CASE WHEN cp.owner = 'player'
        THEN cp.name || ' (' || cp.code || ')'
        ELSE
          CASE WHEN f.shortname IS NULL
            THEN ''
            ELSE f.shortname
          END
          || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'sell' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      t.trade_sum / 100.0 AS trade_sum,
      w.transport AS transport,
      CASE WHEN w.transport = 'container'
        THEN
          (t.trade_sum - w.price_avg * t.volume) / 100.0
        ELSE
          t.trade_sum / 100.0
      END AS profit,
      CASE WHEN t.ware = 'rawscrap' || tc.total_capacity IS NULL
        THEN t.volume
        ELSE
          tc.total_capacity / w.volume
      END AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.seller
  JOIN component AS cp
    ON cp.id = t.buyer AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
UNION ALL
-- Case 2: player ship is the buyer
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      sn.macro  AS sector_macro,
      CASE WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
          ELSE
            CASE WHEN f.shortname IS NULL
              THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'buy' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      -t.trade_sum/ 100.0 AS trade_sum,
      w.transport AS transport,
      CASE WHEN w.transport = 'container'
        THEN
          (w.price_avg * t.volume - t.trade_sum) / 100.0
        ELSE
          -t.trade_sum / 100.0
      END AS profit,
      CASE WHEN t.ware = 'rawscrap' || tc.total_capacity IS NULL
        THEN t.volume
        ELSE
          tc.total_capacity / w.volume
      END AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.buyer
  JOIN component AS cp
    ON cp.id = t.seller AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
) AS combined
ORDER BY full_name, time;
";
    cmd.ExecuteNonQuery();
  }

  private void UpdateDBSchema(long currentVersion)
  {
    bool clearSaveData = false;
    // Placeholder for future schema updates
    if (currentVersion < _dbSchemaVersion)
    {
      try
      {
        if (currentVersion == 0)
        {
          clearSaveData = true;
          ReOpenConnection();
          using (var deleteCmd = _conn.CreateCommand())
          {
            deleteCmd.CommandText = @"DROP TABLE IF EXISTS ware;";
            deleteCmd.ExecuteNonQuery();
          }
          using (var vacuumCmd = _conn.CreateCommand())
          {
            vacuumCmd.CommandText = "VACUUM;";
            vacuumCmd.ExecuteNonQuery();
          }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"CREATE TABLE ware (
  id         TEXT PRIMARY KEY,
  name       TEXT NOT NULL,
  group_of   TEXT NOT NULL,
  transport  TEXT NOT NULL,
  volume     INTEGER NOT NULL,
  price_min  INTEGER NOT NULL,
  price_avg  INTEGER NOT NULL,
  price_max  INTEGER NOT NULL,
  component_macro TEXT NOT NULL,
  text       TEXT NOT NULL
);
CREATE INDEX idx_ware_component_macro     ON ware(component_macro);
";
            cmd.ExecuteNonQuery();
          }
          // Perform schema updates here
          currentVersion = 1;
        }
        if (currentVersion == 1)
        {
          ReOpenConnection();
          using (var deleteCmd = _conn.CreateCommand())
          {
            deleteCmd.CommandText = @"DROP VIEW IF EXISTS player_ships_transactions_log;";
            deleteCmd.ExecuteNonQuery();
          }
          using (var vacuumCmd = _conn.CreateCommand())
          {
            vacuumCmd.CommandText = "VACUUM;";
            vacuumCmd.ExecuteNonQuery();
          }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"-- View player_ships_transactions_log
CREATE VIEW player_ships_transactions_log AS
SELECT *
FROM (
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      CASE
        WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
        ELSE CASE
              WHEN f.shortname IS NULL THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'sell' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      t.trade_sum / 100.0 AS trade_sum,
      w.transport AS transport,
      CASE
        WHEN w.transport = 'container' THEN
          (t.trade_sum - w.price_avg * t.volume) / 100.0
        ELSE
          t.trade_sum / 100.0
      END AS profit,
      t.volume * w.volume AS capacity
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.seller
  JOIN component AS cp
    ON cp.id = t.buyer
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
UNION ALL
-- Case 2: player ship is the buyer
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      CASE
        WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
        ELSE CASE
              WHEN f.shortname IS NULL THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'buy' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      -t.trade_sum/ 100.0 AS trade_sum,
      w.transport AS transport,
      CASE
        WHEN w.transport = 'container' THEN
          (w.price_avg * t.volume - t.trade_sum) / 100.0
        ELSE
          -t.trade_sum / 100.0
      END AS profit,
      t.volume * w.volume AS capacity
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.buyer
  JOIN component AS cp
    ON cp.id = t.seller
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
) AS combined
ORDER BY full_name, time
";
            cmd.ExecuteNonQuery();
          }
          // Future updates here
          currentVersion = 2;
        }
        if (currentVersion == 2)
        {
          clearSaveData = true;
          ReOpenConnection();
          using (var deleteCmd = _conn.CreateCommand())
          {
            deleteCmd.CommandText = @"DROP TABLE IF EXISTS component;DROP VIEW IF EXISTS player_ships_transactions_log;";
            deleteCmd.ExecuteNonQuery();
          }
          using (var vacuumCmd = _conn.CreateCommand())
          {
            vacuumCmd.CommandText = "VACUUM;";
            vacuumCmd.ExecuteNonQuery();
          }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"--
CREATE TABLE component (
    id      INTEGER PRIMARY KEY,
    type    TEXT NOT NULL,
    class   TEXT NOT NULL,
    macro   TEXT NOT NULL,
    owner   TEXT NOT NULL,
    sector  TEXT NOT NULL,
    name    TEXT NOT NULL,
    nameindex TEXT NOT NULL,
    code    TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS storage (
    id            INTEGER PRIMARY KEY,
    macro         STRING NOT NULL,
    transport     STRING NOT NULL,
    capacity      INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS ship_storage (
    id          INTEGER PRIMARY KEY,
    ship_macro  STRING NOT NULL,
    storage_macro STRING NOT NULL
);
CREATE INDEX idx_component_type           ON component(type);
CREATE INDEX idx_component_type_owner     ON component(type, owner);
CREATE INDEX idx_component_type_owner_id  ON component (type, owner, id);
CREATE INDEX idx_ship_storage_ship_macro  ON ship_storage(ship_macro);
CREATE INDEX idx_storage_transport        ON storage(transport);
CREATE INDEX idx_storage_macro_transport  ON storage(macro, transport);
-- View ships_macro_transport_capacity
CREATE VIEW ships_macro_transport_capacity AS
SELECT
    ss.ship_macro,
    s.transport,
    SUM(s.capacity) AS total_capacity
FROM
    ship_storage ss
JOIN
    storage s ON ss.storage_macro = s.macro
GROUP BY
    ss.ship_macro,
    s.transport;
-- View player_ships_transactions_log
CREATE VIEW player_ships_transactions_log AS
SELECT *
FROM (
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      CASE
        WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
        ELSE CASE
              WHEN f.shortname IS NULL THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'sell' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      t.trade_sum / 100.0 AS trade_sum,
      w.transport AS transport,
      CASE
        WHEN w.transport = 'container' THEN
          (t.trade_sum - w.price_avg * t.volume) / 100.0
        ELSE
          t.trade_sum / 100.0
      END AS profit,
      tc.total_capacity / w.volume AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.seller
  JOIN component AS cp
    ON cp.id = t.buyer AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
UNION ALL
-- Case 2: player ship is the buyer
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      CASE
        WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
        ELSE CASE
              WHEN f.shortname IS NULL THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'buy' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      -t.trade_sum/ 100.0 AS trade_sum,
      w.transport AS transport,
      CASE
        WHEN w.transport = 'container' THEN
          (w.price_avg * t.volume - t.trade_sum) / 100.0
        ELSE
          -t.trade_sum / 100.0
      END AS profit,
      tc.total_capacity / w.volume AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.buyer
  JOIN component AS cp
    ON cp.id = t.seller AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
) AS combined
ORDER BY full_name, time;
";
            cmd.ExecuteNonQuery();
          }
          // Future updates here
          currentVersion = 3;
        }
        if (currentVersion == 3)
        {
          clearSaveData = true;
          ReOpenConnection();
          using (var deleteCmd = _conn.CreateCommand())
          {
            deleteCmd.CommandText = @"DROP VIEW IF EXISTS player_ships_transactions_log;";
            deleteCmd.ExecuteNonQuery();
          }
          using (var vacuumCmd = _conn.CreateCommand())
          {
            vacuumCmd.CommandText = "VACUUM;";
            vacuumCmd.ExecuteNonQuery();
          }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"-- Table gate
CREATE TABLE gate (
    id         INTEGER PRIMARY KEY,
    gate_id    INTEGER NOT NULL,
    code       TEXT NOT NULL,
    sector     TEXT NOT NULL,
    connection INTEGER NOT NULL,
    connected  INTEGER NOT NULL
);
CREATE INDEX idx_gate_id                  ON gate(gate_id);
CREATE INDEX idx_gate_sector              ON gate(sector);
CREATE INDEX idx_gate_connection          ON gate(connection);
CREATE INDEX idx_gate_connected           ON gate(connected);
-- Table superhighway
CREATE TABLE superhighway (
    id         INTEGER PRIMARY KEY,
    macro      TEXT NOT NULL,
    entrygate  INTEGER NOT NULL,
    sector_from TEXT NOT NULL,
    exitgate   INTEGER NOT NULL,
    sector_to  TEXT NOT NULL
);
CREATE INDEX idx_superhighway_entrygate   ON superhighway(entrygate);
CREATE INDEX idx_superhighway_exitgate    ON superhighway(exitgate);
CREATE INDEX idx_superhighway_sector_from ON superhighway(sector_from);
CREATE INDEX idx_superhighway_sector_to   ON superhighway(sector_to);
-- View player_ships_transactions_log
CREATE VIEW player_ships_transactions_log AS
SELECT *
FROM (
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      sn.macro  AS sector_macro,
      CASE
        WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
        ELSE CASE
              WHEN f.shortname IS NULL THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'sell' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      t.trade_sum / 100.0 AS trade_sum,
      w.transport AS transport,
      CASE
        WHEN w.transport = 'container' THEN
          (t.trade_sum - w.price_avg * t.volume) / 100.0
        ELSE
          t.trade_sum / 100.0
      END AS profit,
      tc.total_capacity / w.volume AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.seller
  JOIN component AS cp
    ON cp.id = t.buyer AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
UNION ALL
-- Case 2: player ship is the buyer
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      sn.macro  AS sector_macro,
      CASE
        WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
        ELSE CASE
              WHEN f.shortname IS NULL THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'buy' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      -t.trade_sum/ 100.0 AS trade_sum,
      w.transport AS transport,
      CASE
        WHEN w.transport = 'container' THEN
          (w.price_avg * t.volume - t.trade_sum) / 100.0
        ELSE
          -t.trade_sum / 100.0
      END AS profit,
      tc.total_capacity / w.volume AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.buyer
  JOIN component AS cp
    ON cp.id = t.seller AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
) AS combined
ORDER BY full_name, time;
";
            cmd.ExecuteNonQuery();
          }
          // Future updates here
          currentVersion = 4;
        }
        if (currentVersion == 4)
        {
          clearSaveData = true;
          ReOpenConnection();
          // using (var deleteCmd = _conn.CreateCommand())
          // {
          //   deleteCmd.CommandText = @"DROP VIEW IF EXISTS player_ships_transactions_log;";
          //   deleteCmd.ExecuteNonQuery();
          // }
          // using (var vacuumCmd = _conn.CreateCommand())
          // {
          //   vacuumCmd.CommandText = "VACUUM;";
          //   vacuumCmd.ExecuteNonQuery();
          // }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"
-- Table subordinate
CREATE TABLE subordinate (
    id              INTEGER PRIMARY KEY,
    commander_id    INTEGER NOT NULL,
    subordinate_id  INTEGER NOT NULL,
    assignment      TEXT NOT NULL
);
CREATE INDEX idx_subordinate_commander_id ON subordinate(commander_id);
CREATE INDEX idx_subordinate_child_id     ON subordinate(subordinate_id);
CREATE INDEX idx_subordinate_assignment   ON subordinate(assignment);
";
            cmd.ExecuteNonQuery();
          }
          // Future updates here
          currentVersion = 5;
        }
        if (currentVersion == 5)
        {
          clearSaveData = true;
          ReOpenConnection();
          using (var deleteCmd = _conn.CreateCommand())
          {
            deleteCmd.CommandText = @"DROP VIEW IF EXISTS player_ships_transactions_log;";
            deleteCmd.ExecuteNonQuery();
          }
          using (var vacuumCmd = _conn.CreateCommand())
          {
            vacuumCmd.CommandText = "VACUUM;";
            vacuumCmd.ExecuteNonQuery();
          }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"
-- View player_ships_transactions_log
CREATE VIEW player_ships_transactions_log AS
SELECT *
FROM (
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      sn.macro  AS sector_macro,
      CASE WHEN cp.owner = 'player'
        THEN cp.name || ' (' || cp.code || ')'
        ELSE
          CASE WHEN f.shortname IS NULL
            THEN ''
            ELSE f.shortname
          END
          || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'sell' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      t.trade_sum / 100.0 AS trade_sum,
      w.transport AS transport,
      CASE WHEN w.transport = 'container'
        THEN
          (t.trade_sum - w.price_avg * t.volume) / 100.0
        ELSE
          t.trade_sum / 100.0
      END AS profit,
      CASE WHEN t.ware = 'rawscrap' || tc.total_capacity IS NULL
        THEN t.volume
        ELSE
          tc.total_capacity / w.volume
      END AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.seller
  JOIN component AS cp
    ON cp.id = t.buyer AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
UNION ALL
-- Case 2: player ship is the buyer
  SELECT
      ship.id AS id,
      ship.code AS code,
      ship.name AS name,
      ship.class AS class,
      ship.name || ' (' || ship.code || ')' AS full_name,
      cp.code   AS counterpart_code,
      cp.name   AS counterpart_name,
      cp.owner  AS counterpart_faction,
      sn.name   AS sector,
      sn.macro  AS sector_macro,
      CASE WHEN cp.owner = 'player'
          THEN cp.name || ' (' || cp.code || ')'
          ELSE
            CASE WHEN f.shortname IS NULL
              THEN ''
              ELSE f.shortname
            END
            || ' ' || cp.name || cp.nameindex || ' (' || cp.code || ')'
      END AS station,
      t.time AS time,
      'buy' AS operation,
      t.ware AS ware,
      w.text AS ware_name,
      t.price / 100.0 AS price,
      t.volume AS volume,
      -t.trade_sum/ 100.0 AS trade_sum,
      w.transport AS transport,
      CASE WHEN w.transport = 'container'
        THEN
          (w.price_avg * t.volume - t.trade_sum) / 100.0
        ELSE
          -t.trade_sum / 100.0
      END AS profit,
      CASE WHEN t.ware = 'rawscrap' || tc.total_capacity IS NULL
        THEN t.volume
        ELSE
          tc.total_capacity / w.volume
      END AS cargo_volume
  FROM trade AS t
  JOIN component AS ship
    ON ship.id = t.buyer
  JOIN component AS cp
    ON cp.id = t.seller AND cp.type = 'station'
  LEFT JOIN faction AS f
    ON f.id = cp.owner
  LEFT JOIN cluster_sector_name AS sn
    ON cp.sector = sn.macro
  JOIN ware AS w
    ON w.id = t.ware
  LEFT JOIN ships_macro_transport_capacity AS tc
    ON ship.macro = tc.ship_macro AND w.transport = tc.transport
  WHERE ship.type = 'ship'
    AND ship.owner = 'player'
) AS combined
ORDER BY full_name, time;
";
            cmd.ExecuteNonQuery();
          }
          // Future updates here
          currentVersion = 6;
        }
        if (currentVersion == 6)
        {
          clearSaveData = true;
          ReOpenConnection();
          // using (var deleteCmd = _conn.CreateCommand())
          // {
          //   deleteCmd.CommandText = @"DROP VIEW IF EXISTS player_ships_transactions_log;";
          //   deleteCmd.ExecuteNonQuery();
          // }
          // using (var vacuumCmd = _conn.CreateCommand())
          // {
          //   vacuumCmd.CommandText = "VACUUM;";
          //   vacuumCmd.ExecuteNonQuery();
          // }
          using (var cmd = _conn.CreateCommand())
          {
            cmd.CommandText =
              @"
-- Table ship_type
CREATE TABLE IF NOT EXISTS ship_type (
    macro  STRING NOT NULL,
    type   STRING NOT NULL
);
";
            cmd.ExecuteNonQuery();
          }
          // Future updates here
          currentVersion = 7;
        }
      }
      catch (Exception ex)
      {
        LoggingService.Error($"Error updating DB schema from version {currentVersion} to {_dbSchemaVersion}.", ex);
      }
      SetDBSchemaVersion(_dbSchemaVersion);
      if (clearSaveData)
      {
        ClearTablesFromGameSave();
      }
    }
  }

  private void SetInitialValues()
  {
    ReOpenConnection();
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "INSERT INTO settings (current_language) VALUES (44);";
    cmd.ExecuteNonQuery();
  }

  public void LoadGameXmlFiles(Action<ProgressUpdate>? progress = null)
  {
    ReOpenConnection();
    if (_conn == null)
      throw new InvalidOperationException("Database connection is not initialized.");
    string gamePath = ConfigurationService.Instance.GameFolderExePath ?? string.Empty;
    if (string.IsNullOrWhiteSpace(gamePath) || !Directory.Exists(gamePath))
    {
      LoggingService.Warning("Game folder path is not configured or does not exist. Skipping game XML load.", area: LogArea);
      return;
    }
    var loadStart = DateTime.Now;
    LoggingService.Information($"Starting game XML load from '{gamePath}'.", area: LogArea);
    List<string> extensionsPathList = GetExtensionsSorted();
    extensionsPathList.Insert(0, string.Empty); // base game first
    LoggingService.Debug($"Package queue prepared with {extensionsPathList.Count} entries (including base game).", area: LogArea);
    ClearTableText();
    ClearTableWare();
    ClearTableFaction();
    ClearTableClusterSectorName();
    ClearTableStorage();
    ClearTableShipStorage();
    _waresProcessed = 0;
    _factionsProcessed = 0;
    _processedFiles = 0;
    _clusterSectorNamesProcessed = 0;
    _storagesProcessed = 0;
    _shipStoragesProcessed = 0;
    // dlcPathList.Prepend(string.Empty); // base game first
    foreach (var extensionRelPath in extensionsPathList)
    {
      var dlcFullPath = string.IsNullOrWhiteSpace(extensionRelPath) ? gamePath : Path.Combine(gamePath, extensionRelPath);
      // Report current package: base game or last folder name of the dlc path
      string packageLabel = string.IsNullOrWhiteSpace(extensionRelPath)
        ? "game"
        : (extensionRelPath.Replace('\\', '/').Split('/').LastOrDefault() ?? extensionRelPath);
      progress?.Invoke(new ProgressUpdate { CurrentPackage = packageLabel });
      LoggingService.Debug($"Processing package '{packageLabel}' from '{dlcFullPath}'.", area: LogArea);

      ContentExtractor contentExtractor = new(dlcFullPath);
      if (contentExtractor.FileCount == 0)
      {
        LoggingService.Debug($"Package '{packageLabel}' contains no catalog files. Skipping.", area: LogArea);
        continue;
      }
      if (string.IsNullOrWhiteSpace(extensionRelPath))
      {
        progress?.Invoke(new ProgressUpdate { Status = "Parsing texts..." });
        LoggingService.Debug($"Loading localization texts for '{packageLabel}'.", area: LogArea);
        LoadTextsFromGameT(contentExtractor, progress);
      }
      progress?.Invoke(new ProgressUpdate { Status = "Parsing map defaults..." });
      LoggingService.Debug($"Loading map defaults for '{packageLabel}'.", area: LogArea);
      LoadMapDefaultsXml(contentExtractor, progress);
      progress?.Invoke(new ProgressUpdate { Status = "Parsing wares..." });
      LoggingService.Debug($"Loading wares for '{packageLabel}'.", area: LogArea);
      LoadWaresXml(contentExtractor, progress);
      progress?.Invoke(new ProgressUpdate { Status = "Parsing factions..." });
      LoggingService.Debug($"Loading factions for '{packageLabel}'.", area: LogArea);
      LoadFactionsXml(contentExtractor, progress);
      progress?.Invoke(new ProgressUpdate { Status = "Parsing storages & ship storages..." });
      LoggingService.Debug($"Loading storages for '{packageLabel}'.", area: LogArea);
      LoadStoragesAndShipStorages(contentExtractor, progress);
      LoggingService.Debug($"Completed package '{packageLabel}'.", area: LogArea);
    }
    RefreshStats();
    var elapsed = DateTime.Now - loadStart;
    LoggingService.Information(
      $"Completed game XML load in {elapsed}. Files processed: {_processedFiles}; wares: {_waresProcessed}; factions: {_factionsProcessed}; cluster names: {_clusterSectorNamesProcessed}; storages: {_storagesProcessed}; ship storages: {_shipStoragesProcessed}.",
      area: LogArea
    );
  }

  private void LoadStoragesAndShipStorages(ContentExtractor contentExtractor, Action<ProgressUpdate>? progress = null)
  {
    HashSet<string> allowedTransports = new(StringComparer.OrdinalIgnoreCase) { "container", "solid", "liquid", "gas" };
    ReOpenConnection();
    var entries = contentExtractor.GetFilesByMask("assets/units/size_*/macros/*.xml");
    entries = entries.Concat(contentExtractor.GetFilesByMask("assets/props/StorageModules/macros/*.xml")).ToList();
    if (entries.Count == 0)
    {
      LoggingService.Debug("No storage or ship storage macro files found for current package.", area: LogArea);
      return;
    }

    var storagesStart = DateTime.Now;
    var initialStorages = _storagesProcessed;
    var initialShipStorages = _shipStoragesProcessed;
    LoggingService.Debug($"Scanning {entries.Count} macro files for storages and ship storage links.", area: LogArea);

    SQLiteTransaction txn = _conn.BeginTransaction();
    using var insertStorage = new SQLiteCommand(
      "INSERT OR REPLACE INTO storage(macro, transport, capacity) VALUES (@macro,@transport,@capacity)",
      _conn,
      txn
    );
    insertStorage.Parameters.Add("@macro", System.Data.DbType.String);
    insertStorage.Parameters.Add("@transport", System.Data.DbType.String);
    insertStorage.Parameters.Add("@capacity", System.Data.DbType.Int64);

    using var insertShipStorage = new SQLiteCommand(
      "INSERT OR IGNORE INTO ship_storage(ship_macro, storage_macro) VALUES (@ship,@storage)",
      _conn,
      txn
    );
    insertShipStorage.Parameters.Add("@ship", System.Data.DbType.String);
    insertShipStorage.Parameters.Add("@storage", System.Data.DbType.String);

    using var insertShipType = new SQLiteCommand("INSERT OR REPLACE INTO ship_type(macro, type) VALUES (@macro,@type)", _conn, txn);
    insertShipType.Parameters.Add("@macro", System.Data.DbType.String);
    insertShipType.Parameters.Add("@type", System.Data.DbType.String);

    long writes = 0;
    foreach (var entry in entries)
    {
      try
      {
        using var cs = ContentExtractor.OpenEntryStream(entry);
        var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
        using var xr = XmlReader.Create(cs, settings);

        while (xr.Read())
        {
          if (xr.NodeType == XmlNodeType.Element && xr.Name == "macro")
          {
            string cls = xr.GetAttribute("class") ?? string.Empty;
            string name = xr.GetAttribute("name") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
              continue;

            if (string.Equals(cls, "storage", StringComparison.OrdinalIgnoreCase))
            {
              // Parse storage properties
              long capacity = 0;

              if (!xr.IsEmptyElement)
              {
                int depth = xr.Depth;
                while (xr.Read())
                {
                  if (xr.NodeType == XmlNodeType.Element && xr.Name == "properties")
                  {
                    int pDepth = xr.Depth;
                    while (xr.Read())
                    {
                      if (xr.NodeType == XmlNodeType.Element && xr.Name == "cargo")
                      {
                        var maxAttr = xr.GetAttribute("max") ?? "0";
                        long.TryParse(maxAttr, NumberStyles.Any, CultureInfo.InvariantCulture, out capacity);
                        var tagsAttr = xr.GetAttribute("tags") ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(tagsAttr))
                        {
                          foreach (var tag in tagsAttr.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                          {
                            if (allowedTransports.Contains(tag))
                            {
                              insertStorage.Parameters["@macro"].Value = name.ToLowerInvariant();
                              insertStorage.Parameters["@transport"].Value = tag.ToLowerInvariant();
                              insertStorage.Parameters["@capacity"].Value = capacity;
                              insertStorage.ExecuteNonQuery();
                              writes++;
                              _storagesProcessed++;
                              progress?.Invoke(new ProgressUpdate { StoragesProcessed = (int)Math.Min(int.MaxValue, _storagesProcessed) });
                            }
                          }
                        }
                      }
                      else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "properties" && xr.Depth == pDepth)
                      {
                        break;
                      }
                    }
                  }
                  else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "macro" && xr.Depth == depth)
                  {
                    break;
                  }
                }
              }

              // capacity already recorded per transport tag; no additional handling needed here
            }
            else if (cls.StartsWith("ship_", StringComparison.OrdinalIgnoreCase))
            {
              string shipMacro = name;
              if (!xr.IsEmptyElement)
              {
                int depth = xr.Depth;
                while (xr.Read())
                {
                  if (xr.NodeType == XmlNodeType.Element && xr.Name == "properties")
                  {
                    int pDepth = xr.Depth;
                    while (xr.Read())
                    {
                      if (xr.NodeType == XmlNodeType.Element && xr.Name == "ship")
                      {
                        string typeAttr = xr.GetAttribute("type") ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(typeAttr))
                        {
                          insertShipType.Parameters["@macro"].Value = shipMacro.ToLowerInvariant();
                          insertShipType.Parameters["@type"].Value = typeAttr.ToLowerInvariant();
                          insertShipType.ExecuteNonQuery();
                          writes++;
                          _shipTypesProcessed++;
                          progress?.Invoke(new ProgressUpdate { ShipTypesProcessed = (int)Math.Min(int.MaxValue, _shipTypesProcessed) });
                        }
                      }
                      else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "properties" && xr.Depth == pDepth)
                      {
                        break;
                      }
                    }
                  }
                  else if (xr.NodeType == XmlNodeType.Element && xr.Name == "connections")
                  {
                    int cDepth = xr.Depth;
                    while (xr.Read())
                    {
                      if (xr.NodeType == XmlNodeType.Element && xr.Name == "connection")
                      {
                        if (!xr.IsEmptyElement)
                        {
                          int d2 = xr.Depth;
                          while (xr.Read())
                          {
                            if (xr.NodeType == XmlNodeType.Element && xr.Name == "macro")
                            {
                              string conn = xr.GetAttribute("connection") ?? string.Empty;
                              if (!string.Equals(conn, "ShipConnection", StringComparison.Ordinal))
                                continue;
                              string storageRef = xr.GetAttribute("ref") ?? string.Empty;
                              if (!string.IsNullOrWhiteSpace(storageRef))
                              {
                                insertShipStorage.Parameters["@ship"].Value = shipMacro.ToLowerInvariant();
                                insertShipStorage.Parameters["@storage"].Value = storageRef.ToLowerInvariant();
                                insertShipStorage.ExecuteNonQuery();
                                writes++;
                                _shipStoragesProcessed++;
                                progress?.Invoke(
                                  new ProgressUpdate { ShipStoragesProcessed = (int)Math.Min(int.MaxValue, _shipStoragesProcessed) }
                                );
                              }
                            }
                            else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "connection" && xr.Depth == d2)
                            {
                              break;
                            }
                          }
                        }
                      }
                      else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "connections" && xr.Depth == cDepth)
                      {
                        break;
                      }
                    }
                  }
                  else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "macro" && xr.Depth == depth)
                  {
                    break;
                  }
                }
              }
            }

            if (writes > 0 && writes % _batchSize == 0)
            {
              txn.Commit();
              txn = _conn.BeginTransaction();
              insertStorage.Transaction = txn;
              insertShipStorage.Transaction = txn;
            }
          }
        }
      }
      catch (Exception ex)
      {
        LoggingService.Debug($"Skipping storage macro '{entry.FilePath}' due to parse error.", ex, area: LogArea);
      }
      finally
      {
        _processedFiles++;
        progress?.Invoke(new ProgressUpdate { ProcessedFiles = _processedFiles });
      }
    }

    txn.Commit();

    var storagesDelta = _storagesProcessed - initialStorages;
    var shipStoragesDelta = _shipStoragesProcessed - initialShipStorages;
    var elapsed = DateTime.Now - storagesStart;
    LoggingService.Debug(
      $"Completed storage parsing in {elapsed}. Added {storagesDelta} storages and {shipStoragesDelta} ship-to-storage links (totals: {_storagesProcessed}/{_shipStoragesProcessed}).",
      area: LogArea
    );

    // Final report for this package
    progress?.Invoke(
      new ProgressUpdate
      {
        StoragesProcessed = (int)Math.Min(int.MaxValue, _storagesProcessed),
        ShipStoragesProcessed = (int)Math.Min(int.MaxValue, _shipStoragesProcessed),
        ShipTypesProcessed = (int)Math.Min(int.MaxValue, _shipTypesProcessed),
        Status = "Storages parsing complete.",
      }
    );
    RefreshStats();
  }

  private void ClearTableStorage()
  {
    ReOpenConnection();
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM storage;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
  }

  private void ClearTableShipStorage()
  {
    ReOpenConnection();
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM ship_storage;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
  }

  private void ClearTableText()
  {
    ReOpenConnection();
    // 2) Refine all values and write to DB
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM text;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
  }

  private void LoadTextsFromGameT(ContentExtractor contentExtractor, Action<ProgressUpdate>? progress = null)
  {
    ReOpenConnection();
    var textsStart = DateTime.Now;
    string mask = "t/*-l*.xml";
    int langId = 0;
    string gameFolder = ConfigurationService.Instance.GameFolderExePath ?? string.Empty;
    string langPath = Path.Combine(gameFolder, "lang.dat");
    LoggingService.Debug("Loading localisation text resources.", area: LogArea);
    if (File.Exists(langPath))
    {
      var first = File.ReadLines(langPath).FirstOrDefault();
      langId = ParseInt(first);
      if (langId > 0)
      {
        using var update = new SQLiteCommand($"UPDATE settings SET current_language = {langId};", _conn);
        update.ExecuteNonQuery();
        if (ConfigurationService.Instance.LoadOnlyGameLanguage)
        {
          mask = $"t/*-l{langId:D3}.xml";
          LoggingService.Debug($"Detected game language {langId}; restricting localisation load to this language.", area: LogArea);
        }
        else
        {
          LoggingService.Debug($"Detected game language {langId}; loading all available languages.", area: LogArea);
        }
      }
    }
    List<CatEntry> catEntries = contentExtractor.GetFilesByMask(mask);
    string[] files = catEntries.Select(e => e.FilePath).Distinct().ToArray();
    if (files.Length == 0)
    {
      LoggingService.Debug("No localisation files found for current package.", area: LogArea);
      return;
    }
    LoggingService.Debug($"Parsing {files.Length} localisation file(s) with mask '{mask}'.", area: LogArea);
    // 1) Parse all files and collect raw strings per (lang,page,id)
    //    This allows forward references during refinement.
    var raw = new Dictionary<int, Dictionary<int, Dictionary<int, string>>>(); // lang -> page -> id -> text

    var seenLangs = new HashSet<int>();
    foreach (var file in files)
    {
      LoggingService.Debug($"Parsing localisation file '{file}'.", area: LogArea);
      try
      {
        CatEntry entry = catEntries.Last(e => e.FilePath == file);
        using var cs = ContentExtractor.OpenEntryStream(entry);
        var doc = XDocument.Load(cs, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

        // Find language scope element
        var languageElems = doc.Descendants()
          .Where(e => string.Equals(e.Name.LocalName, "language", StringComparison.OrdinalIgnoreCase))
          .ToList();

        XElement? scope = languageElems.First();
        int lang = 0;

        lang = ParseInt((string?)scope.Attribute("id"));

        if (lang <= 0 || scope == null)
        {
          continue;
        }
        if (seenLangs.Add(lang))
        {
          progress?.Invoke(new ProgressUpdate { Languages = seenLangs.Count });
        }

        // Helper to add an item
        void AddItem(int page, int id, string value)
        {
          if (page <= 0 || id <= 0)
            return;
          if (!raw.TryGetValue(lang, out var pages))
          {
            pages = new Dictionary<int, Dictionary<int, string>>();
            raw[lang] = pages;
          }
          if (!pages.TryGetValue(page, out var items))
          {
            items = new Dictionary<int, string>();
            pages[page] = items;
          }
          items[id] = value ?? string.Empty;
        }

        // 1) <page id> ... <t id>value</t> ...
        foreach (
          var pageElem in scope.Descendants().Where(e => string.Equals(e.Name.LocalName, "page", StringComparison.OrdinalIgnoreCase))
        )
        {
          int pageId = ParseInt((string?)pageElem.Attribute("id"));
          if (pageId <= 0)
          {
            continue;
          }
          int tCountInPage = 0;
          foreach (var tElem in pageElem.Descendants().Where(e => string.Equals(e.Name.LocalName, "t", StringComparison.OrdinalIgnoreCase)))
          {
            int id = ParseInt((string?)tElem.Attribute("id"));
            if (id <= 0)
            {
              continue;
            }
            string value = tElem.Value ?? string.Empty; // concatenated text, entities decoded
            AddItem(pageId, id, value);
            tCountInPage++;
          }
          progress?.Invoke(new ProgressUpdate { CurrentPage = pageId, TItemsInPage = tCountInPage });
        }

        // 2) <t page="..." id="...">value</t> not inside a <page>
        foreach (var tElem in scope.Descendants().Where(e => string.Equals(e.Name.LocalName, "t", StringComparison.OrdinalIgnoreCase)))
        {
          // Skip those already handled under a page
          bool underPage = tElem.Ancestors().Any(a => string.Equals(a.Name.LocalName, "page", StringComparison.OrdinalIgnoreCase));
          if (underPage)
          {
            continue;
          }
          int pageId = ParseInt((string?)tElem.Attribute("page"));
          int id = ParseInt((string?)tElem.Attribute("id"));
          if (pageId <= 0 || id <= 0)
          {
            continue;
          }
          string value = tElem.Value ?? string.Empty;
          AddItem(pageId, id, value);
        }
      }
      catch (Exception ex)
      {
        LoggingService.Debug($"Skipping localisation file '{file}' due to parse error.", ex, area: LogArea);
      }
      finally
      {
        _processedFiles++;
        progress?.Invoke(new ProgressUpdate { ProcessedFiles = _processedFiles });
      }
    }

    SQLiteTransaction txn = _conn.BeginTransaction();
    using var insert = new SQLiteCommand(
      "INSERT OR REPLACE INTO text(id_uniq, language, page, id, text) VALUES (@uniq,@lang,@page,@id,@text)",
      _conn,
      txn
    );
    insert.Parameters.Add("@uniq", System.Data.DbType.Int64);
    insert.Parameters.Add("@lang", System.Data.DbType.Int32);
    insert.Parameters.Add("@page", System.Data.DbType.Int32);
    insert.Parameters.Add("@id", System.Data.DbType.Int32);
    insert.Parameters.Add("@text", System.Data.DbType.String);
    long count = 0;
    // Determine target language when configured to load only one language
    int? targetLanguage = null;
    if (ConfigurationService.Instance.LoadOnlyGameLanguage && raw.Count > 0)
    {
      // Pick the language with the highest number of entries (data-driven)
      targetLanguage = raw.Select(kv => new { Lang = kv.Key, Count = kv.Value.Sum(p => p.Value.Count) })
        .OrderByDescending(x => x.Count)
        .First()
        .Lang;
    }

    // Memoization per language for resolved strings
    foreach (var (lang, pages) in raw)
    {
      if (targetLanguage.HasValue && lang != targetLanguage.Value)
      {
        continue; // skip other languages when only one language is requested
      }
      var memo = new Dictionary<(int page, int id), string>();

      foreach (var (page, items) in pages)
      {
        foreach (var (id, value) in items)
        {
          string refined = ResolveAndRefine(lang, page, id, raw, memo, depth: 0);
          long uniq = MakeUniqueKey(lang, page, id);
          insert.Parameters["@uniq"].Value = uniq;
          insert.Parameters["@lang"].Value = lang;
          insert.Parameters["@page"].Value = page;
          insert.Parameters["@id"].Value = id;
          insert.Parameters["@text"].Value = refined;
          insert.ExecuteNonQuery();
          count++;
          if (count % 100 == 0)
          {
            // Report stored T items progress every 100 rows
            progress?.Invoke(new ProgressUpdate { StoredTItems = (int)count });
          }
          if (count % _batchSize == 0)
          {
            txn.Commit();
            txn = _conn.BeginTransaction();
          }
        }
      }
    }
    // Final report and commit
    progress?.Invoke(new ProgressUpdate { StoredTItems = (int)count });
    txn.Commit();
    var elapsed = DateTime.Now - textsStart;
    string targetLangLabel = targetLanguage?.ToString(CultureInfo.InvariantCulture) ?? "all";
    LoggingService.Debug(
      $"Stored {count} localisation strings across {raw.Count} language(s) (target: {targetLangLabel}) in {elapsed}.",
      area: LogArea
    );
  }

  private static int ParseInt(string? s)
  {
    return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) ? v : 0;
  }

  private static long MakeUniqueKey(int language, int page, int id)
  {
    // Build a 64-bit composite key: [16 bits lang][24 bits page][24 bits id]
    long key = ((long)language << 48) | ((long)page << 24) | (uint)id;
    return key;
  }

  private static string ResolveAndRefine(
    int language,
    int page,
    int id,
    Dictionary<int, Dictionary<int, Dictionary<int, string>>> raw,
    Dictionary<(int page, int id), string> memo,
    int depth
  )
  {
    const int MaxDepth = 8;
    if (depth > MaxDepth)
    {
      return string.Empty;
    }
    if (memo.TryGetValue((page, id), out var cached))
      return cached;

    if (!raw.TryGetValue(language, out var pages) || !pages.TryGetValue(page, out var items) || !items.TryGetValue(id, out var value))
    {
      memo[(page, id)] = string.Empty;
      return string.Empty;
    }

    // First, expand references like {page,id}
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    string expanded = Regex.Replace(
      value,
      "\\{\\s*(\\d+)\\s*,\\s*(\\d+)\\s*\\}",
      m =>
      {
        int rp = ParseInt(m.Groups[1].Value);
        int rid = ParseInt(m.Groups[2].Value);
        if (rp <= 0 || rid <= 0)
          return string.Empty;
        return ResolveAndRefine(language, rp, rid, raw, memo, depth + 1);
      }
    );
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

    // Then, remove segments within unescaped parentheses; keep escaped ones literally (without backslash)
    string refined = RemoveUnescapedParentheses(expanded);

    // Normalize whitespace
    refined = refined.Replace("\r", string.Empty).Replace("\n", " ");
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    refined = Regex.Replace(refined, "\\s+", " ").Trim();
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

    memo[(page, id)] = refined;
    return refined;
  }

  private static string RemoveUnescapedParentheses(string input)
  {
    if (string.IsNullOrEmpty(input))
    {
      return string.Empty;
    }
    var sb = new StringBuilder(input.Length);
    int depth = 0;
    for (int i = 0; i < input.Length; i++)
    {
      char c = input[i];
      char prev = i > 0 ? input[i - 1] : '\0';
      bool escaped = prev == '\\';
      if (c == '(' && !escaped)
      {
        depth++;
        continue;
      }
      if (c == ')' && !escaped)
      {
        if (depth > 0)
        {
          depth--;
        }
        continue;
      }
      if (depth > 0)
      {
        // Inside excluded segment, skip unless this char reduces depth (handled above)
        continue;
      }
      if (c == '(' && escaped)
      {
        // Keep literal '(' without backslash
        // remove the backslash previously appended if any
        if (sb.Length > 0 && sb[^1] == '\\')
        {
          sb.Length -= 1;
        }
        sb.Append('(');
        continue;
      }
      if (c == ')' && escaped)
      {
        if (sb.Length > 0 && sb[^1] == '\\')
        {
          sb.Length -= 1;
        }
        sb.Append(')');
        continue;
      }
      sb.Append(c);
    }
    return sb.ToString();
  }

  private void ClearTableWare()
  {
    ReOpenConnection();
    // 2) Refine all values and write to DB
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM ware;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
  }

  private static (int page, int id) ParseTextItem(string input)
  {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    var match = Regex.Match(input, @"^\{\s*(\d+)\s*,\s*(\d+)\s*\}$", RegexOptions.IgnoreCase);
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    if (match.Success && match.Groups.Count == 3)
    {
      int page = int.TryParse(match.Groups[1].Value, out var p) ? p : 0;
      int id = int.TryParse(match.Groups[2].Value, out var t) ? t : 0;
      return (page, id);
    }
    return (0, 0);
  }

  private string GetTextItem(
    string input,
    ref Dictionary<string, string> texts,
    ref HashSet<int> processedPageIds,
    string? alternate = null
  )
  {
    if (!string.IsNullOrEmpty(input))
    {
      var (page, id) = ParseTextItem(input);
      if (page > 0 && id > 0)
      {
        if (!processedPageIds.Contains(page))
        {
          List<int> pages = new() { page };
          // Load this page into factionNames dictionary
          texts = GetTextDict(pages, texts, true);
          processedPageIds.Add(page);
        }
        if (texts.TryGetValue($"{page}:{id}", out var resolvedText))
        {
          return resolvedText;
        }
      }
    }
    return alternate ?? input;
  }

  private void LoadWaresXml(ContentExtractor contentExtractor, Action<ProgressUpdate>? progress = null)
  {
    const string file = "libraries/wares.xml";
    ReOpenConnection();

    List<CatEntry> catEntries = contentExtractor.GetFilesByMask(file);
    string[] files = catEntries.Select(e => e.FilePath).ToArray();
    if (files.Length == 0)
    {
      LoggingService.Debug("No wares.xml found in current package.", area: LogArea);
      return;
    }
    var waresStart = DateTime.Now;
    var initialWares = _waresProcessed;
    LoggingService.Debug($"Loading wares from '{file}' ({files.Length} file(s) discovered).", area: LogArea);
    try
    {
      CatEntry entry = catEntries.Last(e => e.FilePath == file);
      using var cs = ContentExtractor.OpenEntryStream(entry);
      SQLiteTransaction txn = _conn.BeginTransaction();

      using var cmd = new SQLiteCommand(
        "INSERT OR IGNORE INTO ware(id, name, group_of, transport, volume, price_min, price_avg, price_max, component_macro, text) VALUES (@id,@name,@group_of,@transport,@volume,@price_min,@price_avg,@price_max,@component_macro,@text)",
        _conn,
        txn
      );
      cmd.Parameters.Add("@id", System.Data.DbType.String);
      cmd.Parameters.Add("@name", System.Data.DbType.String);
      cmd.Parameters.Add("@group_of", System.Data.DbType.String);
      cmd.Parameters.Add("@transport", System.Data.DbType.String);
      cmd.Parameters.Add("@volume", System.Data.DbType.Int64);
      cmd.Parameters.Add("@price_min", System.Data.DbType.Int64);
      cmd.Parameters.Add("@price_avg", System.Data.DbType.Int64);
      cmd.Parameters.Add("@price_max", System.Data.DbType.Int64);
      cmd.Parameters.Add("@component_macro", System.Data.DbType.String);
      cmd.Parameters.Add("@text", System.Data.DbType.String);

      var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
      using var xr = XmlReader.Create(cs, settings);

      Dictionary<string, string> wareNames = new();
      HashSet<int> processedPageIds = new();
      while (xr.Read())
      {
        if (xr.NodeType != XmlNodeType.Element || xr.Name != "ware")
          continue;
        string id = xr.GetAttribute("id") ?? string.Empty;
        string name = xr.GetAttribute("name") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(name))
          continue;

        string group = xr.GetAttribute("group") ?? string.Empty;
        string transport = xr.GetAttribute("transport") ?? string.Empty;
        long volume = 0;
        long.TryParse(xr.GetAttribute("volume") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out volume);
        long min = 0,
          avg = 0,
          max = 0;
        string wareComponent = string.Empty;
        if (!xr.IsEmptyElement)
        {
          var depth = xr.Depth;
          while (xr.Read())
          {
            if (xr.NodeType == XmlNodeType.Element && xr.Name == "price")
            {
              long.TryParse(xr.GetAttribute("min") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out min);
              long.TryParse(xr.GetAttribute("average") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out avg);
              long.TryParse(xr.GetAttribute("max") ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out max);
            }
            else if (xr.NodeType == XmlNodeType.Element && xr.Name == "component")
            {
              wareComponent = xr.GetAttribute("ref") ?? string.Empty;
            }
            else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "ware" && xr.Depth == depth)
            {
              break;
            }
          }
        }

        cmd.Parameters["@id"].Value = id;
        cmd.Parameters["@name"].Value = name;
        cmd.Parameters["@group_of"].Value = group;
        cmd.Parameters["@transport"].Value = transport;
        cmd.Parameters["@volume"].Value = volume;
        cmd.Parameters["@price_min"].Value = min * 100;
        cmd.Parameters["@price_avg"].Value = avg * 100;
        cmd.Parameters["@price_max"].Value = max * 100;
        cmd.Parameters["@component_macro"].Value = wareComponent;
        cmd.Parameters["@text"].Value = GetTextItem(name, ref wareNames, ref processedPageIds, alternate: id);

        cmd.ExecuteNonQuery();
        _waresProcessed++;
        if (_waresProcessed % 10 == 0)
        {
          progress?.Invoke(new ProgressUpdate { WaresProcessed = _waresProcessed });
        }
      }
      // Final report
      progress?.Invoke(new ProgressUpdate { WaresProcessed = _waresProcessed });
      txn.Commit();
      var elapsed = DateTime.Now - waresStart;
      var waresDelta = _waresProcessed - initialWares;
      LoggingService.Debug(
        $"Finished loading wares in {elapsed}. Added {waresDelta} ware records (total {_waresProcessed}).",
        area: LogArea
      );
    }
    catch (Exception ex)
    {
      LoggingService.Error("Error loading wares.xml.", ex);
      return;
    }
    finally
    {
      _processedFiles++;
      progress?.Invoke(new ProgressUpdate { ProcessedFiles = _processedFiles });
    }
  }

  private void ClearTableFaction()
  {
    ReOpenConnection();
    // 2) Refine all values and write to DB
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM faction;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
  }

  private void ClearTableClusterSectorName()
  {
    ReOpenConnection();
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM cluster_sector_name;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
  }

  private void LoadFactionsXml(ContentExtractor contentExtractor, Action<ProgressUpdate>? progress = null)
  {
    const string file = "libraries/factions.xml";
    ReOpenConnection();

    List<CatEntry> catEntries = contentExtractor.GetFilesByMask(file);
    string[] files = catEntries.Select(e => e.FilePath).ToArray();
    if (files.Length == 0)
    {
      LoggingService.Debug("No factions.xml found in current package.", area: LogArea);
      return;
    }
    var factionsStart = DateTime.Now;
    var initialFactions = _factionsProcessed;
    LoggingService.Debug($"Loading factions from '{file}' ({files.Length} file(s) discovered).", area: LogArea);
    try
    {
      CatEntry entry = catEntries.Last(e => e.FilePath == file);
      using var cs = ContentExtractor.OpenEntryStream(entry);
      SQLiteTransaction txn = _conn.BeginTransaction();

      using var cmd = new SQLiteCommand(
        "INSERT OR IGNORE INTO faction(id, name, shortname, prefixname) VALUES (@id,@name,@shortname,@prefixname)",
        _conn,
        txn
      );
      cmd.Parameters.Add("@id", System.Data.DbType.String);
      cmd.Parameters.Add("@name", System.Data.DbType.String);
      cmd.Parameters.Add("@shortname", System.Data.DbType.String);
      cmd.Parameters.Add("@prefixname", System.Data.DbType.String);

      var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
      using var xr = XmlReader.Create(cs, settings);

      Dictionary<string, string> factionNames = new();
      HashSet<int> processedPageIds = new();

      while (xr.Read())
      {
        if (xr.NodeType != XmlNodeType.Element || xr.Name != "faction")
          continue;
        string id = xr.GetAttribute("id") ?? string.Empty;
        string name = GetTextItem(xr.GetAttribute("name") ?? string.Empty, ref factionNames, ref processedPageIds);
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrEmpty(name))
          continue;
        string shortname = GetTextItem(xr.GetAttribute("shortname") ?? string.Empty, ref factionNames, ref processedPageIds);
        string prefixname = GetTextItem(xr.GetAttribute("prefixname") ?? string.Empty, ref factionNames, ref processedPageIds);

        cmd.Parameters["@id"].Value = id;
        cmd.Parameters["@name"].Value = name;
        cmd.Parameters["@shortname"].Value = shortname;
        cmd.Parameters["@prefixname"].Value = prefixname;

        cmd.ExecuteNonQuery();
        _factionsProcessed++;
        progress?.Invoke(new ProgressUpdate { FactionsProcessed = _factionsProcessed });
      }
      // Final report
      txn.Commit();
      var elapsed = DateTime.Now - factionsStart;
      var factionsDelta = _factionsProcessed - initialFactions;
      LoggingService.Debug(
        $"Finished loading factions in {elapsed}. Added {factionsDelta} faction records (total {_factionsProcessed}).",
        area: LogArea
      );
    }
    catch (Exception ex)
    {
      LoggingService.Error("Error loading factions.xml.", ex);
      return;
    }
    finally
    {
      _processedFiles++;
      progress?.Invoke(new ProgressUpdate { ProcessedFiles = _processedFiles });
    }
  }

  private void LoadMapDefaultsXml(ContentExtractor contentExtractor, Action<ProgressUpdate>? progress = null)
  {
    const string file = "libraries/mapdefaults.xml";
    ReOpenConnection();

    List<CatEntry> catEntries = contentExtractor.GetFilesByMask(file);
    string[] files = catEntries.Select(e => e.FilePath).ToArray();
    if (files.Length == 0)
    {
      LoggingService.Debug("No mapdefaults.xml found in current package.", area: LogArea);
      return;
    }
    var mapDefaultsStart = DateTime.Now;
    var initialClusterNames = _clusterSectorNamesProcessed;
    LoggingService.Debug($"Loading map defaults from '{file}' ({files.Length} file(s) discovered).", area: LogArea);
    try
    {
      CatEntry entry = catEntries.Last(e => e.FilePath == file);
      using var cs = ContentExtractor.OpenEntryStream(entry);
      SQLiteTransaction txn = _conn.BeginTransaction();

      using var cmd = new SQLiteCommand("INSERT OR IGNORE INTO cluster_sector_name(macro, name) VALUES (@macro, @name)", _conn, txn);
      cmd.Parameters.Add("@macro", System.Data.DbType.String);
      cmd.Parameters.Add("@name", System.Data.DbType.String);

      var settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
      using var xr = XmlReader.Create(cs, settings);

      Dictionary<string, string> namesCache = new();
      HashSet<int> processedPageIds = new();

      while (xr.Read())
      {
        if (xr.NodeType != XmlNodeType.Element || !string.Equals(xr.Name, "dataset", StringComparison.Ordinal))
          continue;

        string macro = xr.GetAttribute("macro") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(macro))
        {
          // skip entries without macro
          continue;
        }

        string resolvedName = macro; // default fallback

        if (!xr.IsEmptyElement)
        {
          int depth = xr.Depth;
          while (xr.Read())
          {
            if (xr.NodeType == XmlNodeType.Element && string.Equals(xr.Name, "identification", StringComparison.Ordinal))
            {
              string nameAttr = xr.GetAttribute("name") ?? string.Empty;
              if (!string.IsNullOrEmpty(nameAttr))
              {
                resolvedName = GetTextItem(nameAttr, ref namesCache, ref processedPageIds, alternate: macro);
              }
            }
            else if (
              xr.NodeType == XmlNodeType.EndElement
              && string.Equals(xr.Name, "dataset", StringComparison.Ordinal)
              && xr.Depth == depth
            )
            {
              break;
            }
          }
        }

        cmd.Parameters["@macro"].Value = macro.ToLowerInvariant();
        cmd.Parameters["@name"].Value = resolvedName;
        cmd.ExecuteNonQuery();

        _clusterSectorNamesProcessed++;
        if (_clusterSectorNamesProcessed % 10 == 0)
        {
          progress?.Invoke(new ProgressUpdate { ClusterSectorNamesProcessed = _clusterSectorNamesProcessed });
        }
      }

      // Final report for this file/package
      progress?.Invoke(new ProgressUpdate { ClusterSectorNamesProcessed = _clusterSectorNamesProcessed });
      txn.Commit();
      var elapsed = DateTime.Now - mapDefaultsStart;
      var namesDelta = _clusterSectorNamesProcessed - initialClusterNames;
      LoggingService.Debug(
        $"Finished loading map defaults in {elapsed}. Added {namesDelta} cluster/sector names (total {_clusterSectorNamesProcessed}).",
        area: LogArea
      );
    }
    catch (Exception ex)
    {
      LoggingService.Error("Error loading mapdefaults.xml.", ex);
      return;
    }
    finally
    {
      _processedFiles++;
      progress?.Invoke(new ProgressUpdate { ProcessedFiles = _processedFiles });
    }
  }

  private Dictionary<string, string> GetWareComponentNamesDict()
  {
    ReOpenConnection();
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "SELECT component_macro, text FROM ware WHERE component_macro IS NOT NULL AND component_macro != ''";
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
      string macro = rdr.GetString(0).ToLowerInvariant();
      string name = rdr.GetString(1);
      if (!dict.ContainsKey(macro))
      {
        dict[macro] = name;
      }
    }
    return dict;
  }

  public void ClearTablesFromGameSave()
  {
    ReOpenConnection();
    // 2) Refine all values and write to DB
    using (var deleteCmd = _conn.CreateCommand())
    {
      deleteCmd.CommandText = @"DELETE FROM trade; DELETE FROM component;";
      deleteCmd.ExecuteNonQuery();
    }
    using (var checkCmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='gate';", _conn))
    {
      using var reader = checkCmd.ExecuteReader();

      if (reader.Read())
      {
        using var deleteCmd = new SQLiteCommand("DELETE FROM gate;", _conn);
        deleteCmd.ExecuteNonQuery();
      }
    }
    using (var checkCmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='superhighway';", _conn))
    {
      using var reader = checkCmd.ExecuteReader();

      if (reader.Read())
      {
        using var deleteCmd = new SQLiteCommand("DELETE FROM superhighway;", _conn);
        deleteCmd.ExecuteNonQuery();
      }
    }
    using (var checkCmd = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='subordinate';", _conn))
    {
      using var reader = checkCmd.ExecuteReader();

      if (reader.Read())
      {
        using var deleteCmd = new SQLiteCommand("DELETE FROM subordinate;", _conn);
        deleteCmd.ExecuteNonQuery();
      }
    }
    using (var vacuumCmd = _conn.CreateCommand())
    {
      vacuumCmd.CommandText = "VACUUM;";
      vacuumCmd.ExecuteNonQuery();
    }
    RefreshStats();
  }

  private GameComponent GetComponentById(long id)
  {
    using var conn = CreateConnection();
    if (conn == null)
      throw new InvalidOperationException("Database connection is not initialized.");
    using var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT id, type, class, owner, sector, name, nameindex, code FROM component WHERE id = @id LIMIT 1";
    cmd.Parameters.AddWithValue("@id", id);
    using var rdr = cmd.ExecuteReader();
    if (rdr.Read())
    {
      return new GameComponent
      {
        Id = rdr.GetInt64(0),
        Type = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1),
        ComponentClass = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
        Owner = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3),
        Sector = rdr.IsDBNull(4) ? string.Empty : rdr.GetString(4),
        Name = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5),
        NameIndex = rdr.IsDBNull(6) ? string.Empty : rdr.GetString(6),
        Code = rdr.IsDBNull(7) ? string.Empty : rdr.GetString(7),
      };
    }
    return new GameComponent();
  }

  public void ImportSaveGame(Action<ProgressUpdate>? progress = null)
  {
    // Prefer external SaveGamesFolder from configuration; fallback to local GameData copy
    string savePath = ConfigurationService.Instance.GameSavePath ?? string.Empty;
    if (string.IsNullOrWhiteSpace(savePath))
    {
      LoggingService.Warning("Save import requested but no save path is configured.", area: LogArea);
      return;
    }
    if (!File.Exists(savePath))
    {
      LoggingService.Warning($"Save import requested but file '{savePath}' was not found.", area: LogArea);
      return;
    }

    ClearTablesFromGameSave();
    LoggingService.Debug("Cleared existing save data tables prior to import.", area: LogArea);
    ReOpenConnection();

    var factoryNames = GetFactoryNamesDict();
    var factionsShortNames = GetFactionsShortNamesDict();
    var shipTypes = GetShipTypesDict();
    LoggingService.Debug(
      $"Loaded lookup dictionaries: factoryNames={factoryNames.Count}, factionsShortNames={factionsShortNames.Count}, shipTypes={shipTypes.Count}.",
      area: LogArea
    );

    // Begin transactional import for speed and atomicity
    SQLiteTransaction txn = _conn.BeginTransaction();

    using var insertComp = new SQLiteCommand(
      "INSERT OR IGNORE INTO component(id, type, class, macro, owner, sector, name, nameindex, code) VALUES (@id,@type,@class,@macro,@owner,@sector,@name,@nameindex,@code)",
      _conn,
      txn
    );
    insertComp.Parameters.Add("@id", System.Data.DbType.Int64);
    insertComp.Parameters.Add("@type", System.Data.DbType.String);
    insertComp.Parameters.Add("@class", System.Data.DbType.String);
    insertComp.Parameters.Add("@macro", System.Data.DbType.String);
    insertComp.Parameters.Add("@owner", System.Data.DbType.String);
    insertComp.Parameters.Add("@sector", System.Data.DbType.String);
    insertComp.Parameters.Add("@name", System.Data.DbType.String);
    insertComp.Parameters.Add("@nameindex", System.Data.DbType.String);
    insertComp.Parameters.Add("@code", System.Data.DbType.String);

    using var insertTrade = new SQLiteCommand(
      "INSERT OR IGNORE INTO trade(seller, buyer, ware, price, volume, time) VALUES (@seller,@buyer,@ware,@price,@volume,@time)",
      _conn,
      txn
    );
    insertTrade.Parameters.Add("@seller", System.Data.DbType.Int64);
    insertTrade.Parameters.Add("@buyer", System.Data.DbType.Int64);
    insertTrade.Parameters.Add("@ware", System.Data.DbType.String);
    insertTrade.Parameters.Add("@price", System.Data.DbType.Int64);
    insertTrade.Parameters.Add("@volume", System.Data.DbType.Int64);
    insertTrade.Parameters.Add("@time", System.Data.DbType.Int64);

    using var insertGate = new SQLiteCommand(
      "INSERT OR IGNORE INTO gate(gate_id, code, sector, connection, connected) VALUES (@gate_id,@code,@sector,@connection,@connected);",
      _conn,
      txn
    );
    insertGate.Parameters.Add("@gate_id", System.Data.DbType.Int64);
    insertGate.Parameters.Add("@code", System.Data.DbType.String);
    insertGate.Parameters.Add("@sector", System.Data.DbType.String);
    insertGate.Parameters.Add("@connection", System.Data.DbType.Int64);
    insertGate.Parameters.Add("@connected", System.Data.DbType.Int64);

    using var insertSuperhighway = new SQLiteCommand(
      "INSERT OR IGNORE INTO superhighway(id, macro, sector_from, entrygate, sector_to, exitgate) VALUES (@id, @macro, @sector_from, @entrygate, @sector_to, @exitgate);",
      _conn,
      txn
    );
    insertSuperhighway.Parameters.Add("@id", System.Data.DbType.Int64);
    insertSuperhighway.Parameters.Add("@macro", System.Data.DbType.String);
    insertSuperhighway.Parameters.Add("@sector_from", System.Data.DbType.String);
    insertSuperhighway.Parameters.Add("@entrygate", System.Data.DbType.Int64);
    insertSuperhighway.Parameters.Add("@sector_to", System.Data.DbType.String);
    insertSuperhighway.Parameters.Add("@exitgate", System.Data.DbType.Int64);

    using var insertSubordinate = new SQLiteCommand(
      "INSERT OR IGNORE INTO subordinate(commander_id, subordinate_id, assignment) VALUES (@commander_id, @subordinate_id, @assignment);",
      _conn,
      txn
    );
    insertSubordinate.Parameters.Add("@commander_id", System.Data.DbType.Int64);
    insertSubordinate.Parameters.Add("@subordinate_id", System.Data.DbType.Int64);
    insertSubordinate.Parameters.Add("@assignment", System.Data.DbType.String);

    long elementsProcessed = 0;
    int itemsForTransaction = 0,
      sectorCount = 0,
      removedCount = 0,
      stationsProcessed = 0,
      shipsProcessed = 0,
      subordinatesProcessed = 0,
      gatesProcessed = 0,
      superhighwaysProcessed = 0,
      tradeCount = 0;

    DateTime startTime = DateTime.Now;
    LoggingService.Information($"Save import started at {startTime:O} from '{savePath}'.", area: LogArea);
    bool traceEnabled = LoggingService.MinimumLevel <= LogLevel.Debug;
    LoggingService.Debug($"Configuration snapshot: LoadRemovedObjects={ConfigurationService.Instance.LoadRemovedObjects}.", area: LogArea);

    using var fs = new FileStream(savePath, FileMode.Open, FileAccess.Read);
    using var gz = new GZipStream(fs, CompressionMode.Decompress);
    using var sr = new StreamReader(gz);
    XmlReaderSettings settings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
    using var xr = XmlReader.Create(sr, settings);

    bool tradeEntries = false;
    bool removedEntries = false;
    bool removedProcessed = false;
    bool connectionsProcessed = false;
    bool tradesProcessed = false;
    bool timeProcessed = false;
    bool detectNameViaProduction = false;
    long gameTime = 0;
    string currentSector = string.Empty;
    Dictionary<string, string> factoryBaseNames = new();
    HashSet<int> processedPageIds = new();
    GameComponent? currentStation = null;
    Dictionary<string, string> wareComponentNames = GetWareComponentNamesDict();
    Dictionary<long, string> zonesToSectors = new();
    List<SuperHighway> superhighwayBuffer = new();
    Dictionary<long, HighWayGate> gatesBuffer = new();
    List<SubordinateOnInput> subordinateBuffer = new();
    List<(long id, int groupId, string name)> commanderGroupsBuffer = new();
    Dictionary<long, int> subordinateIdToGroupId = new();

    void InsertSubordinate(SubordinateOnInput sub)
    {
      if (sub.CommanderId > 0 && sub.SubordinateId > 0)
      {
        insertSubordinate.Parameters["@commander_id"].Value = sub.CommanderId;
        insertSubordinate.Parameters["@subordinate_id"].Value = sub.SubordinateId;
        insertSubordinate.Parameters["@assignment"].Value = sub.Group;
        insertSubordinate.ExecuteNonQuery();
        itemsForTransaction++;
        subordinatesProcessed++;
        if (subordinatesProcessed % 10 == 0)
        {
          progress?.Invoke(new ProgressUpdate { SubordinatesProcessed = subordinatesProcessed });
        }
        if (traceEnabled && subordinatesProcessed % 25 == 0)
        {
          LoggingService.Debug(
            $"Subordinate links processed: {subordinatesProcessed}. Latest commander {sub.CommanderId} -> subordinate {sub.SubordinateId} ({sub.Group}).",
            area: LogArea
          );
        }
        subordinateBuffer.Remove(sub);
      }
    }

    long ProcessStationOrShip(string owner, string componentClass)
    {
      long id = ParseId(xr.GetAttribute("id") ?? string.Empty);
      if (id <= 0)
      {
        return 0;
      }
      string type = componentClass == "station" ? "station" : "ship";
      if (type == "ship" && owner != "player")
      {
        return 0;
      }
      string name = xr.GetAttribute("name") ?? "";
      if (string.IsNullOrWhiteSpace(name))
      {
        name = xr.GetAttribute("basename") ?? "";
      }
      string code = xr.GetAttribute("code") ?? "";
      if (string.IsNullOrWhiteSpace(code))
      {
        return 0;
      }
      string nameIndex = _nameIndex[int.Parse(xr.GetAttribute("nameindex") ?? "0")] ?? "";
      string macro = "";
      if (componentClass == "station")
      {
        if (string.IsNullOrEmpty(name))
        {
          currentStation = new GameComponent
          {
            Id = id,
            Type = type,
            ComponentClass = componentClass,
            Macro = macro,
            Owner = owner,
            Sector = currentSector,
            NameIndex = nameIndex,
            Code = code,
          };
          detectNameViaProduction = true;
          // continue;
        }
        else
        {
          detectNameViaProduction = false;
        }
        stationsProcessed++;
        if (stationsProcessed % 50 == 0)
        {
          progress?.Invoke(new ProgressUpdate { StationsProcessed = stationsProcessed });
        }
        if (traceEnabled && stationsProcessed % 25 == 0)
        {
          LoggingService.Debug(
            $"Stations processed: {stationsProcessed}. Latest code {code}, sector {currentSector}, owner {owner}.",
            area: LogArea
          );
        }
      }
      else
      {
        macro = xr.GetAttribute("macro") ?? string.Empty;
        if (skipShipByType(macro, shipTypes))
        {
          return 0;
        }
        shipsProcessed++;
        if (shipsProcessed % 50 == 0)
        {
          progress?.Invoke(new ProgressUpdate { ShipsProcessed = shipsProcessed });
        }
        if (traceEnabled && shipsProcessed % 25 == 0)
        {
          LoggingService.Debug(
            $"Player ships processed: {shipsProcessed}. Latest macro '{macro}' code {code}, sector {currentSector}.",
            area: LogArea
          );
        }
        if (string.IsNullOrEmpty(name))
        {
          if (!string.IsNullOrEmpty(macro))
          {
            name = wareComponentNames.TryGetValue(macro.ToLowerInvariant(), out var n) ? n : string.Empty;
          }
        }
      }
      if (!detectNameViaProduction)
      {
        insertComp.Parameters["@id"].Value = id;
        insertComp.Parameters["@type"].Value = type;
        insertComp.Parameters["@class"].Value = componentClass;
        insertComp.Parameters["@macro"].Value = macro;
        insertComp.Parameters["@sector"].Value = currentSector;
        insertComp.Parameters["@owner"].Value = owner;
        insertComp.Parameters["@code"].Value = code;
        insertComp.Parameters["@nameindex"].Value = nameIndex;
        insertComp.Parameters["@name"].Value = GetTextItem(name, ref factoryBaseNames, ref processedPageIds);
        insertComp.ExecuteNonQuery();
        itemsForTransaction++;
      }
      if (!detectNameViaProduction && owner != "player")
      {
        return 0;
      }
      return id;
    }

    void ProcessSubordinates(long currentId)
    {
      int subordinateGroup = 0;
      if (xr.Name == "subordinates")
      {
        int depthSub = xr.Depth;
        while (xr.Read())
        {
          if (xr.NodeType == XmlNodeType.Element && xr.Name == "group")
          {
            int groupIdx = ParseInt(xr.GetAttribute("index") ?? string.Empty);
            string groupName = xr.GetAttribute("assignmment") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(groupName))
            {
              groupName = xr.GetAttribute("assignment") ?? string.Empty;
            }
            commanderGroupsBuffer.Add((currentId, groupIdx, groupName));
          }
          else if (
            xr.NodeType == XmlNodeType.EndElement
            && string.Equals(xr.Name, "subordinates", StringComparison.Ordinal)
            && xr.Depth == depthSub
          )
          {
            break;
          }
        }
        return;
      }
      if (xr.Name == "subordinate")
      {
        subordinateGroup = ParseInt(xr.GetAttribute("group") ?? string.Empty);
        if (subordinateGroup <= 0)
        {
          return;
        }
        subordinateIdToGroupId[currentId] = subordinateGroup;
        return;
      }
      if (xr.Name == "connection")
      {
        if (xr.GetAttribute("connection") == "subordinates")
        {
          long subordinateConnectionId = ParseId(xr.GetAttribute("id") ?? string.Empty);
          if (subordinateConnectionId <= 0)
          {
            return;
          }
          var subordinate = subordinateBuffer.FirstOrDefault(s =>
            s.CommanderConnectionId == subordinateConnectionId && s.SubordinateId > 0
          );
          if (subordinate != null)
          {
            subordinate.CommanderId = currentId;
            int subordinateGroupId = subordinateIdToGroupId[subordinate.SubordinateId];
            subordinate.Group =
              commanderGroupsBuffer.FirstOrDefault(g => g.id == currentId && g.groupId == subordinateGroupId).name ?? string.Empty;
            if (subordinate.IsValid)
            {
              InsertSubordinate(subordinate);
            }
            return;
          }
          subordinateBuffer.Add(
            new SubordinateOnInput
            {
              CommanderId = currentId,
              CommanderConnectionId = subordinateConnectionId,
              SubordinateId = 0,
              Group = string.Empty,
            }
          );
        }
        if (xr.GetAttribute("connection") == "commander")
        {
          int depthConn = xr.Depth;
          while (xr.Read())
          {
            if (xr.NodeType == XmlNodeType.Element && xr.Name == "connected")
            {
              long commanderConnectionId = ParseId(xr.GetAttribute("connection") ?? string.Empty);
              if (commanderConnectionId <= 0)
              {
                continue;
              }
              var commander = subordinateBuffer.FirstOrDefault(s => s.CommanderId > 0 && s.CommanderConnectionId == commanderConnectionId);
              if (commander != null)
              {
                commander.SubordinateId = currentId;

                int subordinateGroupId = subordinateIdToGroupId[currentId];
                commander.Group =
                  commanderGroupsBuffer.FirstOrDefault(g => g.id == commander.CommanderId && g.groupId == subordinateGroupId).name
                  ?? string.Empty;
                if (commander.IsValid)
                {
                  InsertSubordinate(commander);
                }
                continue;
              }
              subordinateBuffer.Add(
                new SubordinateOnInput
                {
                  CommanderId = 0,
                  CommanderConnectionId = commanderConnectionId,
                  SubordinateId = currentId,
                  Group = string.Empty,
                }
              );
              continue;
            }
            else if (
              xr.NodeType == XmlNodeType.EndElement
              && string.Equals(xr.Name, "connection", StringComparison.Ordinal)
              && xr.Depth == depthConn
            )
            {
              break;
            }
          }
        }
        return;
      }
    }

    while (xr.Read())
    {
      if (xr.NodeType != XmlNodeType.Element && xr.NodeType != XmlNodeType.EndElement)
        continue;
      if (xr.NodeType == XmlNodeType.Element)
      {
        elementsProcessed++;
        if (elementsProcessed % 2000 == 0)
        {
          progress?.Invoke(new ProgressUpdate { ElementsProcessed = (int)Math.Min(int.MaxValue, elementsProcessed) });
        }
        if (traceEnabled && elementsProcessed % 10000 == 0)
        {
          LoggingService.Debug($"XML elements processed so far: {elementsProcessed:n0}.", area: LogArea);
        }
        if (!timeProcessed && xr.Name == "game")
        {
          gameTime = NormalizeTime(xr.GetAttribute("time") ?? string.Empty);
          timeProcessed = true;
        }
        if (!tradesProcessed && xr.Name == "entries" && xr.GetAttribute("type") == "trade")
        {
          LoggingService.Debug("Switching to trade entries import phase.", area: LogArea);
          tradeEntries = true;
          detectNameViaProduction = false;
          progress?.Invoke(new ProgressUpdate { Status = "Importing trade logs..." });
          txn.Commit();
          txn.Dispose();
          txn = _conn.BeginTransaction();
          // rebind commands to the new transaction
          insertComp.Transaction = txn;
          insertTrade.Transaction = txn;
          continue;
        }
        if (connectionsProcessed && !removedProcessed && !removedEntries && xr.Name == "removed")
        {
          LoggingService.Debug("Switching to removed objects import phase.", area: LogArea);
          removedEntries = true;
          tradeEntries = false;
          continue;
        }
        if (!connectionsProcessed && xr.Name == "component")
        {
          if (xr.GetAttribute("knownto") != "player")
            continue;

          string componentClass = xr.GetAttribute("class") ?? string.Empty;

          if (componentClass == "highway")
          {
            long id = ParseId(xr.GetAttribute("id") ?? string.Empty);
            if (id <= 0)
            {
              continue;
            }
            string macro = xr.GetAttribute("macro") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(macro))
            {
              continue;
            }
            int depth = xr.Depth;
            bool isSuperHighway = false;
            SuperHighway superHighway = new()
            {
              Id = id,
              Macro = macro,
              SectorFrom = "",
              EntryGate = 0,
              SectorTo = "",
              ExitGate = 0,
            };
            while (xr.Read())
            {
              if (xr.NodeType == XmlNodeType.Element)
              {
                if (string.Equals(xr.Name, "highway", StringComparison.Ordinal))
                {
                  isSuperHighway = xr.GetAttribute("superhighway") == "1";
                  if (!isSuperHighway)
                    break;
                  continue;
                }
                if (string.Equals(xr.Name, "connection", StringComparison.Ordinal))
                {
                  if (xr.GetAttribute("connection") == "entrygate")
                  {
                    long entryGateId = ParseId(xr.GetAttribute("id") ?? string.Empty);
                    if (entryGateId <= 0)
                    {
                      break;
                    }
                    superHighway.EntryGate = entryGateId;
                    continue;
                  }
                  if (xr.GetAttribute("connection") == "exitgate")
                  {
                    long exitGateId = ParseId(xr.GetAttribute("id") ?? string.Empty);
                    if (exitGateId <= 0)
                    {
                      break;
                    }
                    superHighway.ExitGate = exitGateId;
                    continue;
                  }
                }
              }
              else if (
                xr.NodeType == XmlNodeType.EndElement
                && string.Equals(xr.Name, "component", StringComparison.Ordinal)
                && xr.Depth == depth
              )
              {
                break;
              }
            }
            if (isSuperHighway && superHighway.IsPrepared)
            {
              if (gatesBuffer.TryGetValue(superHighway.EntryGate, out var entryGate) && entryGate.Type == "highwayentrygate")
              {
                superHighway.SectorFrom = entryGate.Sector;
                gatesBuffer.Remove(superHighway.EntryGate);
              }
              if (gatesBuffer.TryGetValue(superHighway.ExitGate, out var exitGate) && exitGate.Type == "highwayexitgate")
              {
                superHighway.SectorTo = exitGate.Sector;
                gatesBuffer.Remove(superHighway.ExitGate);
              }
              if (superHighway.IsReady)
              {
                insertSuperhighway.Parameters["@id"].Value = superHighway.Id;
                insertSuperhighway.Parameters["@macro"].Value = superHighway.Macro;
                insertSuperhighway.Parameters["@sector_from"].Value = superHighway.SectorFrom;
                insertSuperhighway.Parameters["@entrygate"].Value = superHighway.EntryGate;
                insertSuperhighway.Parameters["@sector_to"].Value = superHighway.SectorTo;
                insertSuperhighway.Parameters["@exitgate"].Value = superHighway.ExitGate;
                insertSuperhighway.ExecuteNonQuery();
                itemsForTransaction++;
                superhighwaysProcessed++;
                progress?.Invoke(new ProgressUpdate { SuperhighwaysProcessed = superhighwaysProcessed });
                if (traceEnabled && superhighwaysProcessed % 5 == 0)
                {
                  LoggingService.Debug(
                    $"Superhighways processed: {superhighwaysProcessed}. Latest macro {superHighway.Macro} from {superHighway.SectorFrom} to {superHighway.SectorTo}.",
                    area: LogArea
                  );
                }
              }
              else
              {
                // still missing gate info - keep in buffer for later processing
                superhighwayBuffer.Add(superHighway);
              }
            }
            continue;
          }

          if (componentClass == "sector")
          {
            currentSector = xr.GetAttribute("macro") ?? string.Empty;
            sectorCount++;
            if (sectorCount % 10 == 0)
            {
              progress?.Invoke(new ProgressUpdate { SectorsProcessed = sectorCount });
            }
            if (traceEnabled && sectorCount % 10 == 0)
            {
              LoggingService.Debug($"Sectors processed: {sectorCount}. Current sector '{currentSector}'.", area: LogArea);
            }
            continue;
          }
          if (componentClass == "zone" && !string.IsNullOrEmpty(currentSector))
          {
            // map zone macro to sector macro for later use
            long id = ParseId(xr.GetAttribute("id") ?? string.Empty);
            if (id <= 0)
            {
              continue;
            }
            zonesToSectors[id] = currentSector;
            continue;
          }
          if (componentClass == "gate" && !string.IsNullOrEmpty(currentSector))
          {
            long id = ParseId(xr.GetAttribute("id") ?? string.Empty);
            if (id <= 0)
            {
              continue;
            }
            string code = xr.GetAttribute("code") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(code))
            {
              continue;
            }
            // if (xr.GetAttribute("known") != "1")
            // {
            //   continue;
            // }
            long connectionId = 0;
            long connectedId = 0;
            int depth = xr.Depth;
            while (xr.Read())
            {
              if (xr.NodeType == XmlNodeType.Element && string.Equals(xr.Name, "connection", StringComparison.Ordinal))
              {
                connectionId = ParseId(xr.GetAttribute("id") ?? string.Empty);
                if (connectionId <= 0)
                {
                  continue;
                }
                int depthConn = xr.Depth;
                while (xr.Read())
                {
                  if (xr.NodeType == XmlNodeType.Element && string.Equals(xr.Name, "connected", StringComparison.Ordinal))
                  {
                    connectedId = ParseId(xr.GetAttribute("connection") ?? string.Empty);
                    if (connectedId <= 0)
                    {
                      continue;
                    }
                    insertGate.Parameters["@gate_id"].Value = id;
                    insertGate.Parameters["@code"].Value = code;
                    insertGate.Parameters["@sector"].Value = currentSector;
                    insertGate.Parameters["@connection"].Value = connectionId;
                    insertGate.Parameters["@connected"].Value = connectedId;
                    insertGate.ExecuteNonQuery();
                    itemsForTransaction++;
                    gatesProcessed++;
                    if (gatesProcessed % 10 == 0)
                    {
                      progress?.Invoke(new ProgressUpdate { GatesProcessed = gatesProcessed });
                    }
                    if (traceEnabled && gatesProcessed % 20 == 0)
                    {
                      LoggingService.Debug(
                        $"Gates processed: {gatesProcessed}. Sector {currentSector}, code {code}, connection {connectionId} -> {connectedId}.",
                        area: LogArea
                      );
                    }
                    connectedId = 0;
                  }
                  else if (
                    xr.NodeType == XmlNodeType.EndElement
                    && string.Equals(xr.Name, "connection", StringComparison.Ordinal)
                    && xr.Depth == depthConn
                  )
                  {
                    connectionId = 0;
                    break;
                  }
                }
              }
              else if (
                xr.NodeType == XmlNodeType.EndElement
                && string.Equals(xr.Name, "component", StringComparison.Ordinal)
                && xr.Depth == depth
              )
              {
                break;
              }
            }
            continue;
          }
          if (componentClass == "highwayentrygate" || componentClass == "highwayexitgate")
          {
            long id = ParseId(xr.GetAttribute("id") ?? string.Empty);
            if (id <= 0)
            {
              continue;
            }
            int depth = xr.Depth;
            long connectedId = 0;
            while (xr.Read())
            {
              if (xr.NodeType == XmlNodeType.Element && string.Equals(xr.Name, "connected", StringComparison.Ordinal))
              {
                connectedId = ParseId(xr.GetAttribute("connection") ?? string.Empty);
                if (connectedId <= 0)
                {
                  continue;
                }
                break;
              }
              else if (
                xr.NodeType == XmlNodeType.EndElement
                && string.Equals(xr.Name, "component", StringComparison.Ordinal)
                && xr.Depth == depth
              )
              {
                break;
              }
            }
            if (connectedId <= 0)
            {
              continue;
            }
            SuperHighway? sh = null;
            if (componentClass == "highwayentrygate")
            {
              sh = superhighwayBuffer.FirstOrDefault(h => h.EntryGate == connectedId);
              if (sh != null)
              {
                sh.SectorFrom = currentSector;
              }
              else
              {
                // Gate first, highway later - remember gate for later processing
                gatesBuffer[connectedId] = new HighWayGate
                {
                  Id = connectedId,
                  Sector = currentSector,
                  Type = componentClass,
                };
              }
            }
            else
            {
              sh = superhighwayBuffer.FirstOrDefault(h => h.ExitGate == connectedId);
              if (sh != null)
              {
                sh.SectorTo = currentSector;
              }
              else
              {
                // Gate first, highway later - remember gate for later processing
                gatesBuffer[connectedId] = new HighWayGate
                {
                  Id = connectedId,
                  Sector = currentSector,
                  Type = componentClass,
                };
              }
            }
            if (sh != null && sh.IsReady)
            {
              insertSuperhighway.Parameters["@id"].Value = sh.Id;
              insertSuperhighway.Parameters["@macro"].Value = sh.Macro;
              insertSuperhighway.Parameters["@sector_from"].Value = sh.SectorFrom;
              insertSuperhighway.Parameters["@entrygate"].Value = sh.EntryGate;
              insertSuperhighway.Parameters["@sector_to"].Value = sh.SectorTo;
              insertSuperhighway.Parameters["@exitgate"].Value = sh.ExitGate;
              insertSuperhighway.ExecuteNonQuery();
              itemsForTransaction++;
              superhighwaysProcessed++;
              progress?.Invoke(new ProgressUpdate { SuperhighwaysProcessed = superhighwaysProcessed });
              if (traceEnabled && superhighwaysProcessed % 5 == 0)
              {
                LoggingService.Debug(
                  $"Superhighways processed: {superhighwaysProcessed}. Latest macro {sh.Macro} from {sh.SectorFrom} to {sh.SectorTo}.",
                  area: LogArea
                );
              }
              superhighwayBuffer.Remove(sh);
            }
            continue;
          }
          if (!(componentClass == "station") && !componentClass.StartsWith("ship_"))
          {
            continue;
          }
          try
          {
            string owner = xr.GetAttribute("owner") ?? "";
            long currentId = ProcessStationOrShip(owner, componentClass);
            if (currentId == 0)
            {
              // skip further processing for this component
              continue;
            }
            int depth = xr.Depth;
            bool stationNameSet = false;
            while (xr.Read())
            {
              if (xr.NodeType == XmlNodeType.Element)
              {
                if (detectNameViaProduction && xr.Name == "production" && currentStation != null)
                {
                  string product = xr.GetAttribute("originalproduct") ?? string.Empty;
                  if (!string.IsNullOrWhiteSpace(product))
                  {
                    currentStation.Name = factoryNames.TryGetValue(product, out var n) ? n : product;
                    stationNameSet = true;
                  }
                  else
                  {
                    continue;
                  }
                }
                string currentClass = xr.GetAttribute("class") ?? string.Empty;
                if (detectNameViaProduction && xr.Name == "component" && currentClass == "production" && currentStation != null)
                {
                  // Use production macro to detect station name
                  string productionMacro = xr.GetAttribute("macro") ?? string.Empty;
                  if (!string.IsNullOrWhiteSpace(productionMacro))
                  {
                    var macroParts = productionMacro.Split('_');
                    if (macroParts.Length == 4)
                    {
                      currentStation.Name = factoryNames.TryGetValue(macroParts[2], out var n) ? n : productionMacro;
                    }
                    else
                    {
                      currentStation.Name = productionMacro;
                    }
                    currentStation.Macro = productionMacro;
                    stationNameSet = true;
                  }
                  else
                  {
                    continue;
                  }
                }
                if (detectNameViaProduction && stationNameSet && currentStation != null)
                {
                  try
                  {
                    // Insert the station now
                    insertComp.Parameters["@id"].Value = currentStation.Id;
                    insertComp.Parameters["@type"].Value = currentStation.Type;
                    insertComp.Parameters["@class"].Value = currentStation.ComponentClass;
                    insertComp.Parameters["@macro"].Value = currentStation.Macro;
                    insertComp.Parameters["@sector"].Value = currentStation.Sector;
                    insertComp.Parameters["@owner"].Value = currentStation.Owner;
                    insertComp.Parameters["@code"].Value = currentStation.Code;
                    insertComp.Parameters["@nameindex"].Value = currentStation.NameIndex;
                    insertComp.Parameters["@name"].Value = currentStation.Name;
                    insertComp.ExecuteNonQuery();
                    itemsForTransaction++;
                    stationsProcessed++;
                    if (stationsProcessed % 50 == 0)
                    {
                      progress?.Invoke(new ProgressUpdate { StationsProcessed = stationsProcessed });
                    }
                  }
                  finally
                  {
                    detectNameViaProduction = false;
                    stationNameSet = false;
                    currentStation = null;
                  }
                  break;
                }

                if (xr.Name == "component" && currentClass.StartsWith("ship_", StringComparison.Ordinal))
                {
                  string shipOwner = xr.GetAttribute("owner") ?? "";
                  if (shipOwner != "player")
                  {
                    continue;
                  }
                  // process docked ship
                  long shipId = ProcessStationOrShip(shipOwner, currentClass);
                  if (shipId == 0)
                  {
                    // skip further processing for this component
                    continue;
                  }
                  int depthShip = xr.Depth;
                  while (xr.Read())
                  {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                      ProcessSubordinates(shipId);
                    }
                    else if (
                      xr.NodeType == XmlNodeType.EndElement
                      && string.Equals(xr.Name, "component", StringComparison.Ordinal)
                      && xr.Depth == depthShip
                    )
                    {
                      break;
                    }
                  }
                }
                if (owner != "player")
                {
                  continue;
                }
                ProcessSubordinates(currentId);
              }
              else if (
                xr.NodeType == XmlNodeType.EndElement
                && string.Equals(xr.Name, "component", StringComparison.Ordinal)
                && xr.Depth == depth
              )
              {
                break;
              }
            }
            detectNameViaProduction = false;
            currentStation = null;
          }
          catch
          {
            // skip malformed component entries
          }
          continue;
        }
        if (removedEntries && xr.Name == "object")
        {
          long id = ParseId(xr.GetAttribute("id") ?? string.Empty);
          if (id <= 0)
          {
            continue;
          }
          long next = ParseId(xr.GetAttribute("next") ?? string.Empty);
          if (next > 0)
          {
            // skip objects that were removed but replaced by another object
            GameComponent nextComp = GetComponentById(next);
            if (nextComp.Type == "ship" && skipShipByType(nextComp.Macro, shipTypes))
            {
              continue;
            }
            if (nextComp != null && nextComp.Id > 0)
            {
              insertComp.Parameters["@id"].Value = id;
              insertComp.Parameters["@type"].Value = nextComp.Type;
              insertComp.Parameters["@class"].Value = nextComp.ComponentClass;
              insertComp.Parameters["@macro"].Value = nextComp.Macro;
              insertComp.Parameters["@sector"].Value = nextComp.Sector;
              insertComp.Parameters["@owner"].Value = nextComp.Owner;
              insertComp.Parameters["@code"].Value = nextComp.Code;
              insertComp.Parameters["@nameindex"].Value = nextComp.NameIndex;
              insertComp.Parameters["@name"].Value = nextComp.Name;
              insertComp.ExecuteNonQuery();
              itemsForTransaction++;
              if (ConfigurationService.Instance.LoadRemovedObjects)
              {
                if (nextComp.Type == "station")
                {
                  stationsProcessed++;
                  progress?.Invoke(new ProgressUpdate { StationsProcessed = stationsProcessed });
                }
                else if (nextComp.Type == "ship")
                {
                  shipsProcessed++;
                  progress?.Invoke(new ProgressUpdate { ShipsProcessed = shipsProcessed });
                }
              }
            }
            continue;
          }
          else if (!ConfigurationService.Instance.LoadRemovedObjects)
          {
            continue;
          }
          string type = "removed";
          string owner = xr.GetAttribute("owner") ?? "";
          string name = xr.GetAttribute("name") ?? "";
          if (string.IsNullOrWhiteSpace(name))
          {
            continue;
          }
          string factionShortName = factionsShortNames.TryGetValue(owner, out var fsn) ? fsn : string.Empty;
          if (!string.IsNullOrEmpty(factionShortName) && name.StartsWith(factionShortName + " "))
          {
            name = name[factionShortName.Length..].TrimStart();
          }
          string code = xr.GetAttribute("code") ?? "";
          if (string.IsNullOrWhiteSpace(code))
          {
            continue;
          }
          long space = ParseId(xr.GetAttribute("space") ?? string.Empty);
          if (space <= 0 || !zonesToSectors.TryGetValue(space, out var sector) || string.IsNullOrEmpty(sector))
          {
            continue;
          }
          insertComp.Parameters["@id"].Value = id;
          insertComp.Parameters["@type"].Value = type;
          insertComp.Parameters["@class"].Value = type;
          insertComp.Parameters["@macro"].Value = "";
          insertComp.Parameters["@sector"].Value = sector;
          insertComp.Parameters["@owner"].Value = owner;
          insertComp.Parameters["@code"].Value = code;
          insertComp.Parameters["@nameindex"].Value = "";
          insertComp.Parameters["@name"].Value = name;
          insertComp.ExecuteNonQuery();
          itemsForTransaction++;
          removedCount++;
          if (removedCount % 100 == 0)
          {
            progress?.Invoke(new ProgressUpdate { RemovedProcessed = removedCount });
          }
          if (traceEnabled && removedCount % 200 == 0)
          {
            LoggingService.Debug($"Removed objects recorded: {removedCount}. Latest code {code} in sector {sector}.", area: LogArea);
          }
          continue;
        }
        if (tradeEntries && xr.Name == "log")
        {
          if (xr.GetAttribute("type") != "trade")
            continue;
          try
          {
            long seller = ParseId(xr.GetAttribute("seller") ?? string.Empty);
            if (seller <= 0)
            {
              continue;
            }
            long buyer = ParseId(xr.GetAttribute("buyer") ?? string.Empty);
            if (buyer <= 0)
            {
              continue;
            }
            long time = NormalizeTime(xr.GetAttribute("time") ?? string.Empty);
            if (time <= 0)
            {
              continue;
            }
            string ware = xr.GetAttribute("ware") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(ware))
            {
              continue;
            }
            long price = long.Parse(xr.GetAttribute("price") ?? string.Empty, CultureInfo.InvariantCulture);
            if (price <= 0)
            {
              continue;
            }
            long volume = long.Parse(xr.GetAttribute("v") ?? string.Empty, CultureInfo.InvariantCulture);
            if (volume <= 0)
            {
              continue;
            }
            insertTrade.Parameters["@seller"].Value = seller;
            insertTrade.Parameters["@buyer"].Value = buyer;
            insertTrade.Parameters["@ware"].Value = ware;
            insertTrade.Parameters["@price"].Value = price;
            insertTrade.Parameters["@volume"].Value = volume;
            insertTrade.Parameters["@time"].Value = time - gameTime;
            insertTrade.ExecuteNonQuery();
            tradeCount++;
            itemsForTransaction++;
            if (traceEnabled && tradeCount % 500 == 0)
            {
              LoggingService.Debug(
                $"Trades processed: {tradeCount}. Latest seller {seller}, buyer {buyer}, ware {ware}, volume {volume}.",
                area: LogArea
              );
            }
            if (tradeCount % 100 == 0)
            {
              progress?.Invoke(new ProgressUpdate { TradesProcessed = tradeCount });
            }
          }
          catch
          {
            // skip malformed trade entries
          }
          continue;
        }
      }

      if (xr.NodeType == XmlNodeType.EndElement)
      {
        if (
          (itemsForTransaction > 0) && (itemsForTransaction % _batchSize == 0)
          || !connectionsProcessed && xr.Name == "universe"
          || (!tradesProcessed && tradeEntries && tradeCount > 0 && xr.Name == "entries")
          || (removedEntries && xr.Name == "removed")
        )
        {
          txn.Commit();
          txn.Dispose();
          txn = _conn.BeginTransaction();
          // rebind commands to the new transaction
          insertComp.Transaction = txn;
          insertTrade.Transaction = txn;
          if (itemsForTransaction > 0)
          {
            itemsForTransaction = 0;
          }
          if (tradeCount > 0 && tradeEntries && xr.Name == "entries")
          {
            tradeEntries = false;
            tradesProcessed = true;
            LoggingService.Debug("Completed trade entries import phase.", area: LogArea);
            progress?.Invoke(new ProgressUpdate { TradesProcessed = tradeCount });
          }
          if (!connectionsProcessed && xr.Name == "universe")
          {
            connectionsProcessed = true;
            LoggingService.Debug("Finished universe component import phase.", area: LogArea);
            progress?.Invoke(new ProgressUpdate { StationsProcessed = stationsProcessed });
            progress?.Invoke(new ProgressUpdate { ShipsProcessed = shipsProcessed });
            progress?.Invoke(new ProgressUpdate { SubordinatesProcessed = subordinatesProcessed });
            progress?.Invoke(new ProgressUpdate { SectorsProcessed = sectorCount });
            progress?.Invoke(new ProgressUpdate { GatesProcessed = gatesProcessed });
            progress?.Invoke(new ProgressUpdate { SuperhighwaysProcessed = superhighwaysProcessed });
          }
          if (removedEntries && xr.Name == "removed")
          {
            removedProcessed = true;
            removedEntries = false;
            LoggingService.Debug("Completed removed objects import phase.", area: LogArea);
            progress?.Invoke(new ProgressUpdate { RemovedProcessed = removedCount });
          }
        }
      }
    }

    txn.Commit();

    // Final report
    progress?.Invoke(
      new ProgressUpdate
      {
        ElementsProcessed = (int)Math.Min(int.MaxValue, elementsProcessed),
        StationsProcessed = stationsProcessed,
        ShipsProcessed = shipsProcessed,
        SectorsProcessed = sectorCount,
        RemovedProcessed = removedCount,
        TradesProcessed = tradeCount,
        Status = "Save import complete.",
      }
    );

    var importElapsed = DateTime.Now - startTime;
    LoggingService.Information(
      $"Imported {shipsProcessed} ships, {stationsProcessed} stations, {tradeCount} trades, {removedCount} removed entries, {gatesProcessed} gates, {superhighwaysProcessed} superhighways, {subordinatesProcessed} subordinate links in {importElapsed}.",
      area: LogArea
    );
    RefreshStats();
    LoggingService.Debug("Database statistics refreshed after import.", area: LogArea);
  }

  public void RefreshStats()
  {
    try
    {
      // trade count
      Stats.TradesCount = ExecuteScalarInt("SELECT COUNT(1) FROM trade");
      // gates count (connections) if gate table present
      Stats.GatesCount = TableExists("gate") ? ExecuteScalarInt("SELECT COUNT(1) FROM gate") : 0;
      // subordinate relations count
      Stats.SubordinateCount = TableExists("subordinate") ? ExecuteScalarInt("SELECT COUNT(1) FROM subordinate") : 0;
      // player ships via view (if exists)
      Stats.PlayerShipsCount = ViewExists("player_ships")
        ? ExecuteScalarInt("SELECT COUNT(1) FROM player_ships")
        : ExecuteScalarInt("SELECT COUNT(1) FROM component WHERE type='ship' AND owner='player'");
      // stations via view (if exists)
      Stats.StationsCount = ViewExists("stations")
        ? ExecuteScalarInt("SELECT COUNT(1) FROM stations")
        : ExecuteScalarInt("SELECT COUNT(1) FROM component WHERE type='station'");
      // removed objects count (from save import) — optionally ignored
      Stats.RemovedObjectCount = ExecuteScalarInt("SELECT COUNT(1) FROM component WHERE type='removed'");
      // wares table may not exist in this project schema; guard it
      Stats.WaresCount = TableExists("ware") ? ExecuteScalarInt("SELECT COUNT(1) FROM ware") : 0;
      // factions count if table exists
      Stats.FactionsCount = TableExists("faction") ? ExecuteScalarInt("SELECT COUNT(1) FROM faction") : 0;
      // cluster/sector names count if table exists
      Stats.ClusterSectorNamesCount = TableExists("cluster_sector_name") ? ExecuteScalarInt("SELECT COUNT(1) FROM cluster_sector_name") : 0;

      // storages related counts if tables exist
      Stats.StoragesCount = TableExists("storage") ? ExecuteScalarInt("SELECT COUNT(1) FROM storage") : 0;
      Stats.ShipStoragesCount = TableExists("ship_storage") ? ExecuteScalarInt("SELECT COUNT(1) FROM ship_storage") : 0;

      // ship types count
      Stats.ShipTypesCount = TableExists("ship_type") ? ExecuteScalarInt("SELECT COUNT(1) FROM ship_type") : 0;

      // languages present in text table
      if (TableExists("text"))
      {
        Stats.LanguagesCount = ExecuteScalarInt("SELECT COUNT(DISTINCT language) FROM text");

        // Always read the current language id from settings (0 if missing)
        int currentLang = ExecuteScalarInt("SELECT current_language FROM settings LIMIT 1");
        Stats.CurrentLanguageId = currentLang;

        // Count texts for the current language if available
        if (ViewExists("lang"))
        {
          Stats.CurrentLanguageTextCount = ExecuteScalarInt("SELECT COUNT(1) FROM lang");
        }
        else
        {
          Stats.CurrentLanguageTextCount = ExecuteScalarInt($"SELECT COUNT(1) FROM text WHERE language = {currentLang}");
        }
      }
      else
      {
        Stats.LanguagesCount = 0;
        Stats.CurrentLanguageTextCount = 0;
        Stats.CurrentLanguageId = 0;
      }
    }
    catch
    {
      // Ignore stats errors; keep previous values
    }
  }

  bool TableExists(string table)
  {
    ReOpenConnection();
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name=@n";
    cmd.Parameters.AddWithValue("@n", table);
    var o = cmd.ExecuteScalar();
    return Convert.ToInt32(o, CultureInfo.InvariantCulture) > 0;
  }

  bool ViewExists(string view)
  {
    ReOpenConnection();
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='view' AND name=@n";
    cmd.Parameters.AddWithValue("@n", view);
    var o = cmd.ExecuteScalar();
    return Convert.ToInt32(o, CultureInfo.InvariantCulture) > 0;
  }

  int ExecuteScalarInt(string sql)
  {
    ReOpenConnection();
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = sql;
    var o = cmd.ExecuteScalar();
    return o == null || o is DBNull ? 0 : Convert.ToInt32(o, CultureInfo.InvariantCulture);
  }

  static long ParseId(string raw)
  {
    if (string.IsNullOrWhiteSpace(raw))
      return 0;
    if (raw.StartsWith('[') && raw.EndsWith(']'))
      raw = raw.Trim('[', ']');
    if (string.IsNullOrWhiteSpace(raw))
      return 0;
    try
    {
      if (raw.StartsWith("0x"))
      {
        raw = raw.Substring(2);
        return long.Parse(raw, NumberStyles.HexNumber);
      }
      else
      {
        return long.Parse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture);
      }
    }
    catch
    {
      return 0;
    }
  }

  static long NormalizeTime(string raw)
  {
    if (string.IsNullOrWhiteSpace(raw))
      return 0;
    if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
      return 0;
    return (long)Math.Round(f * 1000); // ms ticks
  }

  private (Dictionary<string, string>, HashSet<int>) GetProductionWareDict()
  {
    ReOpenConnection();
    var dict = new Dictionary<string, string>();
    var uniquePages = new HashSet<int>();

    using var cmd = new SQLiteCommand(
      @"
    SELECT id, name
    FROM ware
    WHERE id NOT LIKE '%\_%' ESCAPE '\'
      AND id <> 'credits'
      AND id <> 'crew';",
      _conn
    );

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
      string id = reader.GetString(0);
      string name = reader.GetString(1);
      (int textPage, int textId) = ParseTextItem(name);
      if (textPage > 0 && textId > 0)
      {
        string value = $"{textPage}:{textId + 3}";
        dict[id] = value;
        uniquePages.Add(textPage);
      }
    }
    return (dict, uniquePages);
  }

  private Dictionary<string, string> GetTextDict(
    List<int> pages,
    Dictionary<string, string>? existingDict = null,
    bool? newConnection = null
  )
  {
    SQLiteConnection? conn;
    if (newConnection == true)
    {
      conn = CreateConnection();
    }
    else
    {
      ReOpenConnection();
      conn = _conn;
    }
    var dict = existingDict ?? new Dictionary<string, string>();

    // Build parameter placeholders dynamically
    var placeholders = string.Join(",", pages.Select((_, i) => $"@p{i}"));

    using var cmd = new SQLiteCommand($"SELECT page, id, text FROM lang WHERE page IN ({placeholders});", conn);

    // Add parameters
    for (int i = 0; i < pages.Count; i++)
    {
      cmd.Parameters.AddWithValue($"@p{i}", pages[i]);
    }

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
      int page = reader.GetInt32(0);
      int id = reader.GetInt32(1);
      string text = reader.GetString(2);

      string key = $"{page}:{id}";
      dict[key] = text;
    }
    return dict;
  }

  private Dictionary<string, string> GetFactoryNamesDict()
  {
    ReOpenConnection();
    var (prodDict, uniquePages) = GetProductionWareDict();
    var dict = GetTextDict(uniquePages.ToList());
    foreach (var (wareId, prodKey) in prodDict)
    {
      if (dict.TryGetValue(prodKey, out var name))
      {
        prodDict[wareId] = name;
      }
    }
    return prodDict;
  }

  private Dictionary<string, string> GetFactionsShortNamesDict()
  {
    ReOpenConnection();
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "SELECT id, shortname FROM faction WHERE shortname IS NOT NULL AND shortname != ''";
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
      string id = rdr.GetString(0);
      string shortname = rdr.GetString(1);
      if (!dict.ContainsKey(id))
      {
        dict[id] = shortname;
      }
    }
    return dict;
  }

  private Dictionary<string, string> GetShipTypesDict()
  {
    ReOpenConnection();
    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    using var cmd = _conn.CreateCommand();
    cmd.CommandText = "SELECT macro, type FROM ship_type";
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
      string macro = rdr.GetString(0);
      string type = rdr.GetString(1);
      if (!dict.ContainsKey(macro))
      {
        dict[macro] = type;
      }
    }
    return dict;
  }

  private static bool skipShipByType(string? macro, Dictionary<string, string> shipTypes)
  {
    if (macro != null && shipTypes.TryGetValue(macro, out var type))
    {
      return type.Contains("drone", StringComparison.OrdinalIgnoreCase)
        || type.Equals("personalvehicle", StringComparison.OrdinalIgnoreCase)
        || type.Equals("lasertower", StringComparison.OrdinalIgnoreCase);
    }
    return false;
  }
}
