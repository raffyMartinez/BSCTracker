using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCTracker.Entities;

namespace BSCTracker.TreeViewModelControl
{
   public class AllSamplingEntitiesEventHandler:EventArgs
    {
        public Sampling Sampling { get; set; }
        public Fisher Fisher { get; set; }
        public GPS GPS { get; set; }
        public FisherGPS FisherGPS { get; set; }

        public LandingSite LandingSite { get; set; }

        public Gear Gear { get; set; }
        public string GearUsed { get; set; }

        public DateTime? MonthSampled { get; set; }

        public ProjectSetting ProjectSetting { get; set; }

        public string TreeViewEntity { get; set; }
    }
}
