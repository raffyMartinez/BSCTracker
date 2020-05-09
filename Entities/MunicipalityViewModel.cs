using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BSCTracker.Entities
{
    public class MunicipalityViewModel
    {
        public ObservableCollection<Municipality> MunicipalityCollection { get; set; }
        private MunicipalityRepository Municipalities{ get; set; }



        public MunicipalityViewModel()
        {
            Municipalities = new MunicipalityRepository();
            MunicipalityCollection = new ObservableCollection<Municipality>(Municipalities.Municipalities);
            MunicipalityCollection.CollectionChanged += Municipalities_CollectionChanged;
        }
        public List<Municipality> GetAllMunicipalities()
        {
            return MunicipalityCollection.ToList();
        }

        public List<Municipality> GetAllMunicipalities(Province p)
        {
            List<Municipality> tempMunicipalities = new List<Municipality>();
            foreach(Municipality m in MunicipalityCollection)
            {
                if(m.Province.ProvinceID==p.ProvinceID)
                {
                    tempMunicipalities.Add(m);
                }
            }
            return tempMunicipalities;
        }
        public bool CanDeleteEntity(Municipality m)
        {
            return false;
        }
        public bool MunicipalityNameExists(Province p ,string municipalityName)
        {
            if (p == null)
                throw new ArgumentNullException("Error: Province is Null");

            foreach (Municipality m in MunicipalityCollection)
            {
                if ( m.Province.ProvinceID == p.ProvinceID &&  m.MunicipalityName== municipalityName)
                {
                    return true;
                }
            }
            return false;
        }
        public Municipality GetMunicipality(int municipalityID)
        {
            return MunicipalityCollection.FirstOrDefault(n => n.MunicipalityID == municipalityID);

        }
        private void Municipalities_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        Municipalities.Add(MunicipalityCollection[newIndex]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<Municipality> tempListOfRemovedItems = e.OldItems.OfType<Municipality>().ToList();
                        Municipalities.Delete(tempListOfRemovedItems[0].MunicipalityID);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<Municipality> tempList = e.NewItems.OfType<Municipality>().ToList();
                        Municipalities.Update(tempList[0]);      // As the IDs are unique, only one row will be effected hence first index only
                    }
                    break;
            }
        }

        public int Count
        {
            get { return MunicipalityCollection.Count; }
        }

        public void AddRecordToRepo(Municipality m)
        {
            if (m == null)
                throw new ArgumentNullException("Error: The argument is Null");
            MunicipalityCollection.Add(m);
        }

        public void UpdateRecordInRepo(Municipality m)
        {
            if (m.MunicipalityID== 0)
                throw new Exception("Error: ID cannot be zero");

            int index = 0;
            while (index < MunicipalityCollection.Count)
            {
                if (MunicipalityCollection[index].MunicipalityID== m.MunicipalityID)
                {
                    MunicipalityCollection[index] = m;
                    break;
                }
                index++;
            }
        }

        public void DeleteRecordFromRepo(int id)
        {
            if (id == 0)
                throw new Exception("Record ID cannot be null");

            int index = 0;
            while (index < MunicipalityCollection.Count)
            {
                if (MunicipalityCollection[index].MunicipalityID == id)
                {
                    MunicipalityCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }


        public bool EntityValidated(Dictionary<string, string> formValues, out List<string> messages)
        {
            messages = new List<string>();

            string stringLongitude = formValues["txtLongitude"];
            string stringLatitude = formValues["txtLatitude"];



            if (stringLongitude.Length > 0)
            {
                if (double.TryParse(stringLongitude, out double v))
                {
                    //reserved for future use
                }
                else
                {
                    messages.Add("Longitude must be a numeric value");
                }
            }
            else
            {
                messages.Add("Longitude cannot be empty");
            }

            if (stringLatitude.Length > 0)
            {
                if (double.TryParse(stringLatitude, out double v))
                {
                    //reserved for future use
                }
                else
                {
                    messages.Add("Latitude must be a numeric value");
                }
            }
            else
            {
                messages.Add("Latitude cannot be empty");
            }

            return messages.Count == 0;
        }
        public bool EntityValidated(Municipality m, out List<string> messages, bool isNew = false, string oldName = "")
        {

            messages = new List<string>();

            if (m.MunicipalityName.Length < 3)
                messages.Add("Municipality name must be at least 3 characters long");

            if (isNew && m.MunicipalityName.Length > 0 &&   MunicipalityNameExists(m.Province, m.MunicipalityName))
                messages.Add("Municipality name already used");

            if (!isNew && m.MunicipalityName.Length > 0
                 && oldName != m.MunicipalityName
                && MunicipalityNameExists(m.Province, m.MunicipalityName))
                messages.Add("Municipality name already used");

            return messages.Count == 0;
        }
    }
}
