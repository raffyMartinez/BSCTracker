using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCTracker.Entities;

namespace BSCTracker.TreeViewModelControl
{
    public class tv_LandingSiteViewModel:TreeViewItemViewModel
    
    {
        public readonly LandingSite _landingSite;
        public readonly ProjectSetting _projectSetting;
       
        public tv_LandingSiteViewModel(LandingSite landingSite, tv_ProjectSettingViewModel parent):base(parent,true)
        {
            _landingSite = landingSite;
            _projectSetting = parent._projectSetting;
        }

        public string LandingSiteName
        {
            get { return _landingSite.ToString(); }
        }

        public void Add(Gear gear)
        {
            base.Children.Add(new tv_GearViewModel(gear, this));
        }
        public void Refresh()
        {
            Children.Clear();
            LoadChildren();
        }

        protected override void LoadChildren()
        {

            List<Gear> listGear = new List<Gear>();
            foreach(var s in BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t=>t.ProjectSetting.ProjectID==_projectSetting.ProjectID)
                .Where(t=>t.LandingSite.LandingSiteID==_landingSite.LandingSiteID))
            {
                if(!listGear.Contains(s.Gear))
                {
                    listGear.Add(s.Gear);
                }
            }

            foreach(var g in listGear)
            {
                base.Children.Add(new tv_GearViewModel(g, this));
            }

        }
    }
}
