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
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new LocationViewModel();
        }

    }
}