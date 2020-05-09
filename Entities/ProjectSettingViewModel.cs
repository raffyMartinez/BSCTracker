using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BSCTracker.Entities
{
    public class ProjectSettingViewModel
    {
        public ObservableCollection<ProjectSetting> ProjectSettingCollection { get; set; }
        private ProjectSettingRepository ProjectSettings { get; set; }

        public event EventHandler<EntityChangedEventArgs> EntityChanged;

        public EditedEntity EditedEntity { get; private set; }
        public ProjectSettingViewModel()
        {
            ProjectSettings = new ProjectSettingRepository();
            ProjectSettingCollection = new ObservableCollection<ProjectSetting>(ProjectSettings.ProjectSettings);
            ProjectSettingCollection.CollectionChanged += ProjectSettings_CollectionChanged;
        }

        public int Count
        {
            get { return ProjectSettingCollection.Count; }
        }

        public bool ProjectNameExists(string projectName)
        {
            foreach (ProjectSetting ps in ProjectSettingCollection)
            {
                if (ps.ProjectName == projectName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanDeleteEntity(ProjectSetting ps)
        {
            return BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t => t.ProjectSetting.ProjectID == ps.ProjectID).ToList().Count == 0

                &&

                BSCEntities.FisherGPSViewModel.FisherGPSCollection
                .Where(l => l.ProjectSetting.ProjectID == ps.ProjectID).ToList().Count == 0;
        }
        public ProjectSetting GetProjectSetting(string projectID)
        {
            return ProjectSettingCollection.FirstOrDefault(n => n.ProjectID == projectID);

        }
        public List<ProjectSetting> GetAllProjectSettings()
        {
            return ProjectSettingCollection.ToList();
        }

        private void ProjectSettings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ProjectSetting editedEntity = new ProjectSetting();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int newIndex = e.NewStartingIndex;
                        editedEntity = ProjectSettingCollection[newIndex];

                        if (ProjectSettings.Add(editedEntity))
                            EditedEntity = new EditedEntity(EditAction.Add,  editedEntity);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        List<ProjectSetting> tempListOfRemovedItems = e.OldItems.OfType<ProjectSetting>().ToList();
                        editedEntity = tempListOfRemovedItems[0];
                        
                        if (ProjectSettings.Delete(editedEntity.ProjectID))
                            EditedEntity = new EditedEntity(EditAction.Delete, editedEntity);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        List<ProjectSetting> tempListOfProjectSettings = e.NewItems.OfType<ProjectSetting>().ToList();
                        editedEntity = tempListOfProjectSettings[0];
                        
                        if (ProjectSettings.Update(tempListOfProjectSettings[0]))      // As the IDs are unique, only one row will be effected hence first index only
                            EditedEntity = new EditedEntity(EditAction.Update, editedEntity);
                    }       
                    break;
            }
            EntityChangedEventArgs args = new EntityChangedEventArgs(editedEntity.GetType().Name,editedEntity);
            EntityChanged?.Invoke(this, args);
        }

        public void AddRecordToRepo(ProjectSetting ps)
        {
            if (ps == null)
                throw new ArgumentNullException("Error: The argument is Null");
            ProjectSettingCollection.Add(ps);
        }

        public void UpdateRecordInRepo(ProjectSetting ps)
        {
            if (ps.ProjectID == null)
                throw new Exception("Error: ID cannot be null");

            int index = 0;
            while (index < ProjectSettingCollection.Count)
            {
                if (ProjectSettingCollection[index].ProjectID == ps.ProjectID)
                {
                    ProjectSettingCollection[index] = ps;
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
            while (index < ProjectSettingCollection.Count)
            {
                if (ProjectSettingCollection[index].ProjectID == id)
                {
                    ProjectSettingCollection.RemoveAt(index);
                    break;
                }
                index++;
            }
        }

        public bool EntityValidated(ProjectSetting projectSetting, out List<string> messages, bool isNew = false)
        {

            messages = new List<string>();

            if (projectSetting.ProjectName.Length < 5)
                messages.Add("Project's name must be at least 5 characters long");


            if (projectSetting.DateStart == null)
                messages.Add("Projetct's starting date cannot be empty");


            return messages.Count == 0;
        }
    }
}
