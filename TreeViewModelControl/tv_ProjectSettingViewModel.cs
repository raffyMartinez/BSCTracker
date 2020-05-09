using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BSCTracker.Entities;

namespace BSCTracker.TreeViewModelControl
{


    public class tv_ProjectSettingViewModel :TreeViewItemViewModel
    {
        public TreeViewControl ParentTreeView { get; set; }
        //public readonly ProjectSetting _projectSetting;
        public  ProjectSetting _projectSetting;

        public tv_ProjectSettingViewModel(ProjectSetting projectSetting)
            :base (null, true)
        {
            _projectSetting = projectSetting;
        }

        public void  Edit(ProjectSetting projectSetting)
        {
            _projectSetting = projectSetting;
        }

        public string ProjectName
        {   
            get { return _projectSetting.ProjectName; }
        }

        public void Add(LandingSite ls)
        {
            base.Children.Add(new tv_LandingSiteViewModel(ls, this));
        }

        protected override void LoadChildren()
        {
            List<LandingSite> tempList = new List<LandingSite>();
            foreach(var s in BSCEntities.SamplingViewModel.SamplingCollection
                .Where(t=>t.ProjectSetting.ProjectID==_projectSetting.ProjectID))
            {
                if(!tempList.Contains(s.LandingSite))
                {
                    tempList.Add(s.LandingSite);
                }
            }

            foreach(var ls in tempList)
            {
                base.Children.Add(new tv_LandingSiteViewModel(ls, this));
            }
        }



    }
}
