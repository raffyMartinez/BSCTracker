using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for SamplingHistoryWindow.xaml
    /// </summary>
    public partial class SamplingHistoryWindow : Window
    {
        private Sampling _sampling;
        public bool Cancelled { get; set; }
        public Fisher Fisher { get; set; }
        public LandingSite LandingSite {get;set;}
        public Gear Gear { get; set; }
        public string EntityType { get; internal set; }
        public SamplingHistoryWindow(string entityType)
        {
            InitializeComponent();
            EntityType = entityType;
            Loaded += OnWindowLoaded;
            Closing += ClosingTrigger;
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

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            buttonDelete.IsEnabled = false;
            buttonEdit.IsEnabled = false;
            string labelTitleCaption = "";
            switch (EntityType)
            {
                case "Fisher":
                    Fisher = ((StartWindow)Owner).Fisher;
                    labelTitleCaption = $"Sampling history of fisher: {Fisher.FisherName}";
                    break;
                case "LandingSite":
                    LandingSite = ((StartWindow)Owner).LandingSite;
                    labelTitleCaption = $"Sampling history of landing site: {LandingSite}";
                    break;
                case "Gear":
                    Gear = ((StartWindow)Owner).Gear;
                    labelTitleCaption = $"Sampling history of fishing gear: {Gear.GearName}";
                    break;
            }
            RefreshGridSource();
            lblTitle.Content = labelTitleCaption;
            Title = labelTitleCaption;


        }
        private void RefreshGridSource()
        {
            switch (EntityType)
            {
                case "Fisher":
                    dataGrid.ItemsSource = BSCEntities.SamplingViewModel.GetAllSamplings(Fisher);
                    break;
                case "LandingSite":
                    dataGrid.ItemsSource = BSCEntities.SamplingViewModel.GetAllSamplings(LandingSite);
                    break;
                case "Gear":
                    dataGrid.ItemsSource = BSCEntities.SamplingViewModel.GetAllSamplings(Gear);
                    break;
            }
            if (dataGrid.Items.Count == 0)
            {
                lblTitle.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Collapsed;
                lblNoRecords.Visibility = Visibility.Visible;
                panelLowerButtons.Visibility = Visibility.Collapsed;
                Title = "No records found";
            }

        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            switch(((Button)sender).Name)
            {
                case "buttonClose":
                    Close();
                    break;
                case "buttonDelete":
                    BSCEntities.SamplingViewModel.DeleteRecordFromRepo(_sampling.RowID);
                    if(BSCEntities.SamplingViewModel.EditedEntity!=null && BSCEntities.SamplingViewModel.EditedEntity.EditAction==EditAction.Delete)
                    {
                        RefreshGridSource();
                    }
                    break;
                case "buttonEdit":


                    AddEditWindow aew = new AddEditWindow("Sampling", false, _sampling.RowID);
                    aew.ShowDialog();
                    if(!aew.Cancelled)
                    {
                        RefreshGridSource();
                    }
                    break;
            }
        }

        private void OnGridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEdit.IsEnabled = dataGrid.SelectedCells.Count > 0;
            buttonDelete.IsEnabled = dataGrid.SelectedCells.Count > 0;

            if (dataGrid.SelectedItem != null)
            {
                _sampling = (Sampling)dataGrid.SelectedItem;
            }

        }    
    }
}
