using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class FisherGPSDetail
    {
        private FisherGPS _fg;
        public FisherGPSDetail(FisherGPS fg)
        {
            _fg = fg;
        }

        public DateTime? DateGPSAssigned
        {
            get { return _fg.DateAssigned; }
        }

        public string FisherName
        {
            get { return _fg.Fisher.FisherName; }
        }

        public string ProjectName
        {
            get { return _fg.ProjectSetting.ProjectName; }
        }

        public string GPSBrand
        {
            get { return _fg.GPS.Brand; }
        }

        public string GPSModel
        {
            get { return _fg.GPS.Model; }
        }

        public string AssignedName
        {
            get { return _fg.GPS.AssignedName; }
        }

        public string SDCardCapacity
        {
            get { return _fg.GPS.SDCardCapacity; }
        }

        public DateTime? DateGPSAcquired
        {
            get { return _fg.GPS.DateAcquired; }
        }
    }
}
