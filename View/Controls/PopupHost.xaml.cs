using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ActiBoard.View.Controls
{
    public delegate void KeyboardRequestedEventHandler(
    object sender,
    TextBox targetTextBox);
    public partial class PopupHost : UserControl
    {
        public PopupHost()
        {
            InitializeComponent();

            AddHandler(
                UIElement.GotFocusEvent,
                new RoutedEventHandler(PopupHost_GotFocus));
        }

        public void ShowKeyboard(TextBox targetTextBox)
        {
            SharedKeyboard.SetTarget(targetTextBox);
            KeyboardOverlay.Visibility = Visibility.Visible;
        }

        public void HideKeyboard()
        {
            SharedKeyboard.ClearTarget();
            KeyboardOverlay.Visibility = Visibility.Collapsed;
        }

        private void SharedKeyboard_Done(
            object sender,
            RoutedEventArgs e)
        {
            HideKeyboard();
        }

        private void PopupHost_GotFocus(
            object sender,
            RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox)
            {
                ShowKeyboard(textBox);
            }
        }
    }
}