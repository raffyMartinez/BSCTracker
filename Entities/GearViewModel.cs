using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace BSCTracker.Entities
{
    public class GearViewModel
    {
        public ObservableCollection<Gear> GearCollection { get; set; }
        private GearRepository Gears { get; set; }



        public GearViewModel()
        {
            Gears = new GearRepository();
            GearCollection = new ObservableCollection<Gear>(Gears.Gears);
            GearCollection.CollectionChanged += Gearss_CollectionChanged;
        }
        public List<Gear> GetAllGears()
        {
            return GearCollection.ToList();
        }
        public bool CanDeleteEntity(Gear g)
        {
            return BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t => t.Gear.GearName == g.GearName).ToList().Count == 0;
        }
        public bool GearNameExist(string gearName)
        {
            foreach (Gear g in GearCollection)
            {
                if (g.GearName == gearName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool GearCodeExist(string gearCode)
        {
            foreach (Gear g in GearCollection)
            {
                if (g.Code == gearCode)
                {
                    return true;
                }
            }
            return false;
        }
        public Gear GetGear(string gear)
        {
            return GearCollection.FirstOrDefault(n => n.GearID == gear);

        }
        private void Gearss_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Gears.Add(GearCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Gear> tempListOfRemovedItems = e.OldItems.OfType<Gear>().ToList();
                        Gears.Delete(tempListOfRemovedItems[0].GearName);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Gear> tempList = e.NewItems.OfType<Gear>().ToList();
                        Gears.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return GearCollection.Count; }
        }

        public void AddRecordToRepo(Gear gear)
        {
            if (gear == null)
                throw new ArgumentNullException("Error: The argument is Null");
            GearCollection.Add(gear);
        }

        public void UpdateRecordInRepo(Gear gear)
        {
            if (gear.GearName == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < GearCollection.Count)
            {
                if (GearCollection[index].GearID == gear.GearID)
                {
                    GearCollection[index] = gear;
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
            while (index < GearCollection.Count)
            {
                if (GearCollection[index].GearID == id)
                {
                    GearCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(Gear gear, out List<EntityValidationMessage> messages, bool isNew = false, string oldName="", string oldCode="")
        {

            messages = new List<EntityValidationMessage>();

            if (gear.GearName.Length < 3)
                messages.Add( new EntityValidationMessage( "Fishing gear's name must be at least 3 characters long"));

            if(gear.Code.Length==0)
            {
                messages.Add(new EntityValidationMessage("Gear code cannot be empty"));
            }
            else if(gear.Code.Length>3)
            {
                messages.Add(new EntityValidationMessage("Gear code cannot be more than 3 letters"));
            }

            if (isNew && gear.GearName.Length > 0 && GearNameExist(gear.GearName))
                messages.Add( new EntityValidationMessage("Gear name already used"));

            if (isNew && gear.Code.Length > 0 && GearCodeExist(gear.Code))
                messages.Add(new EntityValidationMessage("Gear code already used"));

            if (!isNew && gear.GearName.Length > 0
                 && oldName!= gear.GearName
                && GearNameExist(gear.GearName))
                messages.Add(new EntityValidationMessage("Gear name already used"));

            if (!isNew && gear.Code.Length > 0
                 && oldCode != gear.Code
                && GearCodeExist(gear.Code))
                messages.Add(new EntityValidationMessage("Gear code already used"));

            return messages.Count == 0;
        }
    }
}
