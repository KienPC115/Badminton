using Badminton.Business;
using Badminton.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Badminton.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for wCourt.xaml
    /// </summary>
    public partial class wCourt : Window
    {
        private readonly ICourtBusiness _courtBusiness;
        public Court Court { get; set; }
        public wCourt()
        {
            InitializeComponent();
            this._courtBusiness = new CourtBusiness();
            this.LoadGrdCourts();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e) {
        }

        private async void ButtonCancel_Click(object sender, RoutedEventArgs e) {
            this.ClearData();
        }

        private async void grdCourt_ButtonDelete_Click(object sender, RoutedEventArgs e) {
        }

        private async void grdCourt_MouseDouble_Click(object sender, RoutedEventArgs e) {
        }

        public void ClearData() {
            txtCourtId.Clear();
            txtCourtName.Clear();
            txtCourtDescription.Clear();
            txtCourtStatus.Clear();
            txtCourtPrice.Clear();
        }

        private async void LoadGrdCourts() {
            var result = await _courtBusiness.GetAllCourts();

            if (result.Status > 0 && result.Data != null) {
                grdCourt.ItemsSource = result.Data as List<Court>;
            }
            else {
                grdCourt.ItemsSource = new List<Court>();
            }
        }
    }
}
