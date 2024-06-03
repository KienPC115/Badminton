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
    /// Interaction logic for wCourtDetail.xaml
    /// </summary>
    public partial class wCourtDetail : Window
    {
        private readonly CourtDetailBusiness _courtDetailBusiness;
        public wCourtDetail()
        {
            InitializeComponent();
            this._courtDetailBusiness ??= new CourtDetailBusiness();
        }
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
        }
        private async void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
        }
        private async void grdCourtDetail_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
        }
        private async void LoadGrdCourtDetails()
        {
            var result = await _courtDetailBusiness.GetAllCourtDetails();

            if (result.Status > 0 && result.Data != null)
            {
                grdCourtDetail.ItemsSource = result.Data as List<CourtDetail>;
            }
            else
            {
                grdCourtDetail.ItemsSource = new List<CourtDetail>();
            }
        }


    }
}
