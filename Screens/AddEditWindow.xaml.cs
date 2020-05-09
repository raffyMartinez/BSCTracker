using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Xceed.Wpf.Toolkit;
using BSCTracker.Utilities;

namespace BSCTracker.Screens
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private LandingSite _oldLandingSite;

        private bool _saveButtonClicked = false;
        private Dictionary<GPS, AssignedGPSDetails> _assignedGPSToFisher;
        private Province Province { get; set; }
        private ProjectSetting ProjectSetting { get; set; }
        private ComboBox _projectSettingCombo;
        private ComboBox _municipalityCombo;
        private ComboBox _fisherCombo;
        private ComboBox _gpsCombo;
        private ComboBox _gearCombo;
        private ComboBox _landingSiteCombo;

        private GPS GPS { get; set; }
        private Sampling Sampling { get; set; }
        private Fisher Fisher { get; set; }
        private LandingSite LandingSite { get; set; }
        private FisherGPS _fisherGPS;
        private Gear Gear { get; set; }

        private string _oldName;
        private string _oldCode;
        public Waypoint Waypoint { get; set; }
        public bool Cancelled = false;
        public string EntityType { get; set; }
        public bool IsNew { get; set; }


        /// <summary>
        /// Constructor for editing/adding FisherGPS entity
        /// </summary>
        /// <param name="isNew"></param>
        /// <param name="f"></param>
        /// <param name="rowID"></param>
        public AddEditWindow(bool isNew, Fisher f, string rowID)
        {
            InitializeComponent();
            EntityType = "FisherGPS";
            Fisher = f;
            IsNew = isNew;
            if (IsNew)
            {
                _fisherGPS = new FisherGPS();
                _fisherGPS.Fisher = f;
                Title = "Add a new GPS assignment to fisher";
            }
            else
            {
                if (rowID == null)
                {
                    throw new ArgumentNullException("Error: rowID is Null");
                }
                else
                {
                    FisherGPS tempFisherGPS = Entities.BSCEntities.FisherGPSViewModel.GetFisherGPS(rowID);

                    //deep copy of FisherGPS
                    _fisherGPS = new FisherGPS(tempFisherGPS.RowID, tempFisherGPS.GPS, tempFisherGPS.Fisher, tempFisherGPS.ProjectSetting, tempFisherGPS.DateAssigned, tempFisherGPS.DateReturned);
                    ProjectSetting = _fisherGPS.ProjectSetting;

                    Title = "Edit GPS assignment";
                }
            }
            _gpsCombo= (ComboBox)AddRowToGrid(labelCaption: "GPS", controlType: "comboBox", buttonLabel: "Details of GPS");
            _projectSettingCombo = (ComboBox)AddRowToGrid("Project", "comboBox");
            AddRowToGrid("Date assigned", "dateTimePicker");
            AddRowToGrid("Date returned", "dateTimePicker");
            if (!IsNew)
            {
                int rows = gridFields.RowDefinitions.Count;
                for (int n = 0; n < rows; n++)
                {
                    var ctl = gridFields.Children
                        .Cast<UIElement>()
                        .First(c => Grid.GetRow(c) == n && Grid.GetColumn(c) == 1);
                    string ctlName = ((Control)ctl).Name;

                    switch (ctlName)
                    {
                        case "cboGPS":
                            ((ComboBox)ctl).Text = _fisherGPS.GPS.ToString();
                            break;
                        case "cboProject":
                            if (_fisherGPS.ProjectSetting != null)
                            {
                                ((ComboBox)ctl).Text = _fisherGPS.ProjectSetting.ToString();
                            }
                            break;
                        case "dtpDate_assigned":
                            ((DateTimePicker)ctl).Text = ((DateTime)_fisherGPS.DateAssigned).ToString("MMM-dd-yyyy");
                            break;
                        case "dtpDate_returned":
                            if (_fisherGPS.DateReturned != null)
                            {
                                ((DateTimePicker)ctl).Text = ((DateTime)_fisherGPS.DateReturned).ToString("MMM-dd-yyyy");
                            }
                            break;
                    }
                }
            }

        }

        /// <summary>
        /// Constructor for adding a Waypoint entity
        /// </summary>
        /// <param name="isNew"></param>
        /// <param name="wpt"></param>
        public AddEditWindow(bool isNew, Waypoint wpt)
        {
            InitializeComponent();

            TextBox txt = (TextBox) AddRowToGrid("Waypoint name");
            AddRowToGrid("Longitude");
            AddRowToGrid("Latitude");
            AddRowToGrid(labelCaption: "Date acquired", controlType: "dateTimePicker", showTimePart: true);

            EntityType = "Waypoint";
            IsNew = isNew;
            if (IsNew)
            {
                Waypoint = new Waypoint();
                Title = "Add a new waypoint";
            }
            else
            {
                if (wpt == null)
                {
                    throw new ArgumentNullException("Error: GPS is Null");
                }
                else
                {
                    Waypoint = wpt;
                    Title = "Edit waypoint";

                    int rows = gridFields.RowDefinitions.Count;
                    for (int n = 0; n < rows; n++)
                    {
                        var ctl = gridFields.Children
                            .Cast<UIElement>()
                            .First(c => Grid.GetRow(c) == n && Grid.GetColumn(c) == 1);
                        string ctlName = ((Control)ctl).Name;

                        switch (ctlName)
                        {
                            case "txtWaypoint_name":
                                ((TextBox)ctl).Text = Waypoint.Name;
                                break;
                            case "txtLongitude":
                                ((TextBox)ctl).Text = Waypoint.X.ToString();
                                break;
                            case "txtLatitude":
                                ((TextBox)ctl).Text = Waypoint.Y.ToString();
                                break;
                            case "dtpDate_acquired":
                                ((DateTimePicker)ctl).Text = Waypoint.TimeStamp.ToString("MMM-dd-yyyy HH:mm");
                                break;
                        }
                    }
                }
            }

            txt.Focus();
        }
        public AddEditWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// constructor for adding either a Fisher, Sampling, GPS, Gear, ProjectSetting, or LandingSite entity
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="isNew"></param>
        /// <param name="entityID"></param>
        public AddEditWindow(string entityType, bool isNew = false, string entityID = "")
        {
            InitializeComponent();
            IsNew = isNew;
            EntityType = entityType;
            switch (EntityType)
            {
                case "Sampling":
                    if (IsNew)
                    {
                        Sampling = new Sampling();
                        Title = "Add a new sampling";
                    }
                    else
                    {
                        Sampling tempSampling = Entities.BSCEntities.SamplingViewModel.GetSampling(entityID);

                        //deep copy of ProjectSetting entity
                        Sampling = new Sampling(tempSampling.FisherGPS, tempSampling.LandingSite, tempSampling.RowID, tempSampling.Gear,
                                                 tempSampling.DateTimeDeparted, tempSampling.DateTimeArrived, tempSampling.NSAPSamplingID,
                                                 tempSampling.ProjectSetting, tempSampling.DateTimeSampled, tempSampling.DateAdded);

                        Title = "Edit sampling";
                    }
                    _projectSettingCombo = (ComboBox)AddRowToGrid("Project", "comboBox");
                    _fisherCombo = (ComboBox)AddRowToGrid(labelCaption: "Fisher", controlType: "comboBox", buttonLabel: "Assigned GPS");
                    _landingSiteCombo = (ComboBox)AddRowToGrid("Landing site", "comboBox");
                    _gpsCombo = (ComboBox)AddRowToGrid(labelCaption: "GPS", controlType: "comboBox", buttonLabel: "Details of GPS");
                    _gearCombo = (ComboBox)AddRowToGrid("Gear name", "comboBox");
                    AddRowToGrid("NSAP sample identifier");
                    AddRowToGrid(labelCaption: "Date and time departed", controlType: "dateTimePicker", showTimePart: true);
                    AddRowToGrid(labelCaption: "Date and time arrived", controlType: "dateTimePicker", showTimePart: true);
                    AddRowToGrid(labelCaption: "Date and time sampled", controlType: "dateTimePicker", showTimePart: true);

                    break;
                case "Gear":
                    if (IsNew)
                    {
                        Gear = new Gear();
                        Title = "Add a new fishing gear";
                    }
                    else
                    {
                        Gear tempGear = Entities.BSCEntities.GearViewModel.GetGear(entityID);
                        _oldName = tempGear.GearName;
                        _oldCode = tempGear.Code;

                        //deep copy of ProjectSetting entity
                        Gear = new Gear(tempGear.GearName, tempGear.Code, tempGear.GearID);

                        Title = "Edit fishing gear";
                    }
                    TextBox txt = (TextBox) AddRowToGrid("Gear name");
                    AddRowToGrid("Code");

                    txt.Focus();
                    
                    break;
                case "ProjectSetting":
                    if (IsNew)
                    {
                        ProjectSetting = new ProjectSetting();
                        Title = "Add a new BSC Project";
                    }
                    else
                    {
                        ProjectSetting tempProjectSetting = Entities.BSCEntities.ProjectSettingViewModel.GetProjectSetting(entityID);
                        _oldName = tempProjectSetting.ProjectName;

                        //deep copy of ProjectSetting entity
                        ProjectSetting = new ProjectSetting(tempProjectSetting.ProjectID, tempProjectSetting.ProjectName, tempProjectSetting.DateStart);

                        Title = "Edit BSC Project";
                    }
                    txt = (TextBox) AddRowToGrid("Project name");
                    AddRowToGrid("Date started", "dateTimePicker");

                    txt.Focus();
                    
                    break;
                case "Fisher":
                    if (IsNew)
                    {
                        Fisher = new Fisher();
                        Title = "Add a new fisher";
                    }
                    else
                    {
                        Fisher tempFisher = Entities.BSCEntities.FisherViewModel.GetFisher(entityID);
                        _oldName = tempFisher.FisherName;

                        //deep copy of Fisher entity
                        Fisher = new Fisher(tempFisher.FisherID, tempFisher.FisherName, tempFisher.LandingSite);
                        Fisher.FishingBoatName = tempFisher.FishingBoatName;

                        Title = "Edit Fisher";
                    }
                     txt = (TextBox) AddRowToGrid("Fisher name");
                    _landingSiteCombo = (ComboBox)AddRowToGrid("Landing site", "comboBox");
                    AddRowToGrid("Fishing boat name");

                    txt.Focus();
                    break;
                case "GPS":
                    GPS = new GPS();
                    if (IsNew)
                    {
                        GPS = new GPS();
                        Title = "Add a new GPS";
                    }
                    else
                    {
                        GPS tempGPS = Entities.BSCEntities.GPSViewModel.GetGPS(entityID);
                        _oldName = tempGPS.AssignedName;

                        //deepcopy of GPS object
                        GPS = new GPS(tempGPS.ID, tempGPS.Brand, tempGPS.Model, tempGPS.AssignedName, tempGPS.SDCardCapacity, tempGPS.DateAcquired);

                    }
                     txt = (TextBox) AddRowToGrid("Brand");
                    AddRowToGrid("Model");
                    AddRowToGrid("Date acquired", "dateTimePicker");
                    AddRowToGrid("Assigned name");
                    AddRowToGrid("SD card size", "comboBox");

                    txt.Focus();
                    break;
                case "LandingSite":

                    if (IsNew)
                    {
                        LandingSite = new LandingSite();
                        Title = "Add a new landing site";
                    }
                    else
                    {
                        _oldLandingSite= Entities.BSCEntities.LandingSiteViewModel.GetLandingSite(entityID);
                        _oldName = _oldLandingSite.LandingSiteName;


                        //deep copy of LandingSite entity
                        LandingSite = new LandingSite(_oldLandingSite.LandingSiteID, _oldLandingSite.LandingSiteName, _oldLandingSite.Waypoint, _oldLandingSite.Municipality);

                        Title = "Edit landing site";
                    }
                     txt = (TextBox) AddRowToGrid("Landing site name");
                    AddRowToGrid(labelCaption: "Waypoint", buttonLabel: "Define", controlReadOnly: true);
                    AddRowToGrid("Province", "comboBox");
                    _municipalityCombo = (ComboBox)AddRowToGrid("Municipality", "comboBox");

                    txt.Focus();
                    break;
            }


            //if the data is to be edited (not new) then we assign entity values to the field
            if (!isNew)
            {
                int rows = gridFields.RowDefinitions.Count;
                for (int n = 0; n < rows; n++)
                {

                    //first we get the fields already in the grid
                    var ctl = gridFields.Children
                        .Cast<UIElement>()
                        .First(c => Grid.GetRow(c) == n && Grid.GetColumn(c) == 1);

                    //then we get the control name
                    string ctlName = ((Control)ctl).Name;

                    //assign values based on control name
                    switch (EntityType)
                    {
                        case "Sampling":
                            switch (ctlName)
                            {
                                case "cboProject":
                                    _projectSettingCombo.Text = Sampling.ProjectSetting.ToString();
                                    //OnComboLostFocus((ComboBox)ctl, null);
                                    //OnComboSelectionChanged((ComboBox)ctl, null);
                                    break;
                                case "cboLanding_site":
                                    _landingSiteCombo.Text = Sampling.LandingSite.ToString();
                                    LandingSite = Sampling.LandingSite;
                                    break;
                                case "cboFisher":
                                    _fisherCombo.Text = Sampling.FisherGPS.Fisher.FisherName;
                                    //OnComboLostFocus((ComboBox)ctl, null);
                                    //OnComboSelectionChanged((ComboBox)ctl, null);
                                    break;
                                case "cboGPS":
                                    _gpsCombo.Text = Sampling.FisherGPS.GPS.ToString();
                                    break;
                                case "cboGear_name":
                                    _gearCombo.Text = Sampling.Gear.GearName;
                                    break;
                                case "txtNSAP_sample_identifier":
                                    ((TextBox)ctl).Text = Sampling.NSAPSamplingID;
                                    break;
                                case "dtpDate_and_time_departed":
                                    ((DateTimePicker)ctl).Text = ((DateTime)Sampling.DateTimeDeparted).ToString("MMM-dd-yyyy HH:mm");
                                    break;
                                case "dtpDate_and_time_arrived":
                                    ((DateTimePicker)ctl).Text = ((DateTime)Sampling.DateTimeArrived).ToString("MMM-dd-yyyy HH:mm");
                                    break;
                                case "dtpDate_and_time_sampled":
                                    ((DateTimePicker)ctl).Text = ((DateTime)Sampling.DateTimeSampled).ToString("MMM-dd-yyyy HH:mm");
                                    break;
                            }
                            break;
                        case "Gear":
                            switch (ctlName)
                            {
                                case "txtGear_name":
                                    ((TextBox)ctl).Text = Gear.GearName;
                                    break;
                                case "txtCode":
                                    ((TextBox)ctl).Text = Gear.Code;
                                    break;
                            }
                            break;
                        case "ProjectSetting":
                            switch (ctlName)
                            {
                                case "txtProject_name":
                                    ((TextBox)ctl).Text = ProjectSetting.ProjectName;
                                    break;
                                case "dtpDate_started":
                                    ((DateTimePicker)ctl).Text = ((DateTime)ProjectSetting.DateStart).ToString("MMM-dd-yyyy");
                                    break;
                            }
                            break;
                        case "GPS":
                            switch (ctlName)
                            {
                                case "txtBrand":
                                    ((TextBox)ctl).Text = GPS.Brand;
                                    break;
                                case "txtModel":
                                    ((TextBox)ctl).Text = GPS.Model;
                                    break;
                                case "txtAssigned_name":
                                    ((TextBox)ctl).Text = GPS.AssignedName;
                                    break;
                                case "cboSD_card_size":
                                    ComboBox cbo = (ComboBox)ctl;
                                    cbo.Text = GPS.SDCardCapacity;
                                    break;
                                case "dtpDate_acquired":
                                    if (GPS.DateAcquired != null)
                                    {
                                        ((DateTimePicker)ctl).Text = ((DateTime)GPS.DateAcquired).ToString("MMM-dd-yyyy");
                                    }
                                    break;
                            }
                            break;
                        case "Fisher":
                            switch (ctlName)
                            {
                                case "txtFisher_name":
                                    ((TextBox)ctl).Text = Fisher.FisherName;
                                    break;
                                case "cboLanding_site":
                                    ((ComboBox)ctl).Text = Fisher.LandingSite.ToString();
                                    break;
                                case "txtFishing_boat_name":
                                    ((TextBox)ctl).Text = Fisher.FishingBoatName;
                                    break;
                            }
                            break;
                        case "LandingSite":
                            switch (ctlName)
                            {
                                case "txtLanding_site_name":
                                    ((TextBox)ctl).Text = LandingSite.LandingSiteName;
                                    break;
                                case "txtWaypoint":
                                    ((TextBox)ctl).Text = LandingSite.Waypoint.Name;
                                    break;
                                case "cboProvince":
                                    if (LandingSite.Municipality != null)
                                    {
                                        ((ComboBox)ctl).Text = LandingSite.Municipality.Province.ProvinceName;
                                        OnComboSelectionChanged((ComboBox)ctl, null);
                                        //OnComboLostFocus((ComboBox)ctl, null);
                                    }
                                    break;
                                case "cboMunicipality":
                                    if (LandingSite.Municipality != null)
                                        ((ComboBox)ctl).Text = LandingSite.Municipality.MunicipalityName;
                                    break;
                            }
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Method for building a column of labels, fields and buttons within a Grid container
        /// </summary>
        /// <param name="labelCaption">The caption of the label in the first column</param>
        /// <param name="controlType">Adds a control in the second column and is one of
        ///                           1. TextBox
        ///                           2. ComboBox
        ///                           3. DateTimePicker
        /// </param>
        /// <param name="controlReadOnly">specifies if the control is read only</param>
        /// <param name="buttonLabel">If present then a button with a label is added on the third column</param>
        /// <param name="showTimePart">Boolean, specifies if a DateTimePicker shows a time part</param>
        private Control AddRowToGrid(string labelCaption, string controlType = "textBox", bool controlReadOnly = false, string buttonLabel = "", bool showTimePart = false)
        {
            Control ctl = new Control();
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(30);
            gridFields.RowDefinitions.Add(rd);
            Label lbl = new Label();
            lbl.Content = labelCaption;
            gridFields.Children.Add(lbl);
            Grid.SetRow(lbl, gridFields.RowDefinitions.Count - 1);
            Grid.SetColumn(lbl, 0);

            switch (controlType)
            {
                case "dateTimePicker":
                    DateTimePicker dateTimePicker = new DateTimePicker();
                    //dateTimePicker.AllowTextInput = false;
                    dateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
                    dateTimePicker.InputValidationError += DateTimePicker_InputValidationError;
                    dateTimePicker.Format = DateTimeFormat.Custom;
                    dateTimePicker.ShowButtonSpinner = false;
                    dateTimePicker.TimePickerShowButtonSpinner = showTimePart;
                    if (showTimePart)
                    {

                        dateTimePicker.FormatString = "MMM-dd-yyyy HH:mm";
                        dateTimePicker.TimeFormat = DateTimeFormat.Custom;
                        dateTimePicker.TimeFormatString = "HH:mm";
                    }
                    else
                    {
                        dateTimePicker.FormatString = "MMM-dd-yyyy";
                    }
                    gridFields.Children.Add(dateTimePicker);
                    Grid.SetRow(dateTimePicker, gridFields.RowDefinitions.Count - 1);
                    Grid.SetColumn(dateTimePicker, 1);
                    dateTimePicker.Name = $"dtp{labelCaption.Replace(' ', '_')}";
                    ctl = dateTimePicker;
                    break;

                // datepicker is not used, instead a datetime picker is
                case "datePicker":
                    DatePicker datePicker = new DatePicker();
                    gridFields.Children.Add(datePicker);
                    Grid.SetRow(datePicker, gridFields.RowDefinitions.Count - 1);
                    Grid.SetColumn(datePicker, 1);
                    datePicker.Name = $"dp{labelCaption.Replace(' ', '_')}";
                    ctl = datePicker;
                    break;
                case "textBox":
                    TextBox txt = new TextBox();

                    txt.IsReadOnly = controlReadOnly;
                    gridFields.Children.Add(txt);
                    Grid.SetRow(txt, gridFields.RowDefinitions.Count - 1);
                    Grid.SetColumn(txt, 1);
                    txt.Name = $"txt{labelCaption.Replace(' ', '_')}";
                    ctl = txt;
                    break;
                case "comboBox":
                    ComboBox cbo = new ComboBox();
                    gridFields.Children.Add(cbo);
                    Grid.SetRow(cbo, gridFields.RowDefinitions.Count - 1);
                    Grid.SetColumn(cbo, 1);
                    cbo.Name = $"cbo{labelCaption.Replace(' ', '_')}";
                    //cbo.LostFocus += OnComboLostFocus;
                    cbo.DropDownClosed += OnComboDropdownClosed;
                    cbo.SelectionChanged += OnComboSelectionChanged;
                    ctl = cbo;
                    switch (cbo.Name)
                    {
                        //builds the content of a combobox showing GPS units in the database
                        //TODO: the list should not contain those that have been assigned to fishers
                        case "cboFisher":
                            cbo.DisplayMemberPath = "Value";
                            break;
                        case "cboProject":
                            cbo.DisplayMemberPath = "Value";
                            foreach (ProjectSetting prj in BSCEntities.ProjectSettingViewModel.GetAllProjectSettings())
                            {
                                cbo.Items.Add(new KeyValuePair<string, string>(prj.ProjectID, prj.ToString()));
                            }
                            break;
                        case "cboGear_name":
                            cbo.DisplayMemberPath = "Value";
                            foreach (Gear g in BSCEntities.GearViewModel.GearCollection)
                            {
                                cbo.Items.Add(new KeyValuePair<string, string>(g.GearID, g.GearName));
                            }
                            break;
                        case "cboGPS":
                            cbo.DisplayMemberPath = "Value";
                            switch (EntityType)
                            {
                                case "FisherGPS":
                                    var tempGPSList = Entities.BSCEntities.GPSViewModel.AvailableGPS();
                                    if (tempGPSList.Count > 0)
                                    {
                                        foreach (GPS gps in tempGPSList)
                                        {
                                            cbo.Items.Add(new KeyValuePair<string, string>(gps.ID, gps.ToString()));
                                        }
                                    }
                                    else
                                    {
                                        if (IsNew)
                                        {
                                            txtMessages.Text = $"No GPS units available to be assigned to {Fisher.FisherName}";
                                        }
                                    }
                                    if (!IsNew)
                                    {
                                        if (EntityType == "FisherGPS")
                                        {
                                            GPS assignedGPS = _fisherGPS.GPS;
                                            cbo.Items.Add(new KeyValuePair<string, string>(assignedGPS.ID, assignedGPS.ToString()));
                                        }
                                    }
                                    break;
                                case "Sampling":

                                    break;
                            }

                            break;

                        //builds a combobox with valid SD card sizes
                        case "cboSD_card_size":
                            cbo.Items.Add("");
                            cbo.Items.Add("4GB");
                            cbo.Items.Add("8GB");
                            cbo.Items.Add("16GB");
                            cbo.Items.Add("32GB");
                            cbo.Items.Add("64GB");
                            cbo.Items.Add("128GB");
                            cbo.Items.Add("256GB");
                            cbo.Items.Add("512GB");
                            cbo.Items.Add("1TB");
                            cbo.Items.Add("2TB");
                            break;

                        //builds a combobox of landing sites
                        case "cboLanding_site":
                            cbo.DisplayMemberPath = "Value";
                            foreach (LandingSite ls in Entities.BSCEntities.LandingSiteViewModel.GetAllLandingSites())
                            {
                                cbo.Items.Add(new KeyValuePair<string, string>(ls.LandingSiteID, ls.ToString()));
                            }
                            break;

                        case "cboProvince":
                            cbo.DisplayMemberPath = "Value";
                            foreach (Province p in BSCEntities.ProvinceViewModel.GetAllProvinces())
                            {
                                cbo.Items.Add(new KeyValuePair<int, string>(p.ProvinceID, p.ProvinceName));
                            }
                            break;

                        case "cboMunicipality":
                            cbo.DisplayMemberPath = "Value";
                            break;

                    }
                    break;
            }

            //adds a button on the 3rd column of the Grid
            if (buttonLabel.Length > 0)
            {
                Button btn = new Button();
                gridFields.Children.Add(btn);
                Grid.SetRow(btn, gridFields.RowDefinitions.Count - 1);
                Grid.SetColumn(btn, 2);
                btn.Content = buttonLabel;
                btn.Name = $"btn{buttonLabel.Replace(' ', '_')}";
                btn.Click += ButtonField_Click;
                btn.Margin = new Thickness(10, 2, 5, 2);
            }
            return ctl;
        }

        //private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    switch(((ComboBox)sender).Name)
        //    {
        //        case "cboFisher":
        //            break;
        //    }
        //}

        private void OnComboDropdownClosed(object sender, EventArgs e)
        {
            switch (((ComboBox)sender).Name)
            {
                case "cboFisher":
                    break;
                case "cboGPS":
                    if (EntityType == "Sampling" && _gpsCombo.SelectedItem != null)
                    {
                        var gps = BSCEntities.GPSViewModel.GetGPS(((KeyValuePair<string, string>)_gpsCombo.SelectedItem).Key);
                        if (_assignedGPSToFisher[gps].NumberOfTimesAssigned > 1)
                        {
                            SelectFromMultipleGPSWindow sfmgw = new SelectFromMultipleGPSWindow(_assignedGPSToFisher[gps]);
                            sfmgw.ShowDialog();
                            if (!sfmgw.Cancelled)
                            {
                                _fisherGPS = sfmgw.FisherGPS;
                            }
                            else
                            {
                                _gpsCombo.SelectedItem = null;
                                _fisherGPS = null;
                            }
                        }
                        else
                        {
                            GPS = BSCEntities.GPSViewModel.GetGPS(((KeyValuePair<string, string>)_gpsCombo.SelectedItem).Key);
                            _fisherGPS = BSCEntities.FisherGPSViewModel.GetFisherGPS(Fisher, GPS, ProjectSetting);
                        }

                        Sampling.FisherGPS = _fisherGPS;
                    }
                    break;
            }
        }

        private void DateTimePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string textInput = ((DateTimePicker)sender).Text;
            if (!DateTime.TryParse(textInput, out DateTime v))
            {
                DateTimePicker_InputValidationError(sender, new Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs(new Exception("Not a valid date")));
            }
            else
            {
                txtMessages.Text = "";
            }


        }

        private void DateTimePicker_InputValidationError(object sender, Xceed.Wpf.Toolkit.Core.Input.InputValidationErrorEventArgs e)
        {

            txtMessages.Text = e.Exception.Message;
            ((DateTimePicker)sender).Value = null;

        }

        private void OnComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            switch (cbo.Name)
            {
                case "cboGPS":
                    break;
                case "cboGear_name":
                    break;
                case "cboLanding_site":
                    if (_landingSiteCombo.SelectedItem != null && IsNew)
                    {
                        LandingSite = BSCEntities.LandingSiteViewModel.GetLandingSite(((KeyValuePair<string, string>)_landingSiteCombo.SelectedItem).Key);

                    }
                    break;
                case "cboFisher":
                    bool isGPSAssignedManyTimes = false;
                    
                    _fisherGPS = null;
                    GPS = null;

                    if (_fisherCombo.SelectedItem != null)
                    {
                        Fisher = BSCEntities.FisherViewModel.GetFisher(((KeyValuePair<string, string>)cbo.SelectedItem).Key);


                        _landingSiteCombo.Text = Fisher.LandingSite.ToString();


                        _gpsCombo.Items.Clear();


                        _assignedGPSToFisher = BSCEntities.GPSViewModel.GetAllGPSAssignedToFisherEx(Fisher, ProjectSetting);
                        foreach (KeyValuePair<GPS, AssignedGPSDetails> g in _assignedGPSToFisher)
                        {
                            _gpsCombo.Items.Add(new KeyValuePair<string, string>(g.Key.ID, g.Key.ToString()));
                            if (g.Value.NumberOfTimesAssigned > 1 && isGPSAssignedManyTimes == false)
                            {
                                isGPSAssignedManyTimes = true;
                            }
                        }


                        if (!isGPSAssignedManyTimes)
                        {
                            //get the currently assigned GPS or the last returned GPS as the default choice
                            _gpsCombo.Text = BSCEntities.GPSViewModel.GetSuggestedGPS(Fisher, ProjectSetting).ToString();
                            GPS = BSCEntities.GPSViewModel.GetGPS(((KeyValuePair<string, string>)_gpsCombo.SelectedItem).Key);
                            _fisherGPS = BSCEntities.FisherGPSViewModel.GetFisherGPS(Fisher, GPS, ProjectSetting);

                        }
                        else
                        {
                            if (Sampling.FisherGPS != null)
                                _fisherGPS = Sampling.FisherGPS;
                        }


                    }
                    Sampling.FisherGPS = _fisherGPS;
                    break;
                case "cboProject":

                    Fisher = null;
                    GPS = null;

                    if (_projectSettingCombo.SelectedItem != null)
                    {
                        ProjectSetting = BSCEntities.ProjectSettingViewModel.GetProjectSetting(((KeyValuePair<string, string>)cbo.SelectedItem).Key);

                        if (EntityType == "Sampling")
                        {
                            _fisherCombo.Items.Clear();
                            _gpsCombo.Items.Clear();



                            foreach (Fisher f in BSCEntities.FisherViewModel.GetAllFishersInProject(ProjectSetting))

                            {
                                _fisherCombo.Items.Add(new KeyValuePair<string, string>(f.FisherID, f.FisherName));
                            }
                        }
                    }
                    break;
                case "cboProvince":
                    if (cbo.SelectedItem == null)
                    {
                        Province = null;
                    }
                    else
                    {
                        Province = BSCEntities.ProvinceViewModel.GetProvince(((KeyValuePair<int, string>)cbo.SelectedItem).Key);

                        _municipalityCombo.Items.Clear();

                        foreach (Municipality m in BSCEntities.MunicipalityViewModel.GetAllMunicipalities(Province))
                        {
                            _municipalityCombo.Items.Add(new KeyValuePair<int, string>(m.MunicipalityID, m.MunicipalityName));
                        }

                    }
                    break;
            }
        }

        private void ButtonField_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnDetails_of_GPS":
                    if (_gpsCombo.SelectedItem != null)
                    {
                        if (EntityType == "Sampling")
                        {

                                System.Windows.MessageBox.Show(
                                    $"Assigned from {((DateTime)_fisherGPS.DateAssigned).ToString("MMM-dd-yyyy")} to {(_fisherGPS.DateReturned == null ? "present" : ((DateTime)_fisherGPS.DateReturned).ToString("MMM-dd-yyyy"))}",
                                    _fisherGPS.GPS.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information); 
                        }
                        else if(EntityType=="FisherGPS")
                        {
                            FisherGPS lastUser = null;
                            GPS gps = BSCEntities.GPSViewModel.GetGPS(((KeyValuePair<string, string>)_gpsCombo.SelectedItem).Key);
                            string gpsInfo = $"Date of acquisition is {(gps.DateAcquired==null? "not known":((DateTime)gps.DateAcquired).ToString("MMM-dd-yyyy"))}";
                            if (IsNew)
                            {
                                lastUser = BSCEntities.FisherGPSViewModel.LastGPSUser(gps);
                            }
                            else

                            {
                                lastUser = BSCEntities.FisherGPSViewModel.LastGPSUser (gps, _fisherGPS);
                            }
                            if(lastUser!=null)
                            {
                                gpsInfo += $"\r\nDate of previous assignemnt is from {((DateTime)lastUser.DateAssigned).ToString("MMM-dd-yyyy")} to {(lastUser.DateReturned==null?"present date":((DateTime)lastUser.DateReturned).ToString("MMM-dd-yyyy") )}";
                            }
                            else

                            {
                                gpsInfo += "\r\nGPS has no previous assignment";
                            }
                            System.Windows.MessageBox.Show(gpsInfo,gps.ToString(),MessageBoxButton.OK,MessageBoxImage.Information);
                            
                        }
                    }
                    break;
                case "btnAssigned_GPS":
                    if (_fisherCombo.SelectedItem != null)
                    {
                        FisherGPSWindow fgw = new FisherGPSWindow(Fisher, true);
                        fgw.ShowDialog();
                    }
                    break;
                case "btnDefine":
                    switch (EntityType)
                    {
                        case "LandingSite":
                            Waypoint editedWaypoint = new Waypoint(LandingSite.Waypoint.X,
                                LandingSite.Waypoint.Y,
                                LandingSite.Waypoint.Name,
                                LandingSite.Waypoint.TimeStamp);
                            AddEditWindow waypointEditWindow;
                            var ctl = gridFields.Children
                                .Cast<UIElement>()
                                .First(c => Grid.GetRow(c) == 1 && Grid.GetColumn(c) == 1);
                            if (((TextBox)ctl).Text.Length == 0)
                            {
                                waypointEditWindow = new AddEditWindow(isNew: true, wpt: null);
                            }
                            else
                            {
                                //waypointEditWindow = new AddEditWindow(isNew: false, wpt: LandingSite.Waypoint);
                                waypointEditWindow = new AddEditWindow(isNew: false, wpt: editedWaypoint);
                            }

                            waypointEditWindow.ShowDialog();
                            if (!waypointEditWindow.Cancelled)
                            {
                                Waypoint wpt = waypointEditWindow.Waypoint;
                                ((TextBox)ctl).Text = wpt.Name;
                                LandingSite.Waypoint = wpt;
                            }

                            break;

                    }
                    break;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnOk":
                    FisherGPS tempFisherGPS = new FisherGPS();
                    Dictionary<string, string> formValues = new Dictionary<string, string>();
                    int rows = gridFields.RowDefinitions.Count;


                    //we enumerate all the fields that are embedded in the grid container
                    for (int n = 0; n < rows; n++)
                    {
                        var ctl = gridFields.Children
                            .Cast<UIElement>()
                            .First(c => Grid.GetRow(c) == n && Grid.GetColumn(c) == 1);
                        string ctlName = ((Control)ctl).Name;
                        string ctlText = "";


                        //get the text value of each control
                        switch (ctl.GetType().Name)
                        {
                            case "DateTimePicker":
                                ctlText = ((DateTimePicker)ctl).Text;
                                break;
                            case "DatePicker":
                                ctlText = ((DatePicker)ctl).Text;
                                break;
                            case "TextBox":
                                ctlText = ((TextBox)ctl).Text;
                                break;
                            case "ComboBox":
                                ctlText = ((ComboBox)ctl).Text;
                                break;
                        }

                        //we fill up a dictionary that contains the control name and control's content as text
                        formValues.Add(ctlName, ctlText);


                        //then we edit each of the field of the entity
                        switch (EntityType)
                        {
                            case "Sampling":
                                switch (ctlName)
                                {
                                    case "cboProject":
                                        if (_projectSettingCombo != null)
                                        {
                                            Sampling.ProjectSetting = ProjectSetting;
                                            tempFisherGPS.ProjectSetting = ProjectSetting;
                                        }
                                        break;
                                    case "cboLanding_site":
                                        Sampling.LandingSite = LandingSite;
                                        break;

                                    case "txtNSAP_sample_identifier":
                                        Sampling.NSAPSamplingID = ctlText;
                                        break;
                                    case "cboGear_name":
                                        if (_gearCombo.SelectedItem != null)
                                        {
                                            Sampling.Gear = BSCEntities.GearViewModel.GetGear(((KeyValuePair<string, string>)_gearCombo.SelectedItem).Key);
                                        }
                                        break;
                                    case "cboFisher":
                                        if (_fisherCombo.SelectedItem != null)
                                            tempFisherGPS.Fisher = BSCEntities.FisherViewModel.GetFisher(((KeyValuePair<string, string>)_fisherCombo.SelectedItem).Key);
                                        break;
                                    case "cboGPS":
                                        if (_gpsCombo.SelectedItem != null)
                                            tempFisherGPS.GPS = BSCEntities.GPSViewModel.GetGPS(((KeyValuePair<string, string>)(_gpsCombo.SelectedItem)).Key);
                                        break;
                                    case "dtpDate_and_time_departed":
                                        if (!string.IsNullOrEmpty(ctlText))
                                            Sampling.DateTimeDeparted = DateTime.Parse(ctlText);
                                        break;
                                    case "dtpDate_and_time_arrived":
                                        if (!string.IsNullOrEmpty(ctlText))
                                            Sampling.DateTimeArrived = DateTime.Parse(ctlText);
                                        break;
                                    case "dtpDate_and_time_sampled":
                                        if (!string.IsNullOrEmpty(ctlText))
                                            Sampling.DateTimeSampled = DateTime.Parse(ctlText);
                                        break;
                                }
                                break;
                            case "Gear":
                                switch (ctlName)
                                {
                                    case "txtGear_name":
                                        Gear.GearName = ctlText;
                                        break;
                                    case "txtCode":
                                        Gear.Code = ctlText;
                                        break;
                                }
                                break;
                            case "ProjectSetting":
                                switch (ctlName)
                                {
                                    case "txtProject_name":
                                        ProjectSetting.ProjectName = ctlText;
                                        break;
                                    case "dtpDate_started":
                                        if(!string.IsNullOrEmpty(ctlText))
                                           ProjectSetting.DateStart = DateTime.Parse(ctlText);
                                        break;
                                }
                                break;
                            case "FisherGPS":
                                switch (ctlName)
                                {
                                    case "cboGPS":
                                        ComboBox cbo = (ComboBox)ctl;
                                        if (cbo.SelectedItem != null)
                                        {
                                            _fisherGPS.GPS = Entities.BSCEntities.GPSViewModel.GetGPS(((KeyValuePair<string, string>)cbo.SelectedItem).Key);
                                        }
                                        break;
                                    case "cboProject":
                                        cbo = (ComboBox)ctl;
                                        if (cbo.SelectedItem != null)
                                        {
                                            _fisherGPS.ProjectSetting = ProjectSetting;
                                        }
                                        break;
                                    case "dtpDate_assigned":
                                        if (!string.IsNullOrEmpty(ctlText))
                                            _fisherGPS.DateAssigned = DateTime.Parse(ctlText);

                                        break;
                                    case "dtpDate_returned":
                                        if (!string.IsNullOrEmpty(ctlText))
                                            _fisherGPS.DateReturned = DateTime.Parse(ctlText);

                                        break;
                                }
                                break;
                            case "Waypoint":
                                switch (ctlName)
                                {
                                    case "txtWaypoint_name":
                                        Waypoint.Name = ctlText;
                                        break;
                                    case "txtLongitude":

                                        if (Global.ParsedCoordinateIsValid(ctlText, "x", out double v))
                                        {
                                            Waypoint.X = v;
                                        }
                                        else
                                        {
                                            txtMessages.Text = "Longitude must be a number";
                                        }

                                        break;
                                    case "txtLatitude":
                                        if (Global.ParsedCoordinateIsValid(ctlText, "y", out v))
                                        {
                                            Waypoint.Y = v;

                                        }
                                        else
                                        {
                                            txtMessages.Text = "Latitude must be a number";
                                        }
                                        break;
                                    case "dtpDate_acquired":
                                        if (DateTime.TryParse(ctlText, out DateTime dt))
                                            Waypoint.TimeStamp = dt;
                                        break;
                                }
                                break;
                            case "Fisher":
                                switch (ctlName)
                                {
                                    case "txtFisher_name":
                                        Fisher.FisherName = ctlText;
                                        break;
                                    case "cboLanding_site":
                                        ComboBox cbo = (ComboBox)ctl;
                                        if (cbo.SelectedItem != null)
                                            Fisher.LandingSite = Entities.BSCEntities.LandingSiteViewModel.GetLandingSite(((KeyValuePair<string, string>)cbo.SelectedItem).Key);
                                        break;
                                    case "txtFishing_boat_name":
                                        Fisher.FishingBoatName = ctlText;
                                        break;
                                }
                                break;
                            case "GPS":

                                switch (ctlName)
                                {
                                    case "txtBrand":
                                        GPS.Brand = ctlText;
                                        break;
                                    case "txtModel":
                                        GPS.Model = ctlText;
                                        break;
                                    case "txtAssigned_name":
                                        GPS.AssignedName = ctlText;
                                        break;
                                    case "cboSD_card_size":
                                        GPS.SDCardCapacity = ctlText;
                                        break;
                                    case "dtpDate_acquired":
                                        if (!string.IsNullOrWhiteSpace(ctlText))
                                            GPS.DateAcquired = DateTime.Parse(ctlText);
                                        break;
                                }
                                break;
                            case "LandingSite":
                                switch (ctlName)
                                {
                                    case "txtLanding_site_name":
                                        LandingSite.LandingSiteName = ctlText;
                                        break;
                                    case "txtWaypoint":
                                        //not used
                                        break;
                                    case "cboMunicipality":
                                        ComboBox cbo = (ComboBox)ctl;
                                        if (cbo.SelectedItem != null)
                                            LandingSite.Municipality = BSCEntities.MunicipalityViewModel.GetMunicipality(((KeyValuePair<int, string>)cbo.SelectedItem).Key);
                                        break;
                                }
                                break;
                        }

                    }



                    if (ValidateEntities(formValues))
                    {
                        if (IsNew)
                        {
                            var newGUID = Guid.NewGuid().ToString();
                            switch (EntityType)
                            {
                                case "Sampling":
                                    Sampling.RowID = newGUID;
                                    BSCEntities.SamplingViewModel.AddRecordToRepo(Sampling);
                                    break;
                                case "Gear":
                                    Gear.GearID = newGUID;
                                    BSCEntities.GearViewModel.AddRecordToRepo(Gear);
                                    break;
                                case "ProjectSetting":
                                    ProjectSetting.ProjectID = newGUID;
                                    Entities.BSCEntities.ProjectSettingViewModel.AddRecordToRepo(ProjectSetting);
                                    break;
                                case "FisherGPS":
                                    _fisherGPS.RowID = newGUID;
                                    Entities.BSCEntities.FisherGPSViewModel.AddRecordToRepo(_fisherGPS);
                                    break;
                                case "GPS":
                                    GPS.ID = newGUID;
                                    Entities.BSCEntities.GPSViewModel.AddRecordToRepo(GPS);
                                    break;
                                case "Fisher":
                                    Fisher.FisherID = newGUID;
                                    Entities.BSCEntities.FisherViewModel.AddRecordToRepo(Fisher);
                                    break;
                                case "LandingSite":
                                    LandingSite.LandingSiteID = newGUID;
                                    Entities.BSCEntities.LandingSiteViewModel.AddRecordToRepo(LandingSite);
                                    break;
                            }
                        }
                        else
                        {
                            switch (EntityType)
                            {
                                case "Sampling":
                                    BSCEntities.SamplingViewModel.UpdateRecordInRepo(Sampling);
                                    break;
                                case "Gear":
                                    BSCEntities.GearViewModel.UpdateRecordInRepo(Gear);
                                    break;
                                case "ProjectSetting":
                                    Entities.BSCEntities.ProjectSettingViewModel.UpdateRecordInRepo(ProjectSetting);
                                    break;
                                case "FisherGPS":
                                    Entities.BSCEntities.FisherGPSViewModel.UpdateRecordInRepo(_fisherGPS);
                                    break;
                                case "GPS":
                                    Entities.BSCEntities.GPSViewModel.UpdateRecordInRepo(GPS);
                                    break;
                                case "LandingSite":
                                    Entities.BSCEntities.LandingSiteViewModel.UpdateRecordInRepo(LandingSite);
                                    break;
                                case "Fisher":
                                    Entities.BSCEntities.FisherViewModel.UpdateRecordInRepo(Fisher);
                                    break;
                            }

                        }
                        _saveButtonClicked = true;
                        Close();
                    }


                    break;
                case "btnCancel":
                    //Cancelled is true because _saveButtonClicked is still false;
                    if (EntityType == "LandingSite")
                    {
                        LandingSite.Waypoint = _oldLandingSite.Waypoint; ;
                    }
                    Close();

                    break;
            }
        }

        ///This method is save the actual position of the window to file "WindowName.pos"
        private void ClosingTrigger(object sender, EventArgs e)
        {
            Cancelled = !_saveButtonClicked;
            this.SavePlacement();
        }
        ///This method is load the actual position of the window from the file
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.ApplyPlacement();
        }

        /// <summary>
        /// Validate each entity using the validation methods in each entity class
        /// Displays any error message if any
        /// </summary>
        /// <returns></returns>
        private bool ValidateEntities(Dictionary<string, string> formValues)
        {
            int errorCount = 0;
            int warningCount = 0;
            bool success = false;
            List<string> messages = new List<string>();
            List<EntityValidationMessage> entityMessages = new List<EntityValidationMessage>();
            switch (EntityType)
            {
                case "Sampling":
                    if (BSCEntities.SamplingViewModel.EntityValidated(Sampling, out entityMessages))
                        success = true;
                    break;
                case "Gear":
                    if (BSCEntities.GearViewModel.EntityValidated(Gear, out entityMessages, IsNew, _oldName, _oldCode))
                        success = true;
                    break;
                case "ProjectSetting":
                    if (Entities.BSCEntities.ProjectSettingViewModel.EntityValidated(ProjectSetting, out messages))
                        success = true;
                    break;
                case "FisherGPS":
                    if (Entities.BSCEntities.FisherGPSViewModel.EntityValidated(_fisherGPS, out entityMessages, IsNew))
                        success = true;
                    break;
                case "Waypoint":
                    if (formValues.Count > 0 && Waypoint.EntityValidated(formValues, out entityMessages))
                        success = true;

                    break;
                case "LandingSite":
                    if (Entities.BSCEntities.LandingSiteViewModel.EntityValidated(LandingSite, out messages))
                        success = true;

                    break;
                case "Fisher":
                    if (Entities.BSCEntities.FisherViewModel.EntityValidated(Fisher, out messages))
                        success = true;

                    break;
                case "GPS":
                    if (Entities.BSCEntities.GPSViewModel.EntityValidated(GPS, out messages, IsNew, _oldName))
                    {
                        success = true;
                    }

                    break;
            }

            if (!success)
            {
                string allMessages = "";
                if (messages.Count > 0)
                {

                    foreach (string msg in messages)
                    {
                        allMessages += $"{msg}\n\r";
                    }


                }
                else if (entityMessages.Count > 0)
                {
                    foreach (EntityValidationMessage em in entityMessages)
                    {
                        switch (em.MessageType)
                        {
                            case MessageType.Error:
                                allMessages += $"ERROR: {em.Message}\n\r";
                                errorCount++;
                                break;
                            case MessageType.Information:
                                allMessages += $"INFORMATION: {em.Message}\n\r";
                                break;
                            case MessageType.Warning:
                                allMessages += $"WARNING: {em.Message}\n\r";
                                warningCount++;
                                break;
                        }
                    }
                }
                txtMessages.Text = allMessages;
            }
            if (errorCount == 0 && warningCount > 0)
            {
                MessageBoxResult msb = System.Windows.MessageBox.Show("Do you wish to proceed and save?", "There are warnings", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                return msb == MessageBoxResult.Yes;
            }
            else
            {
                return success && errorCount == 0;
            }
        }
    }
}
