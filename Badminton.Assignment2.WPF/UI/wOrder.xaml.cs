using Badminton.Assignment2.WPF.Services;
using Badminton.Business;
using Badminton.Data;
using Badminton.Data.Models;
using Badminton.Data.Repository;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

namespace Badminton.Assignment2.WPF.UI
{
    /// <summary>
    /// Interaction logic for wOrder.xaml
    /// </summary>
    public partial class wOrder : Window
    {
        private OrderService _service;
        private ICustomerBusiness _customerBusiness;
        public wOrder()
        {
            _service ??= new();
            _customerBusiness ??= new CustomerBusiness();
            InitializeComponent();
            LoadGrdOrders();
        }

        private void LoadGrdOrders()
        {
            var selectedComboBoxItem = cboSource.SelectedItem as ComboBoxItem;
            var f = selectedComboBoxItem?.Content.ToString();

            if (string.IsNullOrEmpty(f))
            {
                cboSource.SelectedIndex = 0;
                f = "Json";
            }

            _service.ChangeSource(f);
            grdOrder.ItemsSource = _service.GetAll();
        }

        private void txtCustomerId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = txtCustomerId.Text;
            var result = _customerBusiness.GetCustomerById(int.Parse(s));
            if (result.Result.Status < 0)
            {
                lbCustomerInfo.Content = "Not Found";
                return;
            }
            var customer = result.Result.Data as Customer;
            lbCustomerInfo.Content = customer.Email;
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtCustomerId.Text = "0";
            txtDate.Text = DateTime.Now.ToString();
            txtKey.Text = string.Empty;
            txtNote.Text = string.Empty;
            txtOrderId.Text = "0";
            lbCustomerInfo.Content = string.Empty;
            txtTotalAmount.Text = "0";
        }

        private Order GetOrder()
        {
            try
            {
                var result = _customerBusiness.GetCustomerById(int.Parse(txtCustomerId.Text));
                if (result.Result.Status < 0)
                {
                    throw new Exception(result.Result.Message);
                }
                return new Order
                {
                    Customer = result.Result.Data as Customer,
                    CustomerId = int.Parse(txtCustomerId.Text),
                    OrderDate = DateTime.Parse(txtDate.Text),
                    OrderId = int.Parse(txtOrderId.Text),
                    OrderNotes = txtNote.Text,
                    TotalAmount = double.Parse(txtTotalAmount.Text),
                    Type = txtType.Text
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var orderId = int.Parse(txtOrderId.Text);
                if (orderId ==0)
                {
                    _service.Insert(GetOrder());
                }
                else { _service.Update(GetOrder()); }
                _service.SaveChange(cboSource.Text);
                LoadGrdOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cboSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadGrdOrders();
        }

        private void grdOrder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
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
                            var result = _service.GetById(item.OrderId);

                            if (result != null)
                            {
                                txtOrderId.Text = result.OrderId.ToString();
                                txtCustomerId.Text = result.CustomerId.ToString();
                                txtTotalAmount.Text = result.TotalAmount.ToString();
                                txtNote.Text = result.OrderNotes;
                                txtDate.SelectedDate = result.OrderDate;

                                // Select the appropriate item in the ComboBox
                                foreach (ComboBoxItem comboBoxItem in txtType.Items)
                                {
                                    if (comboBoxItem.Content.ToString() == result.Type)
                                    {
                                        txtType.SelectedItem = comboBoxItem;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                string orderCode = btn.CommandParameter.ToString();

                //MessageBox.Show(orderCode);

                if (!string.IsNullOrEmpty(orderCode))
                {
                    if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _service.Delete(int.Parse(orderCode));
                        _service.SaveChange(cboSource.Text);
                        this.LoadGrdOrders();
                    }
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
    }
}
