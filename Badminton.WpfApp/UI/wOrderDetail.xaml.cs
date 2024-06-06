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
    /// Interaction logic for wOrderDetail.xaml
    /// </summary>
    public partial class wOrderDetail : Window
    {
        IOrderBusiness _orderBusiness;
        IOrderDetailBunsiness _orderDetailBunsiness;
        ICourtDetailBusiness _courtDetailBusiness;
        ICourtBusiness _courtBusiness;
        public wOrderDetail()
        {
            _courtDetailBusiness = new CourtDetailBusiness();
            _orderBusiness = new OrderBusiness();
            _orderDetailBunsiness = new OrderDetailBusiness();
            _courtBusiness = new CourtBusiness();
            InitializeComponent();
            LoadGrdOrderDetails();
        }
        private OrderDetail GetOrderDetail()
        {
            return new OrderDetail
            {
                CourtDetailId = int.Parse(txtCourtDetailId.Text),
                OrderDetailId = int.Parse(txtOrderDetailId.Text),
                OrderId = int.Parse(txtOrderId.Text),
                Amount = double.Parse(txtAmount.Text),
            };
        }
        private async void LoadGrdOrderDetails()
        {
            try
            {
                var result = await _orderDetailBunsiness.GetAllOrderDetails();

                if (result.Status > 0 && result.Data != null)
                {
                    grdOrderDetail.ItemsSource = result.Data as List<OrderDetail>;
                }
                else
                {
                    grdOrderDetail.ItemsSource = new List<OrderDetail>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = await _orderDetailBunsiness.GetOrderDetailById(int.Parse(txtOrderDetailId.Text));
                if (item.Data == null)
                {
                    OrderDetail orderDetail = GetOrderDetail();
                    var result = await _orderDetailBunsiness.AddOrderDetail(orderDetail);
                    if (result.Status < 0) { MessageBox.Show(result.Message); return; }
                    result = await _orderBusiness.UpdateAmount(orderDetail.OrderId); MessageBox.Show(result.Message);
                }
                else
                {
                    OrderDetail orderDetail = GetOrderDetail();
                    var result = await _orderDetailBunsiness.UpdateOrderDetail(orderDetail);
                    MessageBox.Show(result.Message);

                }
                LoadGrdOrderDetails();
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

        private async void grdOrderDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                        var item = row.Item as OrderDetail;
                        if (item != null)
                        {
                            var result = await _orderDetailBunsiness.GetOrderDetailById(item.OrderDetailId);

                            if (result.Status > 0 && result.Data != null)
                            {
                                item = result.Data as OrderDetail;
                                txtOrderDetailId.Text = item.OrderDetailId.ToString();
                                txtOrderId.Text = item.OrderId.ToString();
                                txtCourtDetailId.Text = item.CourtDetailId.ToString();
                                txtAmount.Text = item.Amount.ToString();
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

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;

                string code = btn.CommandParameter.ToString();

                //MessageBox.Show(orderCode);

                if (!string.IsNullOrEmpty(code))
                {
                    if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var deletedOrderDetail = await _orderDetailBunsiness.GetOrderDetailById(int.Parse(code));
                        var orderDetail = deletedOrderDetail.Data as OrderDetail;
                        var result = await _orderDetailBunsiness.DeleteOrderDetail(int.Parse(code));
                        _orderBusiness.UpdateAmount(orderDetail.OrderId);
                        MessageBox.Show($"{result.Message}", "Delete");
                        LoadGrdOrderDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void txtOrderId_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string temp = txtOrderId.Text;
                if (temp.IsNullOrEmpty())
                {
                    lbOrderInfo.Content = string.Empty;
                    return;
                }
                var result = await _orderBusiness.GetOrderById(int.Parse(temp));
                if (result.Status <= 0)
                {
                    lbOrderInfo.Content = "Not Found";
                    return;
                }
                var order = result.Data as Order;
                lbOrderInfo.Content = $"Customer Name: {order.Customer.Name} - Type: {order.Type}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void txtCourtDetailId_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string temp = txtCourtDetailId.Text;
                if (temp.IsNullOrEmpty())
                {
                    lbCourtInfo.Content = string.Empty;
                    return;
                }
                var result = await _courtDetailBusiness.GetCourtDetail(int.Parse(temp));
                if (result.Status <= 0)
                {
                    lbCourtInfo.Content = "Not Found";
                    return;
                }
                var courtDetail = result.Data as CourtDetail;
                result = await _courtBusiness.GetCourtById(int.Parse(temp));
                if (result.Data == null)
                {
                    lbCourtInfo.Content = "Not Found";
                    return;
                }
                courtDetail.Court = result.Data as Court;
                lbCourtInfo.Content = $"Court name: {courtDetail.Court.Name} - Start time: {courtDetail.StartTime} - End time: {courtDetail.EndTime}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void txtAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
