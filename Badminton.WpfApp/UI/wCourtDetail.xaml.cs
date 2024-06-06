using Badminton.Business;
using Badminton.Business.Shared;
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
using System.Windows.Forms.VisualStyles;
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
        private readonly CourtBusiness _courtBusiness;
        public wCourtDetail()
        {
            this._courtDetailBusiness = new CourtDetailBusiness();
            this._courtBusiness = new CourtBusiness();
            InitializeComponent();
            this.LoadData();
        }

        public async void LoadData()
        {
            var result = await _courtDetailBusiness.GetAllCourtDetailsIncludeCourt();
            var resultCourts = await _courtBusiness.GetAllCourts();
            var SlotList = CourtDetailShared.Slot();
            var StatusList = CourtDetailShared.Status();
            ComboBoxSlot.ItemsSource = SlotList;
            ComboBoxStatus.ItemsSource = StatusList;
            if (result.Status > 0 && result.Data != null)
            {
                grdCourtDetail.ItemsSource = result.Data as List<Data.Models.CourtDetail>;
            }
            else
            {
                grdCourtDetail.ItemsSource = new List<Data.Models.CourtDetail>();
            }
            if (resultCourts.Status > 0 && resultCourts.Data != null)
            {
                ComboBoxCourtName.ItemsSource = resultCourts.Data as List<Court>;
            }
            else
            {
                ComboBoxCourtName.ItemsSource = new List<Court>();
            }
        }
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        { 
            var court = ComboBoxCourtName.SelectedItem as Court;
            if (string.IsNullOrEmpty(court.Name) || string.IsNullOrEmpty(ComboBoxSlot.SelectedItem.ToString()) || string.IsNullOrEmpty(ComboBoxStatus.SelectedItem.ToString()) || string.IsNullOrEmpty(txtCourtPrice.Text))
            {
                MessageBox.Show("You need to fill required fields!");
                return;
            }
            string slot = ComboBoxSlot.SelectedItem.ToString();
            string status = ComboBoxStatus.SelectedItem.ToString();
            var Ok = Double.TryParse(txtCourtPrice.Text, out var price);
            if (Ok == false || price < 0) MessageBox.Show("The price is invalid please enter again");
            if (string.IsNullOrEmpty(txtCourtDetailId.Text))
            {
                var courtDetail = new CourtDetail();
                courtDetail.CourtId = court.CourtId;
                courtDetail.Status = status;
                courtDetail.Price = price;
                courtDetail.Slot = slot;
                var result  = await _courtDetailBusiness.AddCourtDetail(courtDetail);   
                if (result.Status > 0)
                {
                    this.LoadData();
                }
                MessageBox.Show(result.Message);
            }
            else
            {
                var courtDetailId = int.Parse(txtCourtDetailId.Text);
                var result =await _courtDetailBusiness.GetCourtDetail(courtDetailId);
                if(result.Status < 0 && result == null) {
                    MessageBox.Show(result.Message);
                }
                var courtDetail = result as CourtDetail;
                courtDetail.CourtId = court.CourtId;
                courtDetail.Status = status;
                courtDetail.Slot = slot;
                courtDetail.Price = price;
                var resultUpdate = await _courtDetailBusiness.UpdateCourtDetail(courtDetailId,courtDetail);
                if(resultUpdate.Status < 0)
                {
                    MessageBox.Show(resultUpdate.Message);
                }

            }
        }
        public void ClearData()
        {
            txtCourtDetailId.Clear();
            ComboBoxStatus.SelectedItem = null;
            ComboBoxSlot.SelectedItem = null;
            ComboBoxCourtName.SelectedItem = null;
            txtCourtPrice.Clear();
        }
        private async void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.ClearData();
        }

        private async void grdCourt_ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            var courtDetailId = Int32.Parse(btn.CommandParameter.ToString());


                if (MessageBox.Show("Do you want to delete this item?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var result = await _courtDetailBusiness.DeleteCourtDetail(courtDetailId);
                    MessageBox.Show($"{result.Message}", "Delete");
                    this.LoadData();
                }
        }

        private async void grdCourtDetail_MouseDouble_Click(object sender, MouseButtonEventArgs e)
        {
            var courtDetail = (sender as DataGrid).SelectedItem as CourtDetail;

            if (courtDetail != null ) {
                txtCourtDetailId.Text = courtDetail.CourtDetailId.ToString();
                ComboBoxCourtName.SelectedItem = courtDetail.Court;
                ComboBoxSlot.SelectedItem = courtDetail.Slot;
                ComboBoxStatus.SelectedItem = courtDetail.Status;
                txtCourtPrice.Text = courtDetail.Price.ToString();
            }
        }
    }
}
