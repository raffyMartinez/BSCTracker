using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class FisherAndGPS
    {
        public Fisher Fisher { get; set; }
        public GPS GPS { get; set; }
        public ProjectSetting ProjectSetting { get; set; }  
    }
}
