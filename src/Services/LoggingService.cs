using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Avalonia.Logging;

namespace X4PlayerShipTradeAnalyzer.Services;

public enum LogLevel
{
  Trace = 0,
  Debug = 1,
  Information = 2,
  Warning = 3,
  Error = 4,
  Critical = 5,
  None = 6,
}

public static class LoggingService
{
  private const string DefaultArea = "X4PlayerShipTradeAnalyzer";

  static readonly object _syncRoot = new();
  static readonly string _logDirectory;
  static string _currentDateStamp = string.Empty;
  static string _currentLogFilePath = string.Empty;
  static LogLevel _minimumLevel = LogLevel.Warning;
  static bool _enableFileLogging;
  static bool _includeAvaloniaLogs;
  static bool _initialized;
  static bool _sinkRegistered;
  static CompositeLogSink? _compositeSink;
  static AvaloniaLogSink? _avaloniaSink;
  static readonly AsyncLocal<bool> _isInternalLog = new();

  static LoggingService()
  {
    var baseDirectory = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
    _logDirectory = Path.Combine(baseDirectory, "Logs");
  }

  public static LogLevel MinimumLevel
  {
    get
    {
      EnsureInitialized();
      lock (_syncRoot)
      {
        return _minimumLevel;
      }
    }
    set => ApplyConfiguration(_enableFileLogging, value, _includeAvaloniaLogs);
  }

  public static void Initialize() => EnsureInitialized();

  public static void Initialize(bool enableFileLogging, LogLevel minimumLevel) =>
    ApplyConfiguration(enableFileLogging, minimumLevel, includeAvaloniaLogs: true);

  public static void Initialize(bool enableFileLogging, LogLevel minimumLevel, bool includeAvaloniaLogs) =>
    ApplyConfiguration(enableFileLogging, minimumLevel, includeAvaloniaLogs);

  public static void ApplyConfiguration(bool enableFileLogging, LogLevel minimumLevel, bool includeAvaloniaLogs)
  {
    bool levelChanged;
    bool fileChanged;
    bool avaloniaChanged;
    bool firstInitialization;

    lock (_syncRoot)
    {
      levelChanged = _minimumLevel != minimumLevel;
      fileChanged = _enableFileLogging != enableFileLogging;
      avaloniaChanged = _includeAvaloniaLogs != includeAvaloniaLogs;
      firstInitialization = !_initialized;

      _minimumLevel = minimumLevel;
      _enableFileLogging = enableFileLogging;
      _includeAvaloniaLogs = includeAvaloniaLogs;

      EnsureSinkRegistered_NoLock();
      _initialized = true;
    }

    if (firstInitialization)
    {
      WriteInternal(LogLevel.Information, DefaultArea, "Logging initialised.", null);
    }
    else
    {
      if (fileChanged)
      {
        WriteInternal(LogLevel.Information, DefaultArea, enableFileLogging ? "File logging enabled." : "File logging disabled.", null);
      }

      if (levelChanged)
      {
        WriteInternal(LogLevel.Information, DefaultArea, $"Minimum log level set to {minimumLevel}.", null);
      }

      if (avaloniaChanged)
      {
        WriteInternal(
          LogLevel.Information,
          DefaultArea,
          includeAvaloniaLogs ? "Avalonia framework logging enabled." : "Avalonia framework logging disabled.",
          null
        );
      }
    }
  }

  static void EnsureSinkRegistered_NoLock()
  {
    if (_sinkRegistered)
      return;

    _avaloniaSink ??= new AvaloniaLogSink();

    if (Logger.Sink is CompositeLogSink existingComposite)
    {
      existingComposite.Add(_avaloniaSink);
      _compositeSink = existingComposite;
    }
    else if (Logger.Sink is null)
    {
      _compositeSink = new CompositeLogSink(new[] { _avaloniaSink });
      Logger.Sink = _compositeSink;
    }
    else
    {
      _compositeSink = new CompositeLogSink(new[] { Logger.Sink, _avaloniaSink });
      Logger.Sink = _compositeSink;
    }

    _sinkRegistered = true;
  }

