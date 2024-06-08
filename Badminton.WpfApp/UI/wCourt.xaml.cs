using Badminton.Business;
using Badminton.Business.Shared;
using Badminton.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        List<string> courtStatus;
        public wCourt()
        {
            InitializeComponent();
            this._courtBusiness = new CourtBusiness();
            this.LoadGrdCourts();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e) {
            try {
                int idTmp = -1;
                int.TryParse(txtCourtId.Text, out idTmp);
                var item = await _courtBusiness.GetCourtById(idTmp);

                if (item.Data == null) {
                    /*|| string.IsNullOrEmpty(txtCourtStatus.Text)*/
                    if (string.IsNullOrEmpty(txtCourtName.Text) || string.IsNullOrEmpty(txtCourtPrice.Text)) {
                        MessageBox.Show("You need to fill required fields!");
                        return;
                    }
                    Court court = new Court() {
                        Name = txtCourtName.Text,
                        Description = txtCourtDescription.Text,
                        Status = comboBoxStatus.SelectedItem.ToString(),
                        Price = double.Parse(txtCourtPrice.Text)
                    };
                    var result = await _courtBusiness.AddCourt(court);
                    if (result.Status > 0) {
                        this.LoadGrdCourts();
                    }
                    MessageBox.Show(result.Message);
                }
                else {
                    if (item.Status < 0 && item.Data == null) {
                        MessageBox.Show(item.Message);
                    }
                    var court = item.Data as Court;
                    /*string.IsNullOrEmpty(txtCourtStatus.Text)*/
                    if (string.IsNullOrEmpty(txtCourtName.Text) || string.IsNullOrEmpty(txtCourtPrice.Text)) {
                        MessageBox.Show("You need to fill required fields!");
                        return;
                    }
                    court.Name = txtCourtName.Text.Trim();
                    court.Description = txtCourtDescription.Text.Trim();
                    court.Status = comboBoxStatus.SelectedItem.ToString();
                    /*court.Status = txtCourtStatus.Text.Trim();*/
                    court.Price = double.Parse(txtCourtPrice.Text.Trim());

                    var resultUpdate = await _courtBusiness.UpdateCourt(court.CourtId, court);
                    MessageBox.Show(resultUpdate.Message);
                    if (resultUpdate.Status > 0) {
                        this.LoadGrdCourts();
                    }
                }

                this.ClearData();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void ButtonCancel_Click(object sender, RoutedEventArgs e) {
            this.ClearData();
        }

        private async void grdCourt_ButtonDelete_Click(object sender, RoutedEventArgs e) {

            Button btn = (Button)sender;

            string courtId = btn.CommandParameter.ToString();

            //MessageBox.Show(currencyCode);

            if (!string.IsNullOrEmpty(courtId)) {
                if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    int id = int.Parse(courtId);
                    var result = await _courtBusiness.DeleteCourt(id);
                    MessageBox.Show($"{result.Message}", "Delete");
                    this.LoadGrdCourts();
                }
            }
            /*var parameter = (sender as ButtonBase).CommandParameter;
            int courtId = int.Parse(parameter.ToString());
            if(MessageBox.Show("Are you sure?", "Delete Court", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) {
                return;
            }

            var result = await _courtBusiness.DeleteCourt(courtId);
            if (result.Status > 0) {
                this.LoadGrdCourts();
            }
            MessageBox.Show(result.Message);*/
        }

        private async void grdCourt_MouseDouble_Click(object sender, MouseButtonEventArgs e) {

            //MessageBox.Show("Double Click on Grid");
            DataGrid grd = sender as DataGrid;
            if (grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1) {
                var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                if (row != null) {
                    var item = row.Item as Court;
                    if (item != null) {
                        var courtResult = await _courtBusiness.GetCourtById(item.CourtId);

                        if (courtResult.Status > 0 && courtResult.Data != null) {
                            item = courtResult.Data as Court;
                            txtCourtId.Text = item.CourtId.ToString();
                            txtCourtName.Text = item.Name.ToString();
                            txtCourtDescription.Text = item.Description.ToString();
                            comboBoxStatus.SelectedValue = item.Status;
                            /*txtCourtStatus.Text = item.Status.ToString();*/
                            txtCourtPrice.Text = item.Price.ToString();
                        }
                    }
                }
                /*Court court = (sender as DataGrid).SelectedItem as Court;
                if(court != null) {
                    txtCourtId.Text = court.CourtId.ToString();
                    txtCourtName.Text = court.Name.ToString();
                    txtCourtDescription.Text = court.Description.ToString();
                    txtCourtStatus.Text = court.Status.ToString();
                    txtCourtPrice.Text = court.Price.ToString();
                }*/
            }
        }

        public void ClearData() {
            txtCourtId.Clear();
            txtCourtName.Clear();
            txtCourtDescription.Clear();
            comboBoxStatus.SelectedItem = courtStatus[0];
            txtCourtPrice.Clear();
        }

        private async void LoadGrdCourts() {
            var result = await _courtBusiness.GetAllCourts();
            courtStatus = CourtShared.Status();
            comboBoxStatus.ItemsSource = courtStatus;
            comboBoxStatus.SelectedItem = courtStatus[0];
            if (result.Status > 0 && result.Data != null) {
                grdCourt.ItemsSource = result.Data as List<Court>;
            }
            else {
                grdCourt.ItemsSource = new List<Court>();
            }
        }
    }
}
