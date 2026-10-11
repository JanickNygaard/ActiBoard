using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ActiBoard.View.Controls;

/// <summary>Clips location contents to the inside of the rounded status border.</summary>
public sealed class RoundedLocationGrid : Grid
{
    public RoundedLocationGrid()
    {
        SizeChanged += (_, _) => UpdateClip();
        Loaded += (_, _) => UpdateClip();
    }

    private void UpdateClip()
    {
        double radius = Math.Min(14, Math.Min(ActualWidth, ActualHeight) / 2);
        Clip = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight), radius, radius);
    }
}
