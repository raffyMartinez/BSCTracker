using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCTracker.Entities;
namespace BSCTracker.TreeViewModelControl
    
{
    public class tv_MonthFishingViewModel:TreeViewItemViewModel
    {
        public readonly String _month;
        public readonly Gear _gear;
        public readonly LandingSite _landingSite;
        public readonly ProjectSetting _projectSetting;
        public tv_MonthFishingViewModel(string month, tv_GearViewModel gear) : base(gear, true)
        {
            _month=month;
            _gear = gear._gear;
            _landingSite = gear._landingSite;
            _projectSetting = gear._projectSetting;
        }

        public string MonthName
        {
            get { return DateTime.Parse(_month).ToString("MMM-yyyy"); }
        }


    }
}
