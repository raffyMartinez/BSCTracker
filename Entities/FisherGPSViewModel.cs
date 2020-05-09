using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MoreLinq;
using BSCTracker.Utilities;



namespace BSCTracker.Entities
{
    public class FisherGPSViewModel
    {
        public ObservableCollection<FisherGPS> FisherGPSCollection { get; set; }
        private FisherGPSRepository FisherGPSes { get; set; }




        public FisherGPSViewModel()
        {
            FisherGPSes = new FisherGPSRepository();
            FisherGPSCollection = new ObservableCollection<FisherGPS>(FisherGPSes.FisherGPSes);
            FisherGPSCollection.CollectionChanged += FisherGPSes_CollectionChanged;
        }

        public List<FisherGPS> GetAllFisherGPS(Fisher f = null)
        {

            if (f == null)
            {
                return FisherGPSCollection.ToList();
            }
            else
            {
                return FisherGPSCollection.Where(t => t.Fisher.FisherID == f.FisherID).ToList();
            }
        }

        public FisherGPS GetFisherGPS( FisherGPS fg, DateTime date)
        {
            try
            {
                return FisherGPSCollection
                    .Single(a => a.GPS.ID == fg.GPS.ID
                        && a.DateAssigned <= date
                        && a.RowID != fg.RowID
                        && (a.DateReturned == null ? DateTime.Now : a.DateReturned) >= date);
            }
            catch (System.InvalidOperationException)
            {
                return null;
            }
            catch(Exception ex)
            {
                Logger.Log(ex);
                return null;
            }



        }

        public FisherGPS LastGPSUser(GPS gps, FisherGPS fg)
        {
            return GetAllFisherGPS(gps, fg)
                .OrderByDescending(t => t.DateAssigned).FirstOrDefault();

        }
        public FisherGPS LastGPSUser(GPS gps)
        {
            return GetAllFisherGPS(gps)
                .OrderByDescending(t => t.DateAssigned).FirstOrDefault();

        }
        public List<FisherGPS> GetAllFisherGPS(GPS g, FisherGPS fg=null)
        {
            if (fg == null)
            {
                return FisherGPSCollection.Where(t => t.GPS.ID == g.ID).ToList();
            }
            else

            {
                return FisherGPSCollection
                    .Where(t => t.GPS.ID == g.ID)
                    .Where(t=>t.DateReturned!=null)
                    .Where(t=>t.RowID!=fg.RowID) .ToList();
            }
        }



        public bool CanAssignGPS(Fisher f)
        {
            if (FisherGPSCollection.Count > 0)
            {
                if (GetAllFisherGPS(f).Count == 0)
                {
                    return true;
                }
                else

                {
                    var maxRow = FisherGPSCollection
                        .Where(d => d.Fisher.FisherID == f.FisherID)
                        .MaxBy(d => d.DateAssigned).ToList()[0];
                    return maxRow.DateReturned != null;

                }

            }
            else
            {
                return true;
            }

        }

        public FisherGPS GetFisherGPS(string ID)
        {
            return FisherGPSCollection.FirstOrDefault(n => n.RowID == ID);

        }

        public bool CanDeleteEntity(FisherGPS fg)
        {
            return BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t => t.FisherGPS.RowID == fg.RowID).ToList().Count == 0;


        }

