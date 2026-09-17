using ActiBoard.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ActiBoard.View.Controls
{
    /// <summary>
    /// Interaction logic for UnlockPopup.xaml
    /// </summary>
    public partial class UnlockPopup : UserControl
    {
        private string enteredCode = "";

        public UnlockPopup()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && enteredCode.Length < 4)
            {
                enteredCode += button.Content.ToString();

                UnlockCodeBox.Text = string.Join(" ", enteredCode.Select(c => "●"));
            }
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (enteredCode.Length > 0)
            {
                enteredCode = enteredCode.Substring(0, enteredCode.Length - 1);

                UnlockCodeBox.Text = string.Join(" ", enteredCode.Select(c => "●"));
            }
        }

        private void UnlockButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LocationViewModel viewModel)
            {
                viewModel.UnlockCommand.Execute(enteredCode);

                if (viewModel.IsUnlocked)
                {
                    ClearUnlockCode();
                }
                else
                {
                    ErrorText.Visibility = Visibility.Visible;
                    ShakePopup();

                    enteredCode = "";
                    UnlockCodeBox.Clear();
                }
            }
        }

        private void ClearUnlockCode()
        {
            enteredCode = "";
            UnlockCodeBox.Clear();
            ErrorText.Visibility = Visibility.Collapsed;
        }

        private void CancelUnlockButton_Click(object sender, RoutedEventArgs e)
        {
            ClearUnlockCode();

            if (DataContext is LocationViewModel viewModel)
            {
                viewModel.ClosePopupCommand.Execute(null);
            }
        }

        private void ShakePopup()
        {
            var transform = new TranslateTransform();
            PopupBorder.RenderTransform = transform;

            var animation = new DoubleAnimationUsingKeyFrames();

            animation.KeyFrames.Add(new EasingDoubleKeyFrame(-10, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(50))));
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(10, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100))));
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(-8, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(150))));
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(8, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200))));
            animation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250))));

            transform.BeginAnimation(TranslateTransform.XProperty, animation);
        }
    }
}
