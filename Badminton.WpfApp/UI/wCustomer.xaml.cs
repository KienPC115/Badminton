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
using Microsoft.IdentityModel.Tokens;

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


        private Customer GetCustomer() {
            return new Customer {
                CustomerId = int.Parse(txtCustomerCode.Text),
                Name = txtCustomerName.Text,
                Address = txtCustomerAddress.Text,
                Phone = txtCustomerPhone.Text,
                DateOfBirth = txtCustomerDateOfBirth.DisplayDate,
                Email = txtCustomerEmail.Text,
            };
        }
        
        private async void ButtonSave_Click(object sender, RoutedEventArgs e) {
            try {
                var item = await _business.GetCustomerById(int.Parse(txtCustomerCode.Text));
                if (item.Data == null) {
                    Customer customer = GetCustomer();
                    var result = await _business.AddCustomer(customer);
                    MessageBox.Show(result.Message);
                    LoadGrdCustomers();
                    RefreshAllText();
                } else {
                    Customer customer = GetCustomer();
                    var result = await _business.UpdateCustomer(int.Parse(txtCustomerCode.Text), customer);
                    MessageBox.Show(result.Message);
                    LoadGrdCustomers();
                    RefreshAllText();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private async void grdCustomer_ButtonDelete_Click(object sender, RoutedEventArgs e) {
            try {
                Button btn = (Button)sender;

                string customerCode = btn.CommandParameter.ToString();

                //MessageBox.Show(customerCode);

                if (!string.IsNullOrEmpty(customerCode)) {
                    if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                        var result = await _business.DeleteCustomer(int.Parse(customerCode));
                        MessageBox.Show($"{result.Message}", "Delete");
                        this.LoadGrdCustomers();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void grdCustomer_MouseDouble_Click(object sender, MouseButtonEventArgs e) {
            try {
                //MessageBox.Show("Double Click on Grid");
                DataGrid grd = sender as DataGrid;
                if (grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1) {
                    var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                    if (row != null) {
                        var item = row.Item as Customer;
                        if (item != null) {
                            var result = await _business.GetCustomerById(item.CustomerId);

                            if (result.Status > 0 && result.Data != null) {
                                item = result.Data as Customer;
                                txtCustomerCode.Text = item.CustomerId.ToString();
                                txtCustomerName.Text = item.Name.ToString();
                                txtCustomerPhone.Text = item.Phone.ToString();
                                txtCustomerDateOfBirth.DisplayDate = item.DateOfBirth;
                                txtCustomerEmail.Text = item.Email.ToString();
                                txtCustomerAddress.Text = item.Address.ToString();
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void RefreshAllText() {
            txtCustomerCode.Text = string.Empty;
            txtCustomerName.Text = string.Empty;
            txtCustomerPhone.Text = string.Empty;
            txtCustomerDateOfBirth = null;
            txtCustomerEmail.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e) {
            RefreshAllText();
        }
    }
}
