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
using BSCTracker.Entities;
using BSCTracker.Utilities;
namespace BSCTracker.Screens
{
    /// <summary>
    /// Interaction logic for FisherGPSWindow.xaml
    /// </summary>
    public partial class FisherGPSWindow : Window
    {
        public bool Cancelled { get; set; }
        private Fisher _fisher;
        private bool _readOnly;
        private StartWindow _parentWindow;
        public FisherGPSWindow(Fisher fisher, bool readOnly=false, StartWindow parentWindow=null )
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            if(Entities.BSCEntities.GPSViewModel==null)
            {
                Entities.BSCEntities.GPSViewModel = new GPSViewModel();
            }


            //BSCEntities.FisherGPSViewModel = new FisherGPSViewModel(fisher.FisherID);
            
            this.Loaded += Window_Loaded;
            lblFisher.Content = $"Fisher: {fisher.FisherName}";
            lblLandingSite.Content = $"Landing site: {fisher.LandingSite.LandingSiteName}, {fisher.LandingSite.Municipality.ToString()}";
            _fisher = fisher;
            _readOnly = readOnly;
        }

        private void EnableAddButton()
        {
            btnAdd.IsEnabled = Entities.BSCEntities.FisherGPSViewModel.CanAssignGPS(_fisher);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //gridAssignedGPS.ItemsSource = TheEntities.FisherGPSViewModel.GetFisherGPSByFisher(_fisher.FisherID);
            //gridAssignedGPS.ItemsSource = Entities.BSCEntities.FisherGPSViewModel.FisherGPSCollection.OrderByDescending(t=>t.DateAssigned);
            RefreshGridSource();
            EnableAddButton();
            btnDelete.IsEnabled = false;
            btnEdit.IsEnabled = false;
            if(_readOnly)
            {
                gridEditButtons.Visibility = Visibility.Collapsed;
            }
        }

        private void gridAssignedGPS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEdit.IsEnabled = gridAssignedGPS.SelectedCells.Count > 0;
            btnDelete.IsEnabled = gridAssignedGPS.SelectedCells.Count > 0;
        }

        ///This method is save the actual position of the window to file "WindowName.pos"
        private void ClosingTrigger(object sender, EventArgs e)
        {
            this.SavePlacement();
        }
        ///This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        private void RefreshGridSource()
        {
            //gridAssignedGPS.ItemsSource = Entities.BSCEntities.FisherGPSViewModel.FisherGPSCollection.OrderByDescending(t=>t.DateAssigned);
            gridAssignedGPS.ItemsSource = Entities.BSCEntities.FisherGPSViewModel.GetAllFisherGPS(_fisher).OrderByDescending(t => t.DateAssigned);
            if (_parentWindow != null)
            {
                _parentWindow.RefreshDataGridNeeded = true;
            }
        
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddEditWindow aew;
            switch (((Button)sender).Name)
            {
                case "btnClose":
                    Close();
                    break;
                case "btnEdit":
                    aew = new AddEditWindow(isNew: false, f: _fisher, ((FisherGPS)gridAssignedGPS.SelectedItem).RowID);
                    aew.ShowDialog();
                    if(!aew.Cancelled)
                    {
                        //gridAssignedGPS.Items.Refresh();
                        RefreshGridSource();
                        EnableAddButton();
                    }
                    break;
                case "btnDelete":
                    FisherGPS fg = (FisherGPS)gridAssignedGPS.SelectedItem;
                    if (BSCEntities.FisherGPSViewModel.CanDeleteEntity(fg))
                    {
                        Entities.BSCEntities.FisherGPSViewModel.DeleteRecordFromRepo(fg.RowID);
                        RefreshGridSource();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Cannot delete selected item because it is used in the sampling table", "Cannot delete", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    //gridAssignedGPS.Items.Refresh();

                    break;
                case "btnAdd":
                    aew = new AddEditWindow(isNew: true, f: _fisher, null);
                    aew.ShowDialog();
                    if(!aew.Cancelled)
                    {
                        //gridAssignedGPS.ItemsSource = BSCEntities.FisherGPSViewModel.GetFisherGPSByFisher(_fisher);
                        //gridAssignedGPS.Items.Refresh();
                        RefreshGridSource();
                        EnableAddButton();
                    }
                    break;
            }
        }
    }
}
