using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ActiBoard.ViewModel;

namespace ActiBoard.View
{
    public partial class MainWindow : Window
    {
        private string enteredCode = "";
        private readonly System.Windows.Threading.DispatcherTimer _lockTimer = new() { Interval = System.TimeSpan.FromSeconds(1) };
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new LocationViewModel();
            PreviewMouseDown += (_, _) => RegisterActivity();
            PreviewMouseMove += (_, _) => RegisterActivity();
            PreviewKeyDown += (_, _) => RegisterActivity();
            PreviewTouchDown += (_, _) => RegisterActivity();
            _lockTimer.Tick += (_, _) => (DataContext as LocationViewModel)?.CheckAutoLock(System.DateTime.UtcNow);
            Loaded += (_, _) => _lockTimer.Start();
            Closed += (_, _) => _lockTimer.Stop();
        }

        private void RegisterActivity() => (DataContext as LocationViewModel)?.RegisterInteraction();

    }
}
