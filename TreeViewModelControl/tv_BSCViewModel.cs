using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCTracker.Entities;
using System.Collections.ObjectModel;

namespace BSCTracker.TreeViewModelControl
{
    public class tv_BSCViewModel
    {
        public TreeViewModelControl.TreeViewControl ParentTreeView { get; set; }
        private List<tv_ProjectSettingViewModel> _listProjectSettings = new List<tv_ProjectSettingViewModel>();
        public  ReadOnlyCollection<tv_ProjectSettingViewModel> Projects { get { return _listProjectSettings.AsReadOnly(); } }
        
        public void Add(ProjectSetting p)
        {
            tv_ProjectSettingViewModel tvp = new tv_ProjectSettingViewModel(p);
            _listProjectSettings.Add(tvp);
            ParentTreeView.treeView.Items.Refresh();
        }
        public tv_BSCViewModel(List<ProjectSetting> projects, TreeViewModelControl.TreeViewControl tree)
            
        {
            ParentTreeView = tree;
            foreach(ProjectSetting p in projects)
            {
                tv_ProjectSettingViewModel tvp = new tv_ProjectSettingViewModel(p);
                _listProjectSettings.Add(tvp) ;
            }

        }
    }
}