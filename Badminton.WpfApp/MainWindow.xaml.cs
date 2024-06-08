using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Badminton.WpfApp.UI;
using Badminton.WpfApp.UI;

namespace Badminton.WpfApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private async void Open_wCourt_Click(object sender, RoutedEventArgs e) {
            var p = new wCourt();
            p.Owner = this;
            p.Show();
        }
        private async void Open_wCourtDetail_Click(object sender, RoutedEventArgs e) {
            var p = new wCourtDetail();
            p.Owner = this;
            p.Show();
        }
        private void Open_wOrder_Click(object sender, RoutedEventArgs e) {
            var p = new wOrder();
            p.Owner = this;
            p.Show();
        }
        private void Open_wOrderDetail_Click(object sender, RoutedEventArgs e) {
            var p = new wOrderDetail();
            p.Owner = this;
            p.Show();
        }
        private async void Open_wCustomer_Click(object sender, RoutedEventArgs e) {
            var p = new wCustomer();
            p.Owner = this;
            p.Show();
        }
    }
}
