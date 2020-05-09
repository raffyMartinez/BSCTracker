using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;

namespace BSCTracker.Entities
{
    public class GPSViewModel
    {
        public ObservableCollection<GPS> GPSCollection { get; set; }
        private GPSRepository GPSes { get; set; }



        public GPSViewModel()
        {

            GPSes = new GPSRepository();
            GPSCollection = new ObservableCollection<GPS>(GPSes.GPSes);
            GPSCollection.CollectionChanged += GPSes_CollectionChanged;

        }

        public Dictionary<GPS, int> GetAllGPSAssignedToFisherEx1(Fisher f, ProjectSetting p)
        {
            var tempList = from g in BSCEntities.FisherGPSViewModel.FisherGPSCollection
                           where g.Fisher.FisherID == f.FisherID
                           where g.ProjectSetting.ProjectID == p.ProjectID
                           group g.GPS by g.GPS into t
                           select new
                           {
                               key = t.Key,
                               value = t.Count()
                           };
            return tempList.ToDictionary(x => x.key, x => x.value);
        }

        public Dictionary<GPS, AssignedGPSDetails> GetAllGPSAssignedToFisherEx(Fisher f, ProjectSetting p)
        {
            var dict = new Dictionary<GPS, AssignedGPSDetails>();
            foreach (var fg in BSCEntities.FisherGPSViewModel.FisherGPSCollection
                .Where(t => t.Fisher.FisherID == f.FisherID)
                .Where(t => t.ProjectSetting.ProjectID == p.ProjectID)
                .OrderBy(t => t.GPS.AssignedName)
                .ThenByDescending(t=>t.DateAssigned))
                
            {
                var agpsd = new AssignedGPSDetails();

                if (dict.Keys.Contains(fg.GPS))
                {
                    dict[fg.GPS].NumberOfTimesAssigned++;
                    //dict[fg.GPS].AddDateToRange((DateTime)fg.DateAssigned, fg.DateReturned);
                    dict[fg.GPS].AddFisherGPS(fg);
                }
                else
                {
                    //agdr = new AssignedGPSDateRange(fg.GPS, (DateTime)fg.DateAssigned, fg.DateReturned, 1);
                    agpsd = new AssignedGPSDetails(fg.GPS, fg, 1);
                    dict.Add(fg.GPS, agpsd);
                }
            }

            return dict;
        }

        public GPS GetSuggestedGPS(Fisher f, ProjectSetting p)
        {
            return BSCEntities.FisherGPSViewModel.FisherGPSCollection
                .Where(t => t.Fisher.FisherID == f.FisherID)
                .Where(t => t.ProjectSetting.ProjectID == p.ProjectID)
                .OrderByDescending(t => t.DateAssigned)
                .Select(t => t.GPS).FirstOrDefault();
        }


        public List<GPS> GetAllGPSAssignedToFisher(Fisher f, ProjectSetting p)
        {
            return BSCEntities.FisherGPSViewModel.FisherGPSCollection
                 .Where(x => x.ProjectSetting.ProjectID == p.ProjectID)
                 .Where(x => x.Fisher.FisherID == f.FisherID)
                 .GroupBy(t => t.GPS.ID)
                 .Select(t => t.First().GPS).ToList();

        }

        public bool CanDeleteEntity(GPS gps)
        {
            //return !GPSes.GPSIsPairedWithFisher(gps);
            return BSCEntities.FisherGPSViewModel.FisherGPSCollection
                .Where(t => t.GPS.ID == gps.ID).ToList().Count == 0;
        }
        public int Count
        {
            get { return GPSCollection.Count; }
        }

        public ObservableCollection<GPS> AvailableGPS()
        {

            List<GPS> availableGPS = GPSCollection.ToList();
            //foreach (var gps in GPSes.GetAssignedUnits())
            foreach (var fg in BSCEntities.FisherGPSViewModel.FisherGPSCollection
                .Where(t => t.DateReturned == null))

            {
                GPS tempGPS = fg.GPS;
                if (availableGPS.Contains(tempGPS))
                {
                    availableGPS.Remove(tempGPS);
                }
            }
            return new ObservableCollection<GPS>(availableGPS);
        }

        public bool AssignedNameExist(string assignedName)
        {
            foreach (GPS gps in GPSCollection)
            {
                if (gps.AssignedName == assignedName)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddRecordToRepo(GPS gps)
        {
            if (gps == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GPSCollection.Add(gps);
        }

        public void UpdateRecordInRepo(GPS gps)
        {
            if (gps.ID == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < GPSCollection.Count)
            {
                if (GPSCollection[index].ID == gps.ID)
                {
                    GPSCollection[index] = gps;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(string id)
        {
            if (id == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < GPSCollection.Count)
            {
                if (GPSCollection[index].ID == id)
                {
                    GPSCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }
        public List<GPS> GetAllGPS()
        {
            return GPSCollection.ToList();
        }

        public GPS GetGPS(string ID)
        {
            return GPSCollection.FirstOrDefault(n => n.ID == ID);

        }
        private void GPSes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        GPSes.Add(GPSCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<GPS> tempListOfRemovedItems = e.OldItems.OfType<GPS>().ToList();
                        GPSes.Delete(tempListOfRemovedItems[0].ID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<GPS> tempListOfGPS = e.NewItems.OfType<GPS>().ToList();
                        GPSes.Update(tempListOfGPS[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public bool EntityValidated(GPS gps, out List<string> messages, bool isNew = false, string oldName = "")
        {

            messages = new List<string>();

            if (gps.Brand.Length == 0)
                messages.Add("GPS brand name cannot be empty");

            if (gps.Model.Length == 0)
                messages.Add("GPS model name cannot be empty");

            if (gps.AssignedName.Length == 0)
                messages.Add("GPS assigned name cannot be empty");

            if (isNew && gps.AssignedName.Length > 0 && AssignedNameExist(gps.AssignedName))
                messages.Add("GPS assigned name already used");

            if (!isNew && gps.AssignedName.Length > 0
                && oldName != gps.AssignedName
                && AssignedNameExist(gps.AssignedName))
                messages.Add("GPS assigned name already used");

            return messages.Count == 0;
        }
    }
}
