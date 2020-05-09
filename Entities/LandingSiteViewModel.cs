using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities

{
    public class LandingSiteViewModel
    {
        public ObservableCollection<LandingSite> LandingSiteCollection { get; set; }
        private LandingSiteRepository LandingSites { get; set; }

        public event EventHandler<EntityChangedEventArgs> EntityChanged;

        public LandingSiteViewModel()
        {

            LandingSites = new LandingSiteRepository();
            LandingSiteCollection = new ObservableCollection<LandingSite>(LandingSites.landingSites);
            LandingSiteCollection.CollectionChanged += LandingSiteCollection_CollectionChanged;
        }

        public int Count

        {
            get { return LandingSiteCollection.Count; }
        }

        public bool LandingSiteNameExists(string landingSiteName)
        {
            foreach (LandingSite ls in LandingSiteCollection)
            {
                if (ls.LandingSiteName == landingSiteName)
                {
                    return true;
                }
            }
            return false;
        }

        public LandingSite GetLandingSite(string landingSiteID)
        {
            return LandingSiteCollection.FirstOrDefault(n => n.LandingSiteID == landingSiteID);

        }

        public bool CanDeleteEntity(LandingSite ls)
        {
            return BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t => t.LandingSite.LandingSiteID == ls.LandingSiteID).ToList().Count == 0

                &&

                BSCEntities.FisherViewModel.FisherCollection
                .Where(l => l.LandingSite.LandingSiteID == ls.LandingSiteID).ToList().Count == 0;
        }
        public List<LandingSite> GetAllLandingSites()
        {
            return LandingSiteCollection.ToList();
        }

        private void LandingSiteCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LandingSite editedLandingSite=new LandingSite();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        editedLandingSite = LandingSiteCollection[newIndex];
                        LandingSites.Add(editedLandingSite);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<LandingSite> tempListOfRemovedItems = e.OldItems.OfType<LandingSite>().ToList();
                        editedLandingSite = tempListOfRemovedItems[0];
                        LandingSites.Delete(editedLandingSite.LandingSiteID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<LandingSite> tempListOfLandingSites = e.NewItems.OfType<LandingSite>().ToList();
                        editedLandingSite = tempListOfLandingSites[0];
                        LandingSites.Update(editedLandingSite);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
            EntityChangedEventArgs args = new EntityChangedEventArgs(editedLandingSite.GetType().Name,editedLandingSite);
            EntityChanged?.Invoke(this, args);
        }

        public void AddRecordToRepo(LandingSite ls)
        {
            if (ls == null)
                throw new ArgumentNullException("Error: The argument is Null");
            LandingSiteCollection.Add(ls);
        }

        public void UpdateRecordInRepo(LandingSite ls)
        {
            if (ls.LandingSiteID == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < LandingSiteCollection.Count)
            {
                if (LandingSiteCollection[index].LandingSiteID == ls.LandingSiteID)
                {
                    LandingSiteCollection[index] = ls;
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
            while (index < LandingSiteCollection.Count)
            {
                if (LandingSiteCollection[index].LandingSiteID == id)
                {
                    LandingSiteCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(LandingSite landingSite, out List<string> messages, bool isNew = false)
        {

            messages = new List<string>();

            if (landingSite.LandingSiteName.Length < 5)
                messages.Add("Landing site's name must be at least 5 characters long");


            if (landingSite.Municipality == null)
                messages.Add("Municipality cannot be empty");

            return messages.Count == 0;
        }
    }
}
