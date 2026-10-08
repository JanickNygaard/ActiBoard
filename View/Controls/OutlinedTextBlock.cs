using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ActiBoard.View.Controls;

/// <summary>Dashboard label with a crisp black contour and normal WPF font properties.</summary>
public sealed class OutlinedTextBlock : Control
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(OutlinedTextBlock),
        new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static readonly Pen Outline = CreateOutline();
    private static Pen CreateOutline()
    {
        var pen = new Pen(Brushes.Black, 2) { LineJoin = PenLineJoin.Round };
        pen.Freeze();
        return pen;
    }

    private FormattedText FormatText() => new(Text ?? "", CultureInfo.CurrentUICulture, FlowDirection,
        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize,
        Foreground ?? Brushes.White, VisualTreeHelper.GetDpi(this).PixelsPerDip);

    protected override Size MeasureOverride(Size availableSize)
    {
        var text = FormatText();
        return new Size(text.WidthIncludingTrailingWhitespace + Padding.Left + Padding.Right + 2,
            text.Height + Padding.Top + Padding.Bottom + 2);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (Background is not null) drawingContext.DrawRectangle(Background, null, new Rect(RenderSize));
        if (string.IsNullOrEmpty(Text)) return;
        var text = FormatText();
        var geometry = text.BuildGeometry(new Point(Padding.Left + 1, Padding.Top + 1));
        drawingContext.DrawGeometry(null, Outline, geometry);
        drawingContext.DrawGeometry(Foreground, null, geometry);
    }
}
