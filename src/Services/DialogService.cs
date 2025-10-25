using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace X4PlayerShipTradeAnalyzer.Services;

public static class DialogService
{
  public static Task ShowInfoAsync(Window owner, string title, string message, string okText = "OK") =>
    ShowDialogAsync(owner, title, message, new DialogButton(okText, DialogResult.Positive, true));

  public static Task ShowErrorAsync(Window owner, string title, string message, string okText = "OK") =>
    ShowDialogAsync(owner, title, message, new DialogButton(okText, DialogResult.Negative, true));

  public static async Task<bool> ShowConfirmationAsync(
    Window owner,
    string title,
    string message,
    string confirmText = "Yes",
    string cancelText = "No"
  )
  {
    var result = await ShowDialogAsync(
        owner,
        title,
        message,
        new DialogButton(confirmText, DialogResult.Positive, true),
        new DialogButton(cancelText, DialogResult.Negative, false)
      )
      .ConfigureAwait(false);
    return result == DialogResult.Positive;
  }

  private static async Task<DialogResult?> ShowDialogAsync(Window owner, string title, string message, params DialogButton[] buttons)
  {
    if (buttons == null || buttons.Length == 0)
      throw new ArgumentException("At least one button is required.", nameof(buttons));

    var tcs = new TaskCompletionSource<DialogResult?>();

    var foreground = owner.Foreground as SolidColorBrush ?? new SolidColorBrush(Colors.Black);
    var background = owner.Background as SolidColorBrush ?? new SolidColorBrush(Colors.White);

    await Dispatcher.UIThread.InvokeAsync(() =>
    {
      var dialog = new Window
      {
        Title = title,
        CanResize = false,
        WindowStartupLocation = WindowStartupLocation.CenterOwner,
        SystemDecorations = SystemDecorations.BorderOnly,
        Padding = new Thickness(12),
        Background = Brushes.Transparent,
        SizeToContent = SizeToContent.WidthAndHeight,
        MaxWidth = 520,
      };

      var text = new TextBlock
      {
        Text = message,
        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
        MaxWidth = 440,
      };

      var buttonPanel = new StackPanel
      {
        Orientation = Orientation.Horizontal,
        HorizontalAlignment = HorizontalAlignment.Right,
        Spacing = 8,
      };

      foreach (var button in buttons)
      {
        var btn = new Button
        {
          Content = button.Text,
          MinWidth = 80,
          HorizontalAlignment = HorizontalAlignment.Right,
          HorizontalContentAlignment = HorizontalAlignment.Center,
          VerticalContentAlignment = VerticalAlignment.Center,
          IsDefault = button.IsDefault,
          IsCancel = !button.IsDefault && button.Result == DialogResult.Negative,
        };

        btn.Click += (_, _) =>
        {
          if (!tcs.Task.IsCompleted)
            tcs.TrySetResult(button.Result);
          dialog.Close();
        };

        buttonPanel.Children.Add(btn);
      }

      var contentPanel = new StackPanel { Orientation = Orientation.Vertical, Spacing = 12 };

      contentPanel.Children.Add(text);
      contentPanel.Children.Add(buttonPanel);

      var contentBorder = new Border
      {
        Background = background,
        BorderBrush = foreground,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(8),
        Padding = new Thickness(16),
        Margin = new Thickness(0),
        BoxShadow = new BoxShadows(
          new BoxShadow { Color = Color.FromArgb(100, foreground.Color.R, foreground.Color.G, foreground.Color.B), Blur = 20 }
        ),
        Child = contentPanel,
      };

      dialog.Content = contentBorder;

      dialog.Closed += (_, _) =>
      {
        if (!tcs.Task.IsCompleted)
          tcs.TrySetResult(null);
      };

      if (owner is null)
      {
        dialog.Show();
      }
      else
      {
        dialog.ShowDialog(owner);
      }
    });

    return await tcs.Task.ConfigureAwait(false);
  }

  private enum DialogResult
  {
    Positive,
    Negative,
  }

  private sealed record DialogButton(string Text, DialogResult Result, bool IsDefault);

  private static IBrush ResolveBrush(string key, IBrush fallback)
  {
    if (Application.Current != null && Application.Current.TryFindResource(key, out var resource) && resource is IBrush brush)
    {
      return brush;
    }

    return fallback;
  }
}