  public static void Trace(string message, Exception? exception = null, string area = DefaultArea, object? source = null) =>
    Log(LogLevel.Trace, message, exception, area, source);

  public static void Debug(string message, Exception? exception = null, string area = DefaultArea, object? source = null) =>
    Log(LogLevel.Debug, message, exception, area, source);

  public static void Information(string message, Exception? exception = null, string area = DefaultArea, object? source = null) =>
    Log(LogLevel.Information, message, exception, area, source);

  public static void Warning(string message, Exception? exception = null, string area = DefaultArea, object? source = null) =>
    Log(LogLevel.Warning, message, exception, area, source);

  public static void Error(string message, Exception? exception = null, string area = DefaultArea, object? source = null) =>
    Log(LogLevel.Error, message, exception, area, source);

  public static void Critical(string message, Exception? exception = null, string area = DefaultArea, object? source = null) =>
    Log(LogLevel.Critical, message, exception, area, source);

  public static void Log(LogLevel level, string message, Exception? exception = null, string area = DefaultArea, object? source = null)
  {
    EnsureInitialized();

    if (!ShouldLog(level))
      return;

    var previous = _isInternalLog.Value;
    _isInternalLog.Value = true;
    try
    {
      var avaloniaLevel = ToAvaloniaLevel(level);
      var logger = Logger.TryGet(avaloniaLevel, area);
      if (logger.HasValue)
      {
        if (exception != null)
        {
          logger.Value.Log(source, "{Message}", message, exception);
        }
        else
        {
          logger.Value.Log(source, "{Message}", message);
        }
      }
      else
      {
        WriteInternal(level, area, message, exception);
      }
    }
    finally
    {
      _isInternalLog.Value = previous;
    }
  }

  static void EnsureInitialized()
  {
    if (_initialized)
      return;

    var configuration = ConfigurationService.Instance;
    ApplyConfiguration(configuration.EnableFileLogging, configuration.MinimumLogLevel, configuration.IncludeAvaloniaLogs);
  }

  static bool ShouldLog(LogLevel level)
  {
    if (_minimumLevel == LogLevel.None)
      return false;
    return level >= _minimumLevel;
  }

  static LogEventLevel ToAvaloniaLevel(LogLevel level) =>
    level switch
    {
      LogLevel.Trace => LogEventLevel.Verbose,
      LogLevel.Debug => LogEventLevel.Debug,
      LogLevel.Information => LogEventLevel.Information,
      LogLevel.Warning => LogEventLevel.Warning,
      LogLevel.Error => LogEventLevel.Error,
      LogLevel.Critical => LogEventLevel.Fatal,
      _ => LogEventLevel.Information,
    };

  static LogLevel FromAvaloniaLevel(LogEventLevel level) =>
    level switch
    {
      LogEventLevel.Verbose => LogLevel.Trace,
      LogEventLevel.Debug => LogLevel.Debug,
      LogEventLevel.Information => LogLevel.Information,
      LogEventLevel.Warning => LogLevel.Warning,
      LogEventLevel.Error => LogLevel.Error,
      LogEventLevel.Fatal => LogLevel.Critical,
      _ => LogLevel.Information,
    };

  static void WriteInternal(LogLevel level, string area, string message, Exception? exception)
  {
    var timestamp = DateTime.Now;
    var dateStamp = timestamp.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    var builder = new StringBuilder();
    builder
      .Append('[')
      .Append(timestamp.ToString("O", CultureInfo.InvariantCulture))
      .Append("] [")
      .Append(level)
      .Append("] [")
      .Append(area)
      .Append("] ")
      .Append(message ?? string.Empty);
    if (exception != null)
    {
      builder.AppendLine();
      builder.Append(exception);
    }

    var entry = builder.ToString();

    lock (_syncRoot)
    {
      if (_enableFileLogging)
      {
        if (!string.Equals(dateStamp, _currentDateStamp, StringComparison.Ordinal))
        {
          _currentDateStamp = dateStamp;
          _currentLogFilePath = BuildLogFilePath(dateStamp);
        }

        Directory.CreateDirectory(_logDirectory);
        File.AppendAllText(_currentLogFilePath, entry + Environment.NewLine, Encoding.UTF8);
      }
    }

    if (Debugger.IsAttached)
    {
      System.Diagnostics.Debug.WriteLine(entry);
    }
  }

  static string BuildLogFilePath(string dateStamp)
  {
    var fileName = $"log_{dateStamp}.txt";
    return Path.Combine(_logDirectory, fileName);
  }

  sealed class AvaloniaLogSink : ILogSink
  {
    public bool IsEnabled(LogEventLevel level, string area) => ShouldCaptureAvaloniaLog() && ShouldLog(FromAvaloniaLevel(level));

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
      WriteEntry(level, area, messageTemplate, Array.Empty<object?>());
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
    {
      WriteEntry(level, area, messageTemplate, propertyValues);
    }

    static void WriteEntry(LogEventLevel level, string area, string messageTemplate, object?[]? propertyValues)
    {
      if (!ShouldCaptureAvaloniaLog())
        return;

      var logLevel = FromAvaloniaLevel(level);
      if (!ShouldLog(logLevel))
        return;

      Exception? exception = null;
      if (propertyValues != null)
      {
        exception = propertyValues.OfType<Exception>().FirstOrDefault();
      }

      string renderedMessage = RenderMessage(messageTemplate, propertyValues);
      WriteInternal(logLevel, area, renderedMessage, exception);
    }

    static string RenderMessage(string template, object?[]? propertyValues)
    {
      if (propertyValues == null || propertyValues.Length == 0)
        return template;

      if (template == "{Message}" && propertyValues[0] is string primary)
      {
        if (propertyValues.Length == 1)
          return primary;

        var builder = new StringBuilder(primary);
        for (int i = 1; i < propertyValues.Length; i++)
        {
          if (propertyValues[i] is Exception)
            continue;
          if (propertyValues[i] is string str && string.IsNullOrEmpty(str))
            continue;
          builder.Append(' ').Append(propertyValues[i]);
        }
        return builder.ToString();
      }

      var parts = new List<string>();
      if (!string.IsNullOrWhiteSpace(template))
        parts.Add(template);

      foreach (var value in propertyValues)
      {
        if (value is null || value is Exception)
          continue;
        var str = value.ToString();
        if (!string.IsNullOrWhiteSpace(str))
          parts.Add(str);
      }

      return string.Join(" | ", parts);
    }
  }

  static bool ShouldCaptureAvaloniaLog() => _includeAvaloniaLogs || _isInternalLog.Value;

  sealed class CompositeLogSink : ILogSink
  {
    readonly List<ILogSink> _sinks;

    public CompositeLogSink(IEnumerable<ILogSink?> sinks)
    {
      _sinks = sinks.Where(s => s != null).Cast<ILogSink>().ToList();
    }

    public void Add(ILogSink sink)
    {
      lock (_sinks)
      {
        if (!_sinks.Contains(sink))
        {
          _sinks.Add(sink);
        }
      }
    }

    public bool IsEnabled(LogEventLevel level, string area)
    {
      lock (_sinks)
      {
        return _sinks.Any(s => s.IsEnabled(level, area));
      }
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
      LogCore(level, area, source, messageTemplate, null);
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
    {
      LogCore(level, area, source, messageTemplate, propertyValues);
    }

    void LogCore(LogEventLevel level, string area, object? source, string messageTemplate, object?[]? propertyValues)
    {
      List<ILogSink> snapshot;
      lock (_sinks)
      {
        snapshot = _sinks.ToList();
      }

      foreach (var sink in snapshot)
      {
        if (!sink.IsEnabled(level, area))
          continue;

        if (propertyValues is null)
        {
          sink.Log(level, area, source, messageTemplate);
        }
        else
        {
          sink.Log(level, area, source, messageTemplate, propertyValues);
        }
      }
    }
  }
}
