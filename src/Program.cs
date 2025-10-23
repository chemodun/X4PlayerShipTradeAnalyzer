using Avalonia;
using X4PlayerShipTradeAnalyzer.Services;

namespace X4PlayerShipTradeAnalyzer;

internal static class Program
{
  public static void Main(string[] args)
  {
    var appBuilder = App.BuildAvaloniaApp();
    LoggingService.Initialize();
    appBuilder.StartWithClassicDesktopLifetime(args);
  }
}
