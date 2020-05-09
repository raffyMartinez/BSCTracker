using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class LandingSite
    {
        public LandingSite() { }
        
        public LandingSite(string id, string landingSiteName, Waypoint waypoint,  Municipality municipality)
        {
            LandingSiteID = id;
            LandingSiteName = landingSiteName;
            Waypoint = waypoint;
            Municipality = municipality;
        }

        public string LandingSiteID{get; set;}
        public string LandingSiteName { get; set; }

        public Waypoint Waypoint { get; set; }


        public override string ToString()
        {
            if (Municipality != null)
            {
                return $"{LandingSiteName}, {Municipality.ToString()}";
            }
            else
            {
                return $"{LandingSiteName}";
            }
        }

        public Municipality Municipality { get; set; }
    }
}
