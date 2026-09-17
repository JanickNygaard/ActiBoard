using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ActiBoard.View.Controls
{
    public partial class OnScreenKeyboard : UserControl
    {
        private TextBox? _targetTextBox;
        private bool _useUpperCase = true;

        public event RoutedEventHandler? Done;

        public OnScreenKeyboard()
        {
            InitializeComponent();
        }

        public void SetTarget(TextBox textBox)
        {
            _targetTextBox = textBox;

            _targetTextBox.Focus();

            if (_targetTextBox.SelectionLength == 0)
            {
                _targetTextBox.CaretIndex =
                    _targetTextBox.Text?.Length ?? 0;
            }

            _useUpperCase =
    string.IsNullOrWhiteSpace(_targetTextBox.Text);

            UpdateShiftVisual();
            UpdateLetterButtons();
        }

        public void ClearTarget()
        {
            _targetTextBox = null;
        }

        private void CharacterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Content is not string character)
            {
                return;
            }

            string text = _useUpperCase
                ? character.ToUpper()
                : character.ToLower();

            InsertText(text);

            // Shift gælder kun for næste bogstav
            if (_useUpperCase)
            {
                _useUpperCase = false;

                UpdateShiftVisual();
                UpdateLetterButtons();
            }
        }

        private void SpaceButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            InsertText(" ");
        }

        private void BackspaceButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_targetTextBox is null)
                return;

            int selectionStart =
                _targetTextBox.SelectionStart;

            int selectionLength =
                _targetTextBox.SelectionLength;

            string currentText =
                _targetTextBox.Text ?? string.Empty;

            if (selectionLength > 0)
            {
                currentText = currentText.Remove(
                    selectionStart,
                    selectionLength);

                _targetTextBox.Text = currentText;
                _targetTextBox.CaretIndex = selectionStart;

                UpdateBinding();
                return;
            }

            if (selectionStart <= 0)
                return;

            currentText = currentText.Remove(
                selectionStart - 1,
                1);

            _targetTextBox.Text = currentText;
            _targetTextBox.CaretIndex =
                selectionStart - 1;

            UpdateBinding();
            UpdateShiftAfterTextChange();



        }

        private void ShiftButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            _useUpperCase = !_useUpperCase;

            UpdateShiftVisual();
            UpdateLetterButtons();
        }

        private void DoneButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_targetTextBox is not null)
            {
                UpdateBinding();

                _targetTextBox.MoveFocus(
                    new TraversalRequest(
                        FocusNavigationDirection.Next));
            }

            ClearTarget();

            Done?.Invoke(this, e);
        }

        private void InsertText(string value)
        {
            if (_targetTextBox is null)
                return;

            int selectionStart =
                _targetTextBox.SelectionStart;

            int selectionLength =
                _targetTextBox.SelectionLength;

            string currentText =
                _targetTextBox.Text ?? string.Empty;

            if (selectionLength > 0)
            {
                currentText = currentText.Remove(
                    selectionStart,
                    selectionLength);
            }

            currentText = currentText.Insert(
                selectionStart,
                value);

            _targetTextBox.Text = currentText;

            _targetTextBox.CaretIndex =
                selectionStart + value.Length;

            UpdateBinding();
        }

        private void UpdateBinding()
        {
            _targetTextBox?
                .GetBindingExpression(TextBox.TextProperty)
                ?.UpdateSource();
        }

        private void UpdateShiftVisual()
        {
            ShiftButton.Content =
                _useUpperCase
                    ? "Skift ↑"
                    : "Skift";
        }

        private void UpdateLetterButtons()
        {
            UpdateButtonsRecursive(this);
        }

        private void UpdateButtonsRecursive(DependencyObject parent)
        {
            for (int i = 0;
                 i < VisualTreeHelper.GetChildrenCount(parent);
                 i++)
            {
                DependencyObject child =
                    VisualTreeHelper.GetChild(parent, i);

                if (child is Button button &&
                    button.Content is string content &&
                    content.Length == 1 &&
                    char.IsLetter(content[0]))
                {
                    button.Content =
                        _useUpperCase
                            ? content.ToUpper()
                            : content.ToLower();
                }

                UpdateButtonsRecursive(child);
            }
        }

        private void UpdateShiftAfterTextChange()
        {
            if (_targetTextBox is null)
                return;

            if (string.IsNullOrEmpty(_targetTextBox.Text))
            {
                _useUpperCase = true;

                UpdateShiftVisual();
                UpdateLetterButtons();
            }
        }
    }
}
