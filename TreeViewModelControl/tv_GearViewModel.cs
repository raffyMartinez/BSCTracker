using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCTracker.Entities;

namespace BSCTracker.TreeViewModelControl
{
    public class tv_GearViewModel:TreeViewItemViewModel
    {
        public readonly Gear _gear;
        public readonly LandingSite _landingSite;
        public readonly ProjectSetting _projectSetting;

        public tv_GearViewModel(Gear gear, tv_LandingSiteViewModel ls) : base(ls, true)
        {
            _gear = gear;
            _landingSite = ls._landingSite;
            _projectSetting = ls._projectSetting;
        }

        public void Refresh()
        {
            Children.Clear();
            LoadChildren();
        }
        public string GearName
        {
            get { return _gear.GearName; }
        }

        public void Add(tv_MonthFishingViewModel month)
        {
            base.Children.Add(month);
        }
        protected override void LoadChildren()
        {

            List<string> listMonthYear = new List<string>();
            foreach(var s in BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t=>t.ProjectSetting.ProjectID==_projectSetting.ProjectID)
                .Where(t=>t.LandingSite.LandingSiteID==_landingSite.LandingSiteID)
                .Where(t=>t.Gear.GearID==_gear.GearID)
                .OrderBy(t=>t.DateTimeSampled))
            {
                string monthYear = s.DateTimeSampled.ToString("MMM-yyyy");
                if(!listMonthYear.Contains(monthYear))
                {
                    listMonthYear.Add(monthYear);
                }
            }


            foreach (string my in listMonthYear)
            {
                base.Children.Add(new tv_MonthFishingViewModel(my, this));
            }
        }
    }
}
