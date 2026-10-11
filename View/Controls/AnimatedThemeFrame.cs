using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ActiBoard.View.Controls;

/// <summary>A quiet, non-interactive icon border for the built-in themes.</summary>
public sealed class AnimatedThemeFrame : FrameworkElement
{
    public static readonly DependencyProperty ThemeIdProperty = DependencyProperty.Register(
        nameof(ThemeId), typeof(string), typeof(AnimatedThemeFrame),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

    public string ThemeId
    {
        get => (string)GetValue(ThemeIdProperty);
        set => SetValue(ThemeIdProperty, value);
    }

    private readonly Stopwatch _clock = new();
    public static readonly DependencyProperty ShowFrameProperty = DependencyProperty.Register(
        nameof(ShowFrame), typeof(bool), typeof(AnimatedThemeFrame),
        new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
    public bool ShowFrame
    {
        get => (bool)GetValue(ShowFrameProperty);
        set => SetValue(ShowFrameProperty, value);
    }
    public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
        nameof(BackgroundColor), typeof(string), typeof(AnimatedThemeFrame),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));
    public string BackgroundColor
    {
        get => (string)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }
    private double _lastFrame;
    private bool _subscribed;

    public AnimatedThemeFrame()
    {
        Loaded += (_, _) => StartRendering();
        Unloaded += (_, _) => StopRendering();
        IsVisibleChanged += (_, _) =>
        {
            if (IsLoaded && IsVisible) StartRendering();
            else StopRendering();
        };
    }

    private void StartRendering()
    {
        if (_subscribed || !ShowFrame) return;
        _clock.Start();
        CompositionTarget.Rendering += RenderFrame;
        _subscribed = true;
    }

    private void StopRendering()
    {
        if (!_subscribed) return;
        CompositionTarget.Rendering -= RenderFrame;
        _clock.Stop();
        _subscribed = false;
    }

    private void RenderFrame(object? sender, EventArgs e)
    {
        // Throttle drawing and leave custom themes to their own backgrounds.
        if (GetTheme(ThemeId) is null || _clock.Elapsed.TotalSeconds - _lastFrame < 1.0 / 30) return;
        _lastFrame = _clock.Elapsed.TotalSeconds;
        InvalidateVisual();
    }

    private sealed record FrameTheme(string Background);

    private static readonly Dictionary<string, FrameTheme?> Themes = new();
    private static readonly Dictionary<string, SolidColorBrush> Brushes = new();
    private static readonly Dictionary<string, BitmapSource[]> Atlases = new();

    private static FrameTheme? GetTheme(string id)
    {
        if (!Themes.TryGetValue(id, out var theme))
            Themes[id] = theme = CreateTheme(id);
        return theme;
    }

    private static FrameTheme? CreateTheme(string id) => id switch
    {
        "default" => new("#F8F6F0"),
        "halloween" => new("#40304F"),
        "rekorduge" => new("#EFF5F8"),
        "fastelavn" => new("#FAF1EB"),
        "åbenthus" => new("#F1F6F4"),
        "juleklippedag" => new("#245C43"),
        _ => null
    };

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        var theme = GetTheme(ThemeId);
        if (ActualWidth < 100 || ActualHeight < 100) return;
        string color = string.IsNullOrWhiteSpace(BackgroundColor) ? theme?.Background ?? "" : BackgroundColor;
        if (!string.IsNullOrEmpty(color)) dc.DrawRectangle(Brush(color), null, new Rect(RenderSize));
        if (theme is null || !ShowFrame) return;
        var images = GetImages(ThemeId);
        double inset = 24, width = ActualWidth - 2 * inset, height = ActualHeight - 2 * inset;
        double perimeter = 2 * (width + height);
        int count = Math.Max(8, (int)(perimeter / 210));
        double phase = SystemParameters.ClientAreaAnimation ? _clock.Elapsed.TotalSeconds * 14 : 0;
        double iconSize = ActualWidth < 1000 ? 36 : 44;
        for (int i = 0; i < count; i++)
        {
            double d = (perimeter * i / count + phase) % perimeter;
            Point point = d < width ? new(d, 0)
                : d < width + height ? new(width, d - width)
                : d < 2 * width + height ? new(2 * width + height - d, height)
                : new(0, perimeter - d);
            dc.PushTransform(new TranslateTransform(point.X + inset - iconSize / 2, point.Y + inset - iconSize / 2));
            var image = images[i % images.Length];
            double scale = iconSize / Math.Max(image.PixelWidth, image.PixelHeight);
            double imageWidth = image.PixelWidth * scale, imageHeight = image.PixelHeight * scale;
            dc.DrawImage(image, new Rect((iconSize - imageWidth) / 2, (iconSize - imageHeight) / 2, imageWidth, imageHeight));
            dc.Pop();
        }
    }

    internal static BitmapSource[] GetImages(string themeId)
    {
        if (Atlases.TryGetValue(themeId, out var cached)) return cached;
        string fileName = themeId == "åbenthus" ? "aabenthus" : themeId;
        var source = new BitmapImage();
        source.BeginInit();
        source.CacheOption = BitmapCacheOption.OnLoad;
        source.UriSource = new Uri($"pack://application:,,,/ActiBoard;component/Images/ThemeFrames/{fileName}.png");
        source.EndInit();
        source.Freeze();
        var rgba = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
        int width = rgba.PixelWidth, height = rgba.PixelHeight;
        var pixels = new byte[width * height * 4];
        rgba.CopyPixels(pixels, width * 4, 0);
        // Find the transparent gaps near each cell boundary, rather than cutting a motif.
        int[] cuts = [0, 0, 0, 0, width];
        for (int cell = 1; cell < 4; cell++)
        {
            int ideal = width * cell / 4, radius = width / 15, best = int.MaxValue;
            cuts[cell] = ideal;
            for (int distance = 0; distance <= radius; distance++)
                foreach (int x in new[] { ideal - distance, ideal + distance })
                {
                    int ink = 0;
                    for (int y = 0; y < height; y++) if (pixels[(y * width + x) * 4 + 3] > 20) ink++;
                    if (ink < best) { best = ink; cuts[cell] = x; }
                }
        }
        var images = new BitmapSource[4];
        for (int cell = 0; cell < 4; cell++)
        {
            int left = cuts[cell + 1], right = cuts[cell], top = height, bottom = -1;
            for (int y = 0; y < height; y++)
                for (int x = cuts[cell]; x < cuts[cell + 1]; x++)
                    if (pixels[(y * width + x) * 4 + 3] > 20)
                    {
                        left = Math.Min(left, x); right = Math.Max(right, x);
                        top = Math.Min(top, y); bottom = Math.Max(bottom, y);
                    }
            if (right < left || bottom < top) throw new InvalidOperationException($"Empty frame motif: {themeId}/{cell}");
            var image = new CroppedBitmap(source, new Int32Rect(left, top, right - left + 1, bottom - top + 1));
            image.Freeze();
            images[cell] = image;
        }
        Atlases[themeId] = images;
        return images;
    }

    private static SolidColorBrush Brush(string color)
    {
        if (Brushes.TryGetValue(color, out var cached)) return cached;
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        brush.Freeze();
        Brushes[color] = brush;
        return brush;
    }

}
