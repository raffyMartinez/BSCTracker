using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class FisherGPS
    {
        public FisherGPS()
        {
        }

        public FisherGPS(string id, GPS gps, Fisher fisher, ProjectSetting projectSetting, DateTime? dateAssigned, DateTime? dateReturned)
        {
            RowID = id;
            GPS = gps;
            Fisher = fisher;
            ProjectSetting = projectSetting;
            DateAssigned = dateAssigned;
            DateReturned = dateReturned;
        }
        public string RowID { get; set; }
        public GPS GPS { get; set; }
        public Fisher Fisher { get; set; }

        public DateTime? DateAssigned { get; set; }
        public DateTime? DateReturned { get; set; }
        public ProjectSetting ProjectSetting {get;set;}

        public override string ToString()
        {
            return $"{Fisher.FisherName} - {GPS.ToString()}";
        }


    }
}
