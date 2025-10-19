using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace X4PlayerShipTradeAnalyzer.Services;

public static class UpdateChecker
{
  private const string UserAgent = "X4PlayerShipTradeAnalyzer-UpdateChecker/1.0 (+https://github.com/chemodun/X4PlayerShipTradeAnalyzer)";

  public static async Task<UpdateCheckResult> CheckForUpdatesAsync(
    string githubRepoUrl,
    Assembly assembly,
    CancellationToken cancellationToken
  )
  {
    if (string.IsNullOrWhiteSpace(githubRepoUrl))
    {
      return UpdateCheckResult.CreateFailure("Repository or project metadata is missing.");
    }

    var currentVersion = assembly?.GetName().Version ?? new Version(0, 0, 0, 0);
    var currentVersionString = FormatVersion(currentVersion);

    try
    {
      var latestVersionString = await GetLatestVersionAsync(githubRepoUrl, cancellationToken).ConfigureAwait(false);

      if (string.IsNullOrWhiteSpace(latestVersionString))
      {
        return UpdateCheckResult.CreateFailure($"Could not determine the latest version.", currentVersionString);
      }

      if (!VersionTryParseFlexible(latestVersionString!, out var latestVersion))
      {
        return new UpdateCheckResult(false, false, currentVersionString, latestVersionString, $"Latest version: {latestVersionString}");
      }

      var comparison = CompareVersionsSafe(currentVersion, latestVersion);
      if (comparison < 0)
      {
        return new UpdateCheckResult(
          true,
          true,
          currentVersionString,
          FormatVersion(latestVersion),
          $"New version available: {FormatVersion(latestVersion)}"
        );
      }

      return new UpdateCheckResult(true, false, currentVersionString, FormatVersion(latestVersion), $"You are on the latest version.");
    }
    catch (OperationCanceledException)
    {
      return UpdateCheckResult.CreateFailure("Update check timed out.", currentVersionString);
    }
    catch (Exception ex)
    {
      return UpdateCheckResult.CreateFailure($"Failed to check updates: {ex.Message}", currentVersionString);
    }
  }

  public static void OpenUrlInBrowser(string url)
  {
    if (string.IsNullOrWhiteSpace(url))
      return;

    try
    {
      var psi = new ProcessStartInfo(url) { UseShellExecute = true };
      Process.Start(psi);
    }
    catch
    {
      // ignore failures to launch browser
    }
  }

  private static async Task<string?> GetLatestVersionAsync(string githubRepoUrl, CancellationToken ct)
  {
    if (!TryParseOwnerRepo(githubRepoUrl, out var owner, out var repo))
    {
      return null;
    }

    var api = $"https://api.github.com/repos/{owner}/{repo}/releases?per_page=100";

    using var http = CreateGitHubHttpClient();
    using var response = await http.GetAsync(api, ct).ConfigureAwait(false);
    if (!response.IsSuccessStatusCode)
      return null;

    await using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
    using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);
    if (doc.RootElement.ValueKind != JsonValueKind.Array)
      return null;

    string? bestVersion = null;
    Version? best = null;

    foreach (var release in doc.RootElement.EnumerateArray())
    {
      if (release.TryGetProperty("draft", out var draftProp) && draftProp.GetBoolean())
        continue;
      if (release.TryGetProperty("prerelease", out var preProp) && preProp.GetBoolean())
        continue;

      if (!release.TryGetProperty("tag_name", out var tagProp))
        continue;

      var tag = tagProp.GetString() ?? string.Empty;
      if (!TryExtractVersionFromTag(tag, out var versionString))
        continue;

      if (!VersionTryParseFlexible(versionString, out var version))
        continue;

      if (best == null || CompareVersionsSafe(version, best) > 0)
      {
        best = version;
        bestVersion = FormatVersion(version);
      }
    }

    return bestVersion;
  }

  private static HttpClient CreateGitHubHttpClient()
  {
    var handler = new HttpClientHandler
    {
      AllowAutoRedirect = true,
      AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
      UseCookies = false,
    };

    var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };

    client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
    return client;
  }

  private static bool TryParseOwnerRepo(string url, out string owner, out string repo)
  {
    owner = string.Empty;
    repo = string.Empty;

    try
    {
      var uri = new Uri(url);
      if (!string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase))
        return false;

      var segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

      if (segments.Length < 2)
        return false;

      owner = segments[0];
      repo = segments[1];
      return true;
    }
    catch
    {
      return false;
    }
  }

  private static bool TryExtractVersionFromTag(string tag, out string version)
  {
    version = string.Empty;
    if (string.IsNullOrWhiteSpace(tag))
      return false;

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    var match = Regex.Match(tag.Trim(), @"\b[vV]?\d+\.\d+\.\d+(?:\.\d+)?\b");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    if (match.Success)
    {
      version = match.Value.TrimStart('v', 'V');
      return true;
    }

    return false;
  }

  private static string FormatVersion(Version version)
  {
    var parts = new[] { version.Major, version.Minor, version.Build };
    return string.Join('.', parts);
  }

  public static bool VersionTryParseFlexible(string text, out Version version)
  {
    version = new Version(0, 0, 0, 0);
    if (string.IsNullOrWhiteSpace(text))
      return false;

    var cleaned = text.Trim().TrimStart('v', 'V').Replace('_', '.');
    var segments = cleaned.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (segments.Length == 0)
      return false;

    var numbers = segments
      .Select(segment => int.TryParse(new string(segment.TakeWhile(char.IsDigit).ToArray()), out var n) ? n : 0)
      .Take(4)
      .ToList();

    while (numbers.Count < 2)
      numbers.Add(0);
    while (numbers.Count < 4)
      numbers.Add(0);

    try
    {
      version = new Version(numbers[0], numbers[1], numbers[2], numbers[3]);
      return true;
    }
    catch
    {
      return false;
    }
  }

  private static int CompareVersionsSafe(Version a, Version b)
  {
    var cmp = a.Major.CompareTo(b.Major);
    if (cmp != 0)
      return cmp;

    cmp = a.Minor.CompareTo(b.Minor);
    if (cmp != 0)
      return cmp;

    cmp = a.Build.CompareTo(b.Build);
    if (cmp != 0)
      return cmp;

    return a.Revision.CompareTo(b.Revision);
  }

  public sealed record UpdateCheckResult(bool Success, bool HasUpdate, string CurrentVersion, string? LatestVersion, string Message)
  {
    public static UpdateCheckResult CreateFailure(string message, string? currentVersion = null) =>
      new(false, false, currentVersion ?? string.Empty, null, message);
  }
}
