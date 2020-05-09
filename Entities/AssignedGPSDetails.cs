using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class AssignedGPSDetails
    {
        public List<FisherGPS> FisherGPSList { get; internal set; }
        public GPS GPS { get; internal set; }
   

        public void AddFisherGPS(FisherGPS fisherGPS)
        {
            FisherGPSList.Add(fisherGPS);
        }
        public AssignedGPSDetails(GPS gps, FisherGPS fisherGPS, int numberOfTimesAssigned)
        {
            if(FisherGPSList==null)
            {
                FisherGPSList = new List<FisherGPS>();
            }
            GPS = gps;
            FisherGPSList.Add(fisherGPS);
            NumberOfTimesAssigned = numberOfTimesAssigned;
        }
        public int NumberOfTimesAssigned { get; set; }
        public AssignedGPSDetails() 
        {
           // DateRangeList = new List<DateRange>();
        }
    }
}
