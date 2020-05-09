using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSCTracker.Entities;

namespace BSCTracker.TreeViewModelControl
{
    class tv_FisherViewModel:TreeViewItemViewModel
    {

        public readonly Fisher _fisher;

        public tv_FisherViewModel(Fisher fisher, tv_LandingSiteViewModel ls):base(ls,true)
        {
            _fisher = fisher;
        }

        public string FisherName
        {
            get { return _fisher.FisherName; }
        }

        protected override void LoadChildren()
        {

        }
    }
}
