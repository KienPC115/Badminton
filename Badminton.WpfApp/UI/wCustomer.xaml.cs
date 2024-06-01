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
using Badminton.Business;
using Badminton.Data.Models;

namespace Badminton.WpfApp.UI {
    /// <summary>
    /// Interaction logic for wCustomer.xaml
    /// </summary>
    public partial class wCustomer : Window {

        private readonly ICustomerBusiness _business;

        public wCustomer() {
            InitializeComponent();
            this._business = new CustomerBusiness();
            LoadGrdCustomers();
        }

        private async void LoadGrdCustomers() {
            var result = await _business.GetAllCustomers();

            if (result.Status > 0 && result.Data != null) {
                grdCustomer.ItemsSource = result.Data as List<Customer>;
            } else {
                grdCustomer.ItemsSource = new List<Customer>();
            }
        }
    }
}
