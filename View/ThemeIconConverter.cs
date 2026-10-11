using System;
using System.Globalization;
using System.Windows.Data;
using ActiBoard.View.Controls;

namespace ActiBoard.View;
public sealed class ThemeIconConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string id || id is not ("halloween" or "juleklippedag" or "fastelavn" or "rekorduge" or "åbenthus")) return null;
        return AnimatedThemeFrame.GetImages(id)[0];
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
