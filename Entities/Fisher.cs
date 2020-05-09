using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class Fisher
    {
        public Fisher() { }
        public Fisher(string id, string fisherName, LandingSite landingSite)
        {
            FisherID = id;
            FisherName = fisherName;
            LandingSite = landingSite;
        }
        public string FisherID { get; set; }
        public string FisherName { get; set; }
        public LandingSite LandingSite { get; set; }

        public string FishingBoatName { get; set; }


    }
}
