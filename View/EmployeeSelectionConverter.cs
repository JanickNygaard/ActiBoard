using ActiBoard.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ActiBoard.View
{
    public class EmployeeSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value ?? LocationViewModel.EmptyEmployeeOption;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => ReferenceEquals(value, LocationViewModel.EmptyEmployeeOption) ? null : value;
    }
}