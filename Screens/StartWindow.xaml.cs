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
using System.Collections.ObjectModel;
using BSCTracker.TreeViewModelControl;
using BSCTracker.Utilities;

namespace BSCTracker.Screens
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        private string _entityType;
        private string _entityID;
        private bool _samplingRadioButtonWasClicked;

        public Sampling Sampling { get; set; }
        public LandingSite LandingSite { get; set; }
        public Fisher Fisher { get; set; }
        public FisherGPS FisherGps { get; set; }
        public ProjectSetting ProjectSetting { get; set; }

        public GPS GPS { get; set; }
        public Gear Gear { get; set; }

        public string TreeViewEntity { get; set; }

        public DateTime? MonthSampled { get; set; }
        public StartWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Global.AppProceed)
            {
                gridData.Visibility = Visibility.Visible;
                panelEditButtons.Visibility = Visibility.Visible;
                buttonDelete.IsEnabled = false;
                buttonEdit.IsEnabled = false;
                buttonSubGrid.IsEnabled = false;
                buttonSubGrid.Visibility = Visibility.Hidden;




                BSCEntities.ProvinceViewModel = new ProvinceViewModel();
                BSCEntities.MunicipalityViewModel = new MunicipalityViewModel();

                BSCEntities.LandingSiteViewModel = new LandingSiteViewModel();
                BSCEntities.LandingSiteViewModel.EntityChanged += OnBSCEntity_Changed;

                BSCEntities.ProjectSettingViewModel = new ProjectSettingViewModel();
                BSCEntities.ProjectSettingViewModel.EntityChanged += OnBSCEntity_Changed;

                BSCEntities.GPSViewModel = new GPSViewModel();
                BSCEntities.FisherViewModel = new FisherViewModel();
                BSCEntities.FisherGPSViewModel = new FisherGPSViewModel();
                BSCEntities.GearViewModel = new GearViewModel();



                BSCEntities.SamplingViewModel = new SamplingViewModel();

                rbSampling.GotFocus += RbSampling_GotFocus;

                rbDBSummary.IsChecked = true;
                OnEntityButtonClick(rbDBSummary, null);
            }
            else

            {
                lblContentTitle.Content = "Backend database not found";
                lblContentTitle.FontSize = 24;
                lblContentTitle.FontWeight = FontWeights.Bold;
                lblContentTitle.VerticalAlignment = VerticalAlignment.Center;
                lblContentTitle.HorizontalAlignment = HorizontalAlignment.Center;
                rowTop.Height = new GridLength(50, GridUnitType.Star);
                stackPanel.Visibility = Visibility.Collapsed;
                panelEditButtons.Visibility = Visibility.Collapsed;
                dockPanelLeft.Visibility = Visibility.Collapsed;
            }

        }

        private void RbSampling_GotFocus(object sender, RoutedEventArgs e)
        {
            _samplingRadioButtonWasClicked = true;
        }


        private void OnBSCEntity_Changed(object sender, EntityChangedEventArgs e)
        {
            switch (e.EntityType)
            {
                case "LandingSite":
                    BSCEntities.FisherViewModel.RefreshFisherLandingSite((LandingSite)e.BSCEntity);
                    break;
                case "ProjectSetting":
                    break;
            }

        }

        private void OnMenuMain_Click(object sender, RoutedEventArgs e)
        {
            switch (((MenuItem)sender).Name)
            {
                case "menuExit":
                    Close();
                    break;
                case "menuAboutThisApp":
                    break;
            }
        }

        private void OnEntityButtonClick(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn col;
            buttonSubGrid.Visibility = Visibility.Collapsed;
            buttonSubGrid1.Visibility = Visibility.Collapsed;
            panelEditButtons.Visibility = Visibility.Visible;
            topGrid.Visibility = Visibility.Collapsed;

            gridData.Visibility = Visibility.Visible;
            gridData.Columns.Clear();

            switch (((RadioButton)sender).Name)
            {
                case "rbSampling":
                    _entityType = "Sampling";
                    buttonSubGrid.Visibility = Visibility.Collapsed;
                    lblContentTitle.Content = "Sampling (Latest 10 rows added only)";
                    refreshDataGrid(dataWasEdited: false);
                    gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("RowID"), Visibility = Visibility.Hidden });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Project", Binding = new Binding("ProjectSetting") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSite") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Fisher", Binding = new Binding("FisherGPS.Fisher.FisherName") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("FisherGPS.GPS") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Gear", Binding = new Binding("Gear.GearName") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "NSAP sampling ID", Binding = new Binding("NSAPSamplingID") });


                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeDeparted"),
                        Header = "Date departed"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    gridData.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeArrived"),
                        Header = "Date arrived"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    gridData.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateTimeSampled"),
                        Header = "Date sampled"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    gridData.Columns.Add(col);

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateAdded"),
                        Header = "Date added"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy HH:mm";
                    gridData.Columns.Add(col);

                    break;
                case "rbDBSummary":
                    _entityType = "DBSummary";
                    lblContentTitle.Content = "Database summary";
                    panelEditButtons.Visibility = Visibility.Collapsed;
                    gridData.Visibility = Visibility.Collapsed;
                    topGrid.Visibility = Visibility.Visible;

                    DBSummary.UpdateSummary();
                    topGrid.AutoGenerateColumns = true;

                    if (topGrid.ItemsSource == null)
                    {
                        topGrid.ItemsSource = DBSummary.SummaryValues;
                    }
                    else

                    {
                        topGrid.Items.Refresh();
                    }

                    topGrid.Columns[0].Header = "Summary item";

                    break;
                case "rbGear":
                    _entityType = "Gear";
                    lblContentTitle.Content = "Fishing gears";
                    buttonSubGrid1.Content = "Gear sampling records";

                    buttonSubGrid1.Visibility = Visibility.Visible;
                    buttonSubGrid1.IsEnabled = false;

                    gridData.ItemsSource = BSCEntities.GearViewModel.GearCollection;
                    gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("GearID"), Visibility = Visibility.Hidden });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("GearName") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Code", Binding = new Binding("Code") });
                    break;
                case "rbFisher":
                    _entityType = "Fisher";
                    buttonSubGrid.Content = "GPS assignment";
                    buttonSubGrid1.Content = "Fisher sampling records";
                    buttonSubGrid.Visibility = Visibility.Visible;
                    buttonSubGrid.IsEnabled = false;

                    buttonSubGrid1.Visibility = Visibility.Visible;
                    buttonSubGrid1.IsEnabled = false;

                    lblContentTitle.Content = "Fishers with assigned GPS";
                    //gridData.ItemsSource = BSCEntities.FisherViewModel.GetFishersAndGPS();
                    refreshDataGrid();
                    //gridData.ItemsSource = Entities.BSCEntities.FisherViewModel.FisherCollection;
                    //gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("FisherID"), Visibility = Visibility.Hidden });
                    //gridData.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("FisherName") });
                    //gridData.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("LandingSite") });
                    //gridData.Columns.Add(new DataGridTextColumn { Header = "Fishing boat", Binding = new Binding("FishingBoatName") });

                    gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Fisher.FisherID"), Visibility = Visibility.Hidden });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Fisher.FisherName") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Landing site", Binding = new Binding("Fisher.LandingSite") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Fishing boat", Binding = new Binding("Fisher.FishingBoatName") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "GPS", Binding = new Binding("GPS") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Project", Binding = new Binding("ProjectSetting") });
                    break;
                case "rbGPS":
                    _entityType = "GPS";
                    buttonSubGrid.Content = "Show GPS availability";
                    buttonSubGrid.Visibility = Visibility.Visible;
                    buttonSubGrid.IsEnabled = true;

                    buttonSubGrid1.Content = "History of GPS assignments";
                    buttonSubGrid1.Visibility = Visibility.Visible;
                    buttonSubGrid1.IsEnabled = true;

                    lblContentTitle.Content = "GPS";

                    gridData.ItemsSource = Entities.BSCEntities.GPSViewModel.GPSCollection;
                    gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID"), Visibility = Visibility.Hidden });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Brand", Binding = new Binding("Brand") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new Binding("Model") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Assigned name", Binding = new Binding("AssignedName") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateAcquired"),
                        Header = "Date acquired"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    gridData.Columns.Add(col);

                    gridData.Columns.Add(new DataGridTextColumn { Header = "SD card capacity", Binding = new Binding("SDCardCapacity") });
                    break;
                case "rbLandingSite":
                    _entityType = "LandingSite";
                    lblContentTitle.Content = "Landing sites";
                    buttonSubGrid1.Content = "Landing site sampling records";

                    buttonSubGrid1.Visibility = Visibility.Visible;
                    buttonSubGrid1.IsEnabled = false;

                    gridData.ItemsSource = Entities.BSCEntities.LandingSiteViewModel.LandingSiteCollection;
                    gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("LandingSiteID"), Visibility = Visibility.Hidden });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("LandingSiteName") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Waypoint", Binding = new Binding("Waypoint") });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Municipality", Binding = new Binding("Municipality") });
                    break;
                case "rbProject":
                    _entityType = "ProjectSetting";


                    gridData.ItemsSource = Entities.BSCEntities.ProjectSettingViewModel.ProjectSettingCollection;
                    gridData.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ProjectID"), Visibility = Visibility.Hidden });
                    gridData.Columns.Add(new DataGridTextColumn { Header = "Project name", Binding = new Binding("ProjectName") });

                    col = new DataGridTextColumn()
                    {
                        Binding = new Binding("DateStart"),
                        Header = "Date started"
                    };
                    col.Binding.StringFormat = "MMM-dd-yyyy";
                    gridData.Columns.Add(col);
                    break;
            }
        }

        private void OnGridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonEdit.IsEnabled = gridData.SelectedCells.Count > 0;
            buttonDelete.IsEnabled = gridData.SelectedCells.Count > 0;

            switch (_entityType)
            {
                case "Fisher":
                    buttonSubGrid.IsEnabled = gridData.SelectedCells.Count > 0;
                    buttonSubGrid1.IsEnabled = buttonSubGrid.IsEnabled;
                    break;
                case "LandingSite":
                case "Gear":
                    buttonSubGrid1.IsEnabled = gridData.SelectedCells.Count > 0;
                    break;

            }

            if (gridData.SelectedItem != null)
            {
                switch (_entityType)
                {
                    case "Sampling":
                        Sampling = (Sampling)gridData.SelectedItem;
                        _entityID = Sampling.RowID;
                        break;
                    case "Gear":
                        Gear = (Gear)gridData.SelectedItem;
                        _entityID = Gear.GearID;
                        break;
                    case "ProjectSetting":
                        ProjectSetting = (ProjectSetting)gridData.SelectedItem;
                        _entityID = ProjectSetting.ProjectID;
                        break;
                    case "Fisher":
                        FisherAndGPS fg = (FisherAndGPS)gridData.SelectedItem;
                        Fisher = fg.Fisher;
                        _entityID = fg.Fisher.FisherID;
                        break;
                    case "LandingSite":
                        LandingSite = (LandingSite)gridData.SelectedItem;
                        _entityID = LandingSite.LandingSiteID;

                        break;
                    case "GPS":
                        GPS = (GPS)gridData.SelectedItem;
                        _entityID = GPS.ID;
                        break;
                }
            }

        }
        public bool RefreshDataGridNeeded { get; set; }

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;

            switch (buttonName)
            {

                case "buttonSubGrid1":
                    switch (_entityType)
                    {
                        case "LandingSite":
                        case "Gear":
                        case "Fisher":
                            SamplingHistoryWindow shw = new SamplingHistoryWindow(_entityType);
                            shw.Owner = this;
                            shw.ShowDialog();
                            break;

                        case "GPS":
                            GPSStatusWindow gsw = new GPSStatusWindow("gps_history");
                            gsw.Owner = this;
                            gsw.Show();
                            break;
                    }


                    break;
                case "buttonSubGrid":
                    switch (_entityType)
                    {
                        case "Fisher":
                            FisherAndGPS fg = (FisherAndGPS)gridData.SelectedItem;
                            FisherGPSWindow fgw = new FisherGPSWindow(fisher: fg.Fisher, readOnly: false, parentWindow: this);
                            //FisherGPSWindow fgw = new FisherGPSWindow((Fisher)gridData.SelectedItem);
                            fgw.ShowDialog();

                            if (!fgw.Cancelled && RefreshDataGridNeeded)
                                refreshDataGrid();

                            break;
                        case "GPS":
                            GPSStatusWindow gsw = new GPSStatusWindow("assignment");
                            gsw.Owner = this;
                            gsw.Show();
                            break;
                    }
                    //buttonSubGrid.IsEnabled = false;

                    break;
                case "buttonAdd":
                    AddEditWindow aew = new AddEditWindow(_entityType, true);
                    aew.ShowDialog();
                    if (!aew.Cancelled)
                    {
                        refreshDataGrid();
                    }
                    break;
                case "buttonEdit":
                    aew = new AddEditWindow(_entityType, false, _entityID);
                    aew.ShowDialog();
                    if (!aew.Cancelled)
                    {
                        refreshDataGrid();
                    }
                    break;
                case "buttonDelete":
                    string msgCannotDelete = "";
                    bool deleteSuccess = false;
                    switch (_entityType)
                    {
                        case "Gear":
                            if (BSCEntities.GearViewModel.CanDeleteEntity(Gear))
                            {
                                BSCEntities.GearViewModel.DeleteRecordFromRepo(Gear.GearName);
                                deleteSuccess = true;
                            }
                            else
                            {
                                msgCannotDelete = $"Cannot delete the gear '{Gear.GearName}' because it is already used by the database";
                            }
                            break;
                        case "Fisher":
                            if (Entities.BSCEntities.FisherViewModel.CanDeleteEntity(Fisher))
                            {
                                Entities.BSCEntities.FisherViewModel.DeleteRecordFromRepo(_entityID);
                                deleteSuccess = true;
                            }
                            else
                            {
                                msgCannotDelete = $"Cannot delete the fisher '{Fisher.FisherName}' because it is already used by the database";
                            }
                            break;
                        case "LandingSite":
                            if (BSCEntities.LandingSiteViewModel.CanDeleteEntity(LandingSite))
                            {
                                Entities.BSCEntities.LandingSiteViewModel.DeleteRecordFromRepo(LandingSite.LandingSiteID);
                            }
                            else
                            {
                                msgCannotDelete = $"Cannot delete the GPS '{LandingSite.ToString()}' because it is already used by the database";
                            }
                            break;
                        case "GPS":
                            if (Entities.BSCEntities.GPSViewModel.CanDeleteEntity(GPS))
                            {
                                Entities.BSCEntities.GPSViewModel.DeleteRecordFromRepo(_entityID);
                                deleteSuccess = true;
                            }
                            else
                            {
                                msgCannotDelete = $"Cannot delete the GPS '{GPS.ToString()}' because it is already used by the database";
                            }
                            break;
                        case "ProjectSetting":
                            if (Entities.BSCEntities.ProjectSettingViewModel.CanDeleteEntity(ProjectSetting))
                            {

                                Entities.BSCEntities.ProjectSettingViewModel.DeleteRecordFromRepo(ProjectSetting.ProjectID);
                                deleteSuccess = true;
                            }
                            else
                            {
                                msgCannotDelete = $"Cannot delete the project '{ProjectSetting.ProjectName}' because it is already used by the database";
                            }
                            break;
                    }

                    if (deleteSuccess)
                    {
                        //gridData.Items.Refresh();
                        refreshDataGrid();
                    }
                    else
                    {
                        if (msgCannotDelete.Length > 0)
                        {
                            MessageBox.Show(msgCannotDelete, "Validation error", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    break;
            }
        }

        public void refreshDataGrid(bool dataWasEdited = true)
        {
            switch (_entityType)
            {
                case "Sampling":
                    if (_samplingRadioButtonWasClicked)
                    {
                        gridData.ItemsSource = Entities.BSCEntities.SamplingViewModel.SamplingCollection
                            .OrderByDescending(p => p.DateAdded).Take(10);
                    }
                    else
                    {
                        gridData.ItemsSource = BSCEntities.SamplingViewModel.GetAllSamplings(ProjectSetting, LandingSite, Gear, (DateTime)MonthSampled)
                            .OrderByDescending(p => p.DateAdded);
                    }

                    //when a sampling is added or edited, the tree is refreshed to reflect the latest edits to the samplings
                    if (dataWasEdited)
                        samplingTree.treeControl.Refresh(BSCEntities.SamplingViewModel.EditedEntity);

                    break;
                case "Fisher":
                    gridData.ItemsSource = gridData.ItemsSource = BSCEntities.FisherViewModel.GetFishersAndGPS();
                    break;
                default:
                    if (_entityType == "ProjectSetting")
                    {
                        samplingTree.treeControl.Refresh(BSCEntities.ProjectSettingViewModel.EditedEntity);
                    }
                    gridData.Items.Refresh();
                    break;
            }
        }
        private void OnTreeViewItemSelected(object sender, AllSamplingEntitiesEventHandler e)
        {
            TreeViewEntity = e.TreeViewEntity;
            ProjectSetting = e.ProjectSetting;
            LandingSite = e.LandingSite;
            Gear = e.Gear;
            MonthSampled = e.MonthSampled;
            switch (TreeViewEntity)
            {
                case "tv_ProjectSettingViewModel":
                    break;
                case "tv_LandingSiteViewModel":
                    break;
                case "tv_GearViewModel":
                    break;
                case "tv_MonthFishingViewModel":

                    _samplingRadioButtonWasClicked = false;
                    rbSampling.IsChecked = true;
                    OnEntityButtonClick(rbSampling, null);
                    lblContentTitle.Content = $"{ProjectSetting.ProjectName} samplings in {LandingSite.ToString()}, caught using {Gear.GearName} on {((DateTime)MonthSampled).ToString("MMM-yyyy")}";
                    break;
            }

        }

        private void OnGridTop_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
