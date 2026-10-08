using ActiBoard.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ActiBoard.View.Controls;

public partial class SettingsPopup : UserControl
{
    private PasswordBox? _pinTarget;
    public SettingsPopup()
    {
        InitializeComponent();
        _pinTarget = CurrentPin;
        Unloaded += (_, _) => { CurrentPin.Clear(); NewPin.Clear(); ConfirmPin.Clear(); _pinTarget = CurrentPin; };
    }
    private void PinFocus(object sender, KeyboardFocusChangedEventArgs e) => _pinTarget = sender as PasswordBox;
    private void PinNumber(object sender, RoutedEventArgs e)
    {
        if (_pinTarget is not null && _pinTarget.Password.Length < 4 && sender is Button button)
            _pinTarget.Password += button.Content.ToString();
    }
    private void PinBackspace(object sender, RoutedEventArgs e)
    {
        if (_pinTarget is not null && _pinTarget.Password.Length > 0) _pinTarget.Password = _pinTarget.Password[..^1];
    }
    private void ChangePin(object sender, RoutedEventArgs e)
    {
        if (DataContext is LocationViewModel vm && vm.ChangeAdministrationPin(CurrentPin.Password, NewPin.Password, ConfirmPin.Password))
        { CurrentPin.Clear(); NewPin.Clear(); ConfirmPin.Clear(); }
    }
}
