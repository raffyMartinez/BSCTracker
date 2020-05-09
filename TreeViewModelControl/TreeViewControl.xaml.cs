using System.Windows.Controls;
using BSCTracker.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System;
using BSCTracker.Utilities;

namespace BSCTracker.TreeViewModelControl
{
    /// <summary>
    /// Interaction logic for TreeViewControl.xaml
    /// </summary>
    public partial class TreeViewControl : UserControl
    {
        public Window ParentWindow { get; internal set; }

        public event EventHandler<AllSamplingEntitiesEventHandler> TreeViewItemSelected;

        public TreeViewItemViewModel _selectedItem;

        private tv_BSCViewModel _bscViewModel;

        public TreeViewControl()
        {
            InitializeComponent();
        }



        private void TreeSelectedItem_Changed(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tv = (TreeView)sender;
            var tvi = tv.SelectedItem;

            if (tvi == null)
                return;

            _selectedItem = (TreeViewItemViewModel)tvi;
            AllSamplingEntitiesEventHandler args = new AllSamplingEntitiesEventHandler();
            string treeViewEntity = tvi.GetType().Name;
            args.TreeViewEntity = treeViewEntity;
            switch (treeViewEntity)
            {
                case "tv_ProjectSettingViewModel":
                    args.ProjectSetting = ((tv_ProjectSettingViewModel)tvi)._projectSetting;
                    break;
                case "tv_LandingSiteViewModel":
                    args.LandingSite = ((tv_LandingSiteViewModel)tvi)._landingSite;
                    args.ProjectSetting = ((tv_LandingSiteViewModel)tvi)._projectSetting;
                    break;
                case "tv_GearViewModel":
                    args.Gear = ((tv_GearViewModel)tvi)._gear;
                    args.LandingSite = ((tv_GearViewModel)tvi)._landingSite;
                    args.ProjectSetting = ((tv_GearViewModel)tvi)._projectSetting;
                    break;
                case "tv_MonthFishingViewModel":
                    args.MonthSampled = DateTime.Parse(((tv_MonthFishingViewModel)tvi)._month);
                    args.Gear = ((tv_MonthFishingViewModel)tvi)._gear;
                    args.LandingSite = ((tv_MonthFishingViewModel)tvi)._landingSite;
                    args.ProjectSetting = ((tv_MonthFishingViewModel)tvi)._projectSetting;
                    break;

            }
            TreeViewItemSelected?.Invoke(this, args);
        }

        public void Refresh(EditedEntity editedEnity)
        {
            bool breakAll = false;
            Boolean itemFound = false;
            switch (editedEnity.BSCEntity.GetType().Name)
            {
                case "ProjectSetting":


                    ProjectSetting editedProjectSetting = (ProjectSetting)editedEnity.BSCEntity;
                    foreach (TreeViewItemViewModel tvi in treeView.Items)
                    {
                        tv_ProjectSettingViewModel psvm = (tv_ProjectSettingViewModel)tvi;
                        if (psvm._projectSetting.ProjectID == editedProjectSetting.ProjectID)
                        {
                            itemFound = true;
                            if (editedEnity.EditAction == EditAction.Delete)
                            {

                            }
                            else if (editedEnity.EditAction == EditAction.Update)
                            {
                                psvm.Edit(editedProjectSetting);
                            }
                            break;
                        }
                    }

                    if (!itemFound && editedEnity.EditAction == EditAction.Add)
                    {
                        _bscViewModel.Add(editedProjectSetting);
                    }
                    treeView.Items.Refresh();
                    break;

                case "Sampling":
                    Sampling sampling = (Sampling)editedEnity.BSCEntity;
                    if (editedEnity.EditAction == EditAction.Add)
                    {
                        foreach (TreeViewItemViewModel projectItem in treeView.Items)
                        {
                            if (projectItem.IsExpanded)
                            {
                                foreach (TreeViewItemViewModel landingSiteItem in projectItem.Children)
                                {
                                    if (sampling.LandingSite.LandingSiteID == ((tv_LandingSiteViewModel)landingSiteItem)._landingSite.LandingSiteID)
                                    {
                                        itemFound = true;
                                        if (landingSiteItem.IsExpanded)
                                        {
                                            itemFound = false;
                                            foreach (TreeViewItemViewModel gearItem in landingSiteItem.Children)
                                            {
                                                if (((tv_GearViewModel)gearItem)._gear.GearName == sampling.Gear.GearName)
                                                {
                                                    itemFound = true;
                                                    if (gearItem.IsExpanded)
                                                    {
                                                        itemFound = false;
                                                        foreach (TreeViewItemViewModel monthItem in gearItem.Children)
                                                        {
                                                            tv_MonthFishingViewModel month = (tv_MonthFishingViewModel)monthItem;
                                                            if (month.MonthName == sampling.DateTimeSampled.ToString("MMM-yyyy"))
                                                            {
                                                                itemFound = true;
                                                                breakAll = true;
                                                                break;
                                                            }

                                                        }

                                                        //month not found in gear
                                                        if (!itemFound && editedEnity.EditAction == EditAction.Add)
                                                        {
                                                            ((tv_GearViewModel)gearItem).Refresh();
                                                            itemFound = true;
                                                            breakAll = true;
                                                            break;
                                                        }
                                                    }

                                                }
                                            }

                                            //gear was not found under landing site
                                            if (!itemFound && editedEnity.EditAction == EditAction.Add)
                                            {
                                                ((tv_LandingSiteViewModel)landingSiteItem).Add(sampling.Gear);
                                                breakAll = true;
                                                itemFound = true;
                                                break;
                                            }
                                        }
                                    }

                                }
                                //landing site was not found under project in the tree
                                if (!itemFound)
                                {
                                    ((tv_ProjectSettingViewModel)projectItem).Add(sampling.LandingSite);
                                    itemFound = true;
                                    breakAll = true;
                                    break;
                                }
                            }
                            if (breakAll)
                            {
                                break;
                            }


                        }
                    }
                    break;
            }
        }



        private void Tree_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Global.AppProceed)
            {
                ParentWindow = Window.GetWindow(treeControl);
                List<ProjectSetting> projList = BSCEntities.ProjectSettingViewModel.GetAllProjectSettings().ToList();
                _bscViewModel = new tv_BSCViewModel(projList, this);
                base.DataContext = _bscViewModel;
            }
        }
    }
}