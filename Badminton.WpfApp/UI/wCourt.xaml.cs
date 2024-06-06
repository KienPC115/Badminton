using Badminton.Business;
using Badminton.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public wCourt()
        {
            InitializeComponent();
            this._courtBusiness = new CourtBusiness();
            this.LoadGrdCourts();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e) {
            if(string.IsNullOrEmpty(txtCourtId.Text)) {
                Court court = new Court();
                if (string.IsNullOrEmpty(txtCourtName.Text) || string.IsNullOrEmpty(txtCourtStatus.Text) || string.IsNullOrEmpty(txtCourtPrice.Text)) {
                    MessageBox.Show("You need to fill required fields!");
                    return;
                }
                court.Name = txtCourtName.Text;
                court.Description = txtCourtDescription.Text;
                court.Status = txtCourtStatus.Text;
                court.Price = double.Parse(txtCourtPrice.Text);

                var result = await _courtBusiness.AddCourt(court);
                if (result.Status > 0) {
                    this.LoadGrdCourts();
                }
                MessageBox.Show(result.Message);
            }
            else {
                var result = await _courtBusiness.GetCourtById(int.Parse(txtCourtId.Text));
                if(result.Status < 0 && result.Data == null) {
                    MessageBox.Show(result.Message);
                }

                Court court = result.Data as Court;
                court.Name = txtCourtName.Text.Trim();
                court.Description = txtCourtDescription.Text.Trim();
                court.Status = txtCourtStatus.Text.Trim();
                court.Price = double.Parse(txtCourtPrice.Text.Trim());

                var resultUpdate = await _courtBusiness.UpdateCourt(court.CourtId, court);
                MessageBox.Show(resultUpdate.Message);
                if(resultUpdate.Status > 0) {
                    this.LoadGrdCourts();
                }
            }
        }

        private async void ButtonCancel_Click(object sender, RoutedEventArgs e) {
            this.ClearData();
        }

        private async void grdCourt_ButtonDelete_Click(object sender, RoutedEventArgs e) {
            var parameter = (sender as ButtonBase).CommandParameter;
            int courtId = int.Parse(parameter.ToString());
            if(MessageBox.Show("Are you sure?", "Delete Court", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) {
                return;
            }

            var result = await _courtBusiness.DeleteCourt(courtId);
            if (result.Status > 0) {
                this.LoadGrdCourts();
            }
            MessageBox.Show(result.Message);
        }

        private async void grdCourt_MouseDouble_Click(object sender, MouseButtonEventArgs e) {
            Court court = (sender as DataGrid).SelectedItem as Court;
            if(court != null) {
                txtCourtId.Text = court.CourtId.ToString();
                txtCourtName.Text = court.Name.ToString();
                txtCourtDescription.Text = court.Description.ToString();
                txtCourtStatus.Text = court.Status.ToString();
                txtCourtPrice.Text = court.Price.ToString();
            }
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
