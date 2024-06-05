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
                OrderId = int.Parse(txtOrderId.Text),
                CustomerId = int.Parse(txtCustomerId.Text),
                TotalAmount = int.Parse(txtTotalAmount.Text),
                Type = txtType.Text,
            };
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = await _orderBusiness.GetOrderById(int.Parse(txtOrderId.Text));
                if (item.Data == null)
                {
                    Order order = GetOrder();
                    var result = await _orderBusiness.AddOrders(order);
                    MessageBox.Show(result.Message);
                    LoadGrdOrders();
                }
                else
                {
                    Order order = GetOrder();
                    var result = await _orderBusiness.UpdateOrder(order);
                    MessageBox.Show(result.Message);
                    LoadGrdOrders();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            string orderCode = btn.CommandParameter.ToString();

            //MessageBox.Show(orderCode);

            if (!string.IsNullOrEmpty(orderCode))
            {
                if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var result = await _orderBusiness.DeleteOrder(int.Parse(orderCode));
                    MessageBox.Show($"{result.Message}", "Delete");
                    this.LoadGrdOrders();
                }
            }
        }

        private async void grdOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid grd = sender as DataGrid;
            if (grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1)
            {
                var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                if (row != null)
                {
                    var item = row.Item as Order;
                    if (item != null)
                    {
                        var result = await _orderBusiness.GetOrderById(item.OrderId);

                        if (result.Status > 0 && result.Data != null)
                        {
                            item = result.Data as Order;
                            txtCustomerId.Text = item.CustomerId.ToString();
                            txtTotalAmount.Text = item.TotalAmount.ToString();
                            txtType.Text = item.Type.ToString();
                        }
                    }
                }
            }
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