        public List<FisherGPSDetail> GetAssignedGPS()
        {
            var listFGD = from fg in FisherGPSCollection
                          where fg.DateReturned == null
                          select new FisherGPSDetail(fg);
            return listFGD.ToList();


        }
        private void FisherGPSes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        FisherGPSes.Add(FisherGPSCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<FisherGPS> tempListOfRemovedItems = e.OldItems.OfType<FisherGPS>().ToList();
                        FisherGPSes.Delete(tempListOfRemovedItems[0].RowID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<FisherGPS> tempListOfFisherGPS = e.NewItems.OfType<FisherGPS>().ToList();
                        FisherGPSes.Update(tempListOfFisherGPS[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }



        public int Count()
        {
            return FisherGPSCollection.Count;
        }

        public void AddRecordToRepo(FisherGPS fg)
        {
            if (fg == null)
                throw new ArgumentNullException("Error: The argument is Null");
            FisherGPSCollection.Add(fg);
        }

        public void UpdateRecordInRepo(FisherGPS fg)
        {
            if (fg.RowID == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < FisherGPSCollection.Count)
            {
                if (FisherGPSCollection[index].RowID == fg.RowID)
                {
                    FisherGPSCollection[index] = fg;
                    break;
                }
                index++;
            }
        }




        public FisherGPS LatestReturnedDateByFisher(Fisher f)
        {
            try
            {
                return GetAllFisherGPS(f)
                    .OrderByDescending(t => t.DateReturned).First();
            }
            catch
            {
                return null;
            }
        }

        public FisherGPS GetFisherGPS(Fisher f, GPS g, ProjectSetting p)
        {
            return (from fg in FisherGPSCollection
                    where fg.Fisher.FisherID == f.FisherID
                    where fg.GPS.ID == g.ID
                    where fg.ProjectSetting.ProjectID == p.ProjectID
                    select fg).FirstOrDefault();


        }
        public FisherGPS GetFisherGPS(Fisher fisher, GPS gps,
                                      ProjectSetting projectSetting,
                                      DateTime dateTimeDeparted, out EntityValidationMessage errorMessage)
        {
            errorMessage = new EntityValidationMessage("");
            FisherGPS rv = null;
            foreach (FisherGPS fg in FisherGPSCollection.OrderBy(p => p.DateAssigned))
            {
                if (fg.Fisher.FisherID == fisher.FisherID
                    && fg.GPS.ID == gps.ID
                    && fg.ProjectSetting.ProjectID == projectSetting.ProjectID)
                {
                    if (fg.DateReturned != null)
                    {

                    }
                    rv = fg;
                    break;
                }
            }
            if (rv == null)
            {
                errorMessage = new EntityValidationMessage("No matching Fisher-GPS record was found. Check date and time departed");
            }
            return rv;
        }
        public void DeleteRecordFromRepo(string id)
        {
            if (id == null)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < FisherGPSCollection.Count)
            {
                if (FisherGPSCollection[index].RowID == id)
                {
                    FisherGPSCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }


        public bool EntityValidated(FisherGPS fg, out List<EntityValidationMessage> entityMessages, bool isNew)
        {
            entityMessages = new List<EntityValidationMessage>();
            DateTime? dateToCompare=null;
            string dateFormat = "MMM-dd-yyyy";
            string comparedDate="";
            FisherGPS FisherWithLatestReturnedDate = LatestReturnedDateByFisher(fg.Fisher);

            if (FisherWithLatestReturnedDate == null && fg.ProjectSetting != null)
            {
                dateToCompare = (DateTime)fg.ProjectSetting.DateStart;
                comparedDate = $"start of Project which is {((DateTime)fg.ProjectSetting.DateStart).ToString(dateFormat)}";
            }
            else if(FisherWithLatestReturnedDate!=null)
            {
             if (FisherWithLatestReturnedDate.DateReturned == null)
                {
                    dateToCompare = (DateTime)FisherWithLatestReturnedDate.DateAssigned;
                    comparedDate = $"assigned date of last GPS assigned to fisher which is {((DateTime)FisherWithLatestReturnedDate.DateAssigned).ToString(dateFormat)}";
                }
                else
                {
                    dateToCompare = (DateTime)FisherWithLatestReturnedDate.DateReturned;
                    comparedDate = $"returned date of last GPS assigned to fisher which is {((DateTime)FisherWithLatestReturnedDate.DateReturned).ToString(dateFormat)}";
                }
            }

            if (fg.DateAssigned == null)
            {
                entityMessages.Add(new EntityValidationMessage("Assigned date cannot be empty"));
            }
            else
            {
                if (dateToCompare!=null &&  fg.DateAssigned < dateToCompare)
                {
                    entityMessages.Add(new EntityValidationMessage($"Date assigned should be after {comparedDate}"));
                }
                else if (fg.DateAssigned > DateTime.Now)
                {
                    entityMessages.Add(new EntityValidationMessage("Date assigned cannot be a future date"));
                }
                else
                {
                    var f = GetFisherGPS(fg,  (DateTime)fg.DateAssigned);
                    if(f!=null)
                    {
                        entityMessages.Add(new EntityValidationMessage("Cannot use assigned date because GPS is not available at this time"));
                    }
                }
            }

            if (fg.DateReturned != null)
            {
                if (isNew)
                {
                    entityMessages.Add(new EntityValidationMessage("Returned date is not required for new GPS assignment", MessageType.Warning));
                }

                if (fg.DateReturned < fg.DateAssigned)
                {
                    entityMessages.Add(new EntityValidationMessage("Date returned must be on or after date assigned"));
                }
                else if (fg.DateReturned > DateTime.Now)
                {
                    entityMessages.Add(new EntityValidationMessage("Date returned cannot be a future date"));
                }
                else
                {
                    var f = GetFisherGPS(fg, (DateTime)fg.DateReturned);
                    if (f != null)
                    {
                        entityMessages.Add(new EntityValidationMessage("Cannot use return date because GPS is not available at this time"));
                    }
                }
            }
            else

            {
                //check if current user is the latest GPS user. If not then date returned must not be null
                var lastUser = LastGPSUser(fg.GPS);
                if(fg.DateAssigned<lastUser.DateAssigned)
                {
                    entityMessages.Add(new EntityValidationMessage("Return date cannot be empty because this GPS has been used in a later date"));
                }

            }

            if (fg.GPS == null)
                entityMessages.Add(new EntityValidationMessage("GPS cannot be empty"));

            if (fg.ProjectSetting == null)
                entityMessages.Add(new EntityValidationMessage("Project cannot be empty"));


            return entityMessages.Count == 0;
        }
    }
}
