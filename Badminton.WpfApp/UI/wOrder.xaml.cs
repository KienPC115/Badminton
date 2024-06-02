using Badminton.Business;
using Badminton.Data.Models;
using Microsoft.IdentityModel.Tokens;
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
    /// Interaction logic for wOrder.xaml
    /// </summary>
    public partial class wOrder : Window
    {
        IOrderBusiness _orderBusiness;
        ICustomerBusiness _customerBusiness;
        public wOrder()
        {
            _customerBusiness = new CustomerBusiness();
            _orderBusiness = new OrderBusiness();
            InitializeComponent();
            LoadGrdOrders();
        }

        private async void LoadGrdOrders()
        {
            var result = await _orderBusiness.GetAllOrders();

            if (result.Status > 0 && result.Data != null)
            {
                grdOrder.ItemsSource = result.Data as List<Order>;
            }
            else
            {
                grdOrder.ItemsSource = new List<Order>();
            }
        }

        private Order GetOrder()
        {
            return new Order
            {
                CustomerId = int.Parse(txtCustomerId.Text),
                TotalAmount = int.Parse(txtTotalAmount.Text),
                Type = txtType.Text,
            };
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Order order = GetOrder();
                var result = await _orderBusiness.AddOrders(order);
                MessageBox.Show(result.Message);
                LoadGrdOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Insert Order");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void grdOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private async void txtCustomerId_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string temp = txtCustomerId.Text;
                if (temp.IsNullOrEmpty())
                {
                    lbName.Content = "Not Found";
                    lbPhone.Content = "";
                    lbEmail.Content = "";
                    return;
                }
                var result = await _customerBusiness.GetCustomerById(int.Parse(temp));
                if (result.Status <= 0)
                {
                    lbName.Content = "Not Found";
                    lbPhone.Content = "";
                    lbEmail.Content = "";
                    return;
                }
                if (result.Data != null)
                {
                    Customer customer = result.Data as Customer;
                    lbEmail.Content = customer.Email;
                    lbName.Content = customer.Name;
                    lbPhone.Content = customer.Phone;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
