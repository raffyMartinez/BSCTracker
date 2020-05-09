using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSCTracker.Entities
{
    public class FisherViewModel
    {
        public ObservableCollection<Fisher> FisherCollection { get; set; }
        private FisherRepository Fishers { get; set; }

        public FisherViewModel()
        {
            Fishers = new FisherRepository();
            FisherCollection = new ObservableCollection<Fisher>(Fishers.Fishers);
            FisherCollection.CollectionChanged += Fishers_CollectionChanged;
        }

        public List<Fisher>GetAllFishersInProject(ProjectSetting p)
        {
            return BSCEntities.FisherGPSViewModel.FisherGPSCollection
                                .Where(t => t.ProjectSetting.ProjectID == p.ProjectID)
                                .GroupBy(t => t.Fisher.FisherID)
                                .Select(t => t.First().Fisher).ToList();
        }

        public void RefreshFisherLandingSite(LandingSite ls)
        {
            foreach(var fisher in FisherCollection
                .Where(t=>t.LandingSite.LandingSiteID==ls.LandingSiteID))
                
            {
                fisher.LandingSite = ls;
            }
        }

        public List<FisherAndGPS> GetFishersAndGPS()
        {
           var fisherGPS = from f in FisherCollection
                              join gps in BSCEntities.FisherGPSViewModel.FisherGPSCollection
                                    .Where(t=>t.DateReturned==null)
                              on f equals gps.Fisher into temp
                              from subset in temp.DefaultIfEmpty()
                              select new FisherAndGPS(){Fisher = f, GPS = subset==null ? null:subset.GPS, ProjectSetting= subset==null ? null:subset.ProjectSetting};
            return fisherGPS.ToList();
        }
        public List<Fisher> GetAllFishers()
        {
            return FisherCollection.ToList();
        }
        public bool CanDeleteEntity(Fisher f)
        {
            //return !Fishers.FisherIsPairedWithGPS(f);
            return BSCEntities.FisherGPSViewModel.FisherGPSCollection
                .Where(t => t.Fisher.FisherID == f.FisherID).ToList().Count == 0;
        }
        public Fisher GetFisherByBoatName(string boatName)
        {
            return FisherCollection.FirstOrDefault(n => n.FishingBoatName == boatName);

        }
        public Fisher GetFisher(string fisherID)
        {
            return FisherCollection.FirstOrDefault(n => n.FisherID == fisherID);

        }
        private void Fishers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Fishers.Add(FisherCollection[newIndex]);
                    }   
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Fisher> tempListOfRemovedItems = e.OldItems.OfType<Fisher>().ToList();
                        Fishers.Delete(tempListOfRemovedItems[0].FisherID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Fisher> tempListOfFishers = e.NewItems.OfType<Fisher>().ToList();
                        Fishers.Update(tempListOfFishers[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public  int Count
        {
            get { return FisherCollection.Count; }
        }

        public void AddRecordToRepo(Fisher fisher)
        {
            if (fisher == null)
                throw new ArgumentNullException("Error: The argument is Null");

            FisherCollection.Add(fisher);
        }

        public void UpdateRecordInRepo(Fisher fisher)
        {
            if (fisher.FisherID == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < FisherCollection.Count)
            {
                if (FisherCollection[index].FisherID == fisher.FisherID)
                {
                    FisherCollection[index] = fisher;
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
            while (index < FisherCollection.Count)
            {
                if (FisherCollection[index].FisherID == id)
                {
                    FisherCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(Fisher fisher, out List<string>messages)
        {
            
            messages = new List<string>();
            
            if(fisher.FisherName.Length<5)
                messages.Add("Fisher's name must be at least 5 characters long");

            if (fisher.LandingSite == null)
                messages.Add("Fisher's landing site cannot be empty");

            return messages.Count == 0;
        }
    }
}
