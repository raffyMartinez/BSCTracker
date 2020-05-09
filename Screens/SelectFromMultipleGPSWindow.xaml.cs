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
    /// Interaction logic for SelectFromMultipleGPSWindow.xaml
    /// </summary>
    /// 

    public partial class SelectFromMultipleGPSWindow : Window
    {
        private bool _okButtonClicked;
        public bool Cancelled { get; internal set; }
        public FisherGPS FisherGPS { get; internal set; }
        private AssignedGPSDetails _assignedGPSDetails;
        public SelectFromMultipleGPSWindow(AssignedGPSDetails assignedGPSDEtails)
        {
            InitializeComponent();
            _assignedGPSDetails = assignedGPSDEtails;
            Loaded += OnWindowLoaded;

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {

            foreach (var fg in _assignedGPSDetails.FisherGPSList)
            {
                RadioButton rdb = new RadioButton()
                {
                    Tag = fg,
                    Content = $"From {((DateTime)fg.DateAssigned).ToString("MMM-dd-yyyy")} to {(fg.DateReturned == null ? "present date" : ((DateTime)fg.DateReturned).ToString("MMM-dd-yyyy"))}",
                    Margin = new Thickness(5)
                };
                stackPanel.Children.Add(rdb);
            }
            Closing += ClosingTrigger;
        }

        private void ClosingTrigger(object sender, EventArgs e)
        {
            Cancelled = !_okButtonClicked;
            this.SavePlacement();
        }
        ///This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "buttonOk":
                    _okButtonClicked = true;
                    bool hasCheck = false;
                    foreach(var child in stackPanel.Children.OfType<RadioButton>())
                    {
                        if((bool)child.IsChecked)
                        {
                            FisherGPS = (FisherGPS)child.Tag;
                            hasCheck = true;
                            break;
                        }
                    }

                    if(hasCheck)
                    {
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("You must select a date range", "Validation error", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                case "buttonCancel":
                    Cancelled = true;
                    Close();
                    break;
            }
        }
    }
}
