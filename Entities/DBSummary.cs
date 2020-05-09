using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public static class DBSummary
    {
        public static int NumberOfProjects { get; set; }
        public  static int NumberOfFishers { get; set; }
        public  static int NumberOfLandingSites { get; set; }
        public  static int NumberOfGPS { get; set; }
        public  static int NumberOfSampling { get; set; }
        public static  int NumberOfGearTypesSampled { get; set; }

        public static int NumberOfUniqueGears { get; set; }

        public  static  Dictionary<string, string> SummaryValues = new Dictionary<string, string>();

        public static void UpdateSummary()
        {
            SummaryValues.Clear();

            NumberOfFishers = BSCEntities.FisherViewModel.Count;
            NumberOfProjects = BSCEntities.ProjectSettingViewModel.Count;
            NumberOfLandingSites = BSCEntities.LandingSiteViewModel.Count;
            NumberOfSampling = BSCEntities.SamplingViewModel.Count;
            NumberOfGearTypesSampled = BSCEntities.GPSViewModel.Count;
            NumberOfGPS = BSCEntities.GPSViewModel.Count;
            NumberOfUniqueGears = BSCEntities.GearViewModel.Count;

            SummaryValues.Add("Number of projects", NumberOfProjects.ToString());
            SummaryValues.Add("Number of fishers", NumberOfFishers.ToString());
            SummaryValues.Add("Number of landing sites", NumberOfLandingSites.ToString());
            SummaryValues.Add("Number of GPS", NumberOfGPS.ToString());
            SummaryValues.Add("Number of unique gears", NumberOfUniqueGears.ToString());
            SummaryValues.Add("Number of samplings", NumberOfSampling.ToString());

        }
        static DBSummary()
        {

        }
        
    }
}
