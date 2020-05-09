using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class Sampling
    {
        
        public FisherGPS FisherGPS { get; set; }
        public string RowID { get; set; }
        public DateTime? DateTimeDeparted { get; set; }
        public DateTime? DateTimeArrived { get; set; }
        public string NSAPSamplingID { get; set; }
        public Gear Gear{ get; set; }
        public ProjectSetting ProjectSetting { get; set; }

        public DateTime DateTimeSampled { get; set; }

        public LandingSite LandingSite { get; set; }
        public DateTime DateAdded { get; set; }
        public Sampling() { }
        public Sampling(FisherGPS fisherGPS, LandingSite landingSite, string rowID, Gear gear, DateTime? datetimeDeparted,
            DateTime? dateTimeArrived, string nsapSamplingID, ProjectSetting projectSetting, DateTime dateTimeSampled, DateTime dateAdded)
        {
            FisherGPS = fisherGPS;
            RowID = rowID;
            DateTimeDeparted = datetimeDeparted;
            DateTimeArrived = dateTimeArrived;
            NSAPSamplingID = nsapSamplingID;
            ProjectSetting = projectSetting;
            DateTimeSampled = dateTimeSampled;
            LandingSite = landingSite;
            Gear = gear;
            DateAdded = dateAdded;
        }
    }
}
