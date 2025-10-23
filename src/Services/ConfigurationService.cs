using System;
using System.IO;
using System.Text.Json;

namespace X4PlayerShipTradeAnalyzer.Services;

public sealed class ConfigurationService
{
  private static readonly Lazy<ConfigurationService> _lazy = new(() => new ConfigurationService());
  public static ConfigurationService Instance => _lazy.Value;

  private readonly string _configPath;
  private JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

  private ConfigurationService()
  {
    // Store <exeName>.json next to the executable for simplicity
    var baseDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
    var exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath) ?? "config";
    _configPath = Path.Combine(baseDir, exeName + ".json");
    Load();
  }

  public string? GameFolderExePath { get; set; }
  public string? GameSavePath { get; set; }
  public bool LoadOnlyGameLanguage { get; set; } = true;
  public bool LoadRemovedObjects { get; set; }
  public string AppTheme { get; set; } = "System"; // System | Light | Dark
  public AutoReloadGameSaveMode AutoReloadMode { get; set; } = AutoReloadGameSaveMode.None; // None | SelectedFile | AnyFile
  public bool CheckForUpdatesOnStartup { get; set; }
  public bool EnableFileLogging { get; set; }
  public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
  public bool IncludeAvaloniaLogs { get; set; }

  public void Save()
  {
    var dto = new PersistedConfig
    {
      GameFolderExePath = GameFolderExePath,
      GameSavePath = GameSavePath,
      LoadOnlyGameLanguage = LoadOnlyGameLanguage,
      LoadRemovedObjects = LoadRemovedObjects,
      AppTheme = AppTheme,
      AutoReloadMode = AutoReloadMode,
      CheckForUpdatesOnStartup = CheckForUpdatesOnStartup,
      EnableFileLogging = EnableFileLogging,
      MinimumLogLevel = MinimumLogLevel.ToString(),
      IncludeAvaloniaLogs = IncludeAvaloniaLogs,
    };
    var json = JsonSerializer.Serialize(dto, _jsonSerializerOptions);
    File.WriteAllText(_configPath, json);
  }

  private void Load()
  {
    try
    {
      if (!File.Exists(_configPath))
        return;
      var json = File.ReadAllText(_configPath);
      var dto = JsonSerializer.Deserialize<PersistedConfig>(json, _jsonSerializerOptions);
      GameFolderExePath = dto?.GameFolderExePath;
      GameSavePath = dto?.GameSavePath;
      LoadOnlyGameLanguage = dto?.LoadOnlyGameLanguage ?? true; // default to true for backward compatibility
      LoadRemovedObjects = dto?.LoadRemovedObjects ?? false; // default to false
      AppTheme = string.IsNullOrWhiteSpace(dto?.AppTheme) ? "System" : dto!.AppTheme!;
      AutoReloadMode = dto?.AutoReloadMode ?? AutoReloadGameSaveMode.None; // default None
      CheckForUpdatesOnStartup = dto?.CheckForUpdatesOnStartup ?? false;
      EnableFileLogging = dto?.EnableFileLogging ?? false;
      IncludeAvaloniaLogs = dto?.IncludeAvaloniaLogs ?? false;
      if (dto?.MinimumLogLevel != null && Enum.TryParse<LogLevel>(dto.MinimumLogLevel, true, out var parsedLevel))
      {
        MinimumLogLevel = parsedLevel;
      }
      else
      {
        MinimumLogLevel = LogLevel.Warning;
      }
    }
    catch
    {
      // ignore malformed config
    }
  }

  private sealed class PersistedConfig
  {
    public string? GameFolderExePath { get; set; }
    public string? GameSavePath { get; set; }
    public bool? LoadOnlyGameLanguage { get; set; }
    public bool? LoadRemovedObjects { get; set; }
    public string? AppTheme { get; set; }
    public AutoReloadGameSaveMode? AutoReloadMode { get; set; }
    public bool? CheckForUpdatesOnStartup { get; set; }
    public bool? EnableFileLogging { get; set; }
    public string? MinimumLogLevel { get; set; }
    public bool? IncludeAvaloniaLogs { get; set; }
  }
}
