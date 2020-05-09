using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BSCTracker.Entities
{
    public class SamplingViewModel
    {
        public ObservableCollection<Sampling> SamplingCollection { get; set; }
        private SamplingRepository Samplings { get; set; }

        public EditedEntity EditedEntity { get; private set; }

        public SamplingViewModel()
        {
            Samplings = new SamplingRepository();
            SamplingCollection = new ObservableCollection<Sampling>(Samplings.Samplings);
            SamplingCollection.CollectionChanged += SamplingCollection_CollectionChanged;
        }


        public int Count
        {
            get { return SamplingCollection.Count; }
        }

        public List<Sampling> GetAllSamplings(ProjectSetting pr, LandingSite ls, Gear g, DateTime month)
        {
            return SamplingCollection
               .Where(t => t.ProjectSetting.ProjectID == pr.ProjectID)
               .Where(t => t.LandingSite.LandingSiteID == ls.LandingSiteID)
               .Where(t => t.Gear.GearName == g.GearName)
               .Where(t => t.DateTimeSampled >= month)
               .Where(t => t.DateTimeSampled < month.AddMonths(1)).ToList();
        }

        public List<Sampling> GetAllSamplings(LandingSite ls)
        {
            return SamplingCollection
               .Where(t => t.LandingSite.LandingSiteID == ls.LandingSiteID).ToList();

        }

        public List<Sampling> GetAllSamplings(Fisher f)
        {
            return SamplingCollection
               .Where(t => t.FisherGPS.Fisher.FisherID== f.FisherID).ToList();

        }
        public List<Sampling> GetAllSamplings(Gear g)
        {
            return SamplingCollection
               .Where(t => t.Gear.GearID == g.GearID).ToList();

        }

        public Sampling GetSampling(string rowID)
        {
            return SamplingCollection.FirstOrDefault(n => n.RowID == rowID);

        }
        public List<Sampling> GetAllSamplings()
        {
            return SamplingCollection.ToList();
        }

        private void SamplingCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Sampling editedEntity = SamplingCollection[newIndex];
                        if (Samplings.Add(editedEntity))
                            EditedEntity = new EditedEntity(EditAction.Add, editedEntity);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Sampling> tempListOfRemovedItems = e.OldItems.OfType<Sampling>().ToList();
                        Sampling editedEntity = tempListOfRemovedItems[0];
                        if (Samplings.Delete(editedEntity.RowID))
                            EditedEntity = new EditedEntity(EditAction.Delete, editedEntity);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Sampling> tempListOfSamplings = e.NewItems.OfType<Sampling>().ToList();
                        Sampling editedEntity = tempListOfSamplings[0];
                        if (Samplings.Update(editedEntity))      // As the IDs are unique, only one row will be effected hence first index only
                            EditedEntity = new EditedEntity(EditAction.Update, editedEntity);
                    }
                    break;
            }
        }

        public void AddRecordToRepo(Sampling s)
        {
            EditedEntity = null;
            if (s == null)
                throw new ArgumentNullException("Error: The argument is Null");
            SamplingCollection.Add(s);
        }

        public void UpdateRecordInRepo(Sampling s)
        {
            EditedEntity = null;
            if (s.RowID == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < SamplingCollection.Count)
            {
                if (SamplingCollection[index].RowID == s.RowID)
                {
                    SamplingCollection[index] = s;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(string id)
        {
            EditedEntity = null;
            if (id == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < SamplingCollection.Count)
            {
                if (SamplingCollection[index].RowID == id)
                {
                    SamplingCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(Sampling sampling, out List<EntityValidationMessage> messages)//, bool isNew = false)
        {
            DateTime? gpsAssignedDate = null;
            DateTime? gpsReturnedDate = null;

            if (sampling.FisherGPS != null)
            {
                gpsAssignedDate = (DateTime)sampling.FisherGPS.DateAssigned;
                gpsReturnedDate = sampling.FisherGPS.DateReturned == null ? DateTime.Now : (DateTime)sampling.FisherGPS.DateReturned;
            }

            messages = new List<EntityValidationMessage>();
            if (sampling.ProjectSetting == null)
            {
                messages.Add(new EntityValidationMessage("Project cannot be empty"));
            }

            if(!(sampling.NSAPSamplingID.Length==0 || sampling.NSAPSamplingID.Length>3))
            {
                messages.Add(new EntityValidationMessage("NSAP sampling ID should be more than 3 characters long"));
            }

            if (sampling.FisherGPS == null)
                messages.Add(new EntityValidationMessage("Fisher assigned with GPS should be complete"));

            if(sampling.LandingSite==null)
                messages.Add(new EntityValidationMessage("Landing site cannot be empty"));
            

            if (sampling.Gear==null)
                messages.Add(new EntityValidationMessage("Gear used cannot be empty"));

            if (sampling.FisherGPS != null)
            {
                if (sampling.DateTimeDeparted == null)
                {
                    messages.Add(new EntityValidationMessage("Date and time of departure cannot be empty"));
                }
                else if (sampling.DateTimeDeparted > DateTime.Now)
                {
                    messages.Add(new EntityValidationMessage("Date and time of departure cannot a future date"));
                }
                else if (!(sampling.DateTimeDeparted >= gpsAssignedDate && sampling.DateTimeDeparted <= ((DateTime)gpsReturnedDate).AddHours(24)))
                {
                    messages.Add(new EntityValidationMessage("Date and time of departure must be within dates when GPS was assigned to fisher"));
                }

                if (sampling.DateTimeArrived == null)
                {
                    messages.Add(new EntityValidationMessage("Date and time of arrival cannot be empty"));
                }
                else if (sampling.DateTimeArrived > DateTime.Now)
                {
                    messages.Add(new EntityValidationMessage("Date and time of arrival cannot a future date"));
                }
                else if (!(sampling.DateTimeArrived >= gpsAssignedDate && sampling.DateTimeArrived <= ((DateTime)gpsReturnedDate).AddHours(24)))
                {
                    messages.Add(new EntityValidationMessage("Date and time of arrival must be within dates when GPS was assigned to fisher"));
                }

                if (sampling.DateTimeSampled == null)
                {
                    messages.Add(new EntityValidationMessage("Date and time of sampling cannot be empty"));
                }
                else if (sampling.DateTimeSampled > DateTime.Now)
                {
                    messages.Add(new EntityValidationMessage("Sampling date and time cannot be a future date"));
                }
                else if (!(sampling.DateTimeSampled >= gpsAssignedDate && sampling.DateTimeSampled <= ((DateTime)gpsReturnedDate).AddHours(24)))
                {
                    messages.Add(new EntityValidationMessage("Date and time of sampling must be within dates when GPS was assigned to fisher"));
                }

            }





            return messages.Count == 0;
        }
    }
}
