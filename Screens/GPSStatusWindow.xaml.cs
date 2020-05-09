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
    /// Interaction logic for GPSStatusWindow.xaml
    /// </summary>
    public partial class GPSStatusWindow : Window
    {
        public string Topic { get; internal set; }
        public GPSStatusWindow(string topic)
        {
            InitializeComponent();
            Topic = topic;
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Entities.BSCEntities.GPSViewModel == null)
            {
                Entities.BSCEntities.GPSViewModel = new GPSViewModel();
            }
            switch (Topic)
            {
                case "assignment":
                    labelTop.Content = "Available GPS";
                    gridHistory.Visibility = Visibility.Collapsed;
                    gridAvailableGPS.ItemsSource = Entities.BSCEntities.GPSViewModel.AvailableGPS();
                    gridAssignedGPS.ItemsSource = BSCEntities.FisherGPSViewModel.GetAssignedGPS()
                        .OrderByDescending(t => t.DateGPSAssigned);
                    break;

                case "gps_history":
                    labelBottom.Visibility = Visibility.Collapsed;
                    labelTop.Content = "History of GPS assignments";
                    gridAvailableGPS.Visibility = Visibility.Collapsed;
                    gridAssignedGPS.Visibility = Visibility.Collapsed;
                    gridHistory.ItemsSource= BSCEntities.FisherGPSViewModel.GetAllFisherGPS()
                        .OrderBy(t=>t.GPS.ToString())
                        .ThenByDescending(t=>t.DateAssigned);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "ButtonOk":
                    Close();
                    break;
            }
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
    }
}
