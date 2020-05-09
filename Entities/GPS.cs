using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class GPS
    {
        public GPS()
        {

        }

        public GPS(string id, string brand, string model, string assignedName, string sdCardCapcity, DateTime? dateAcquired)
        {
            ID = id;
            Brand = brand;
            Model = model;
            AssignedName = assignedName;
            SDCardCapacity = sdCardCapcity;
            DateAcquired = dateAcquired;
        }

        public string ID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }

        public string AssignedName { get; set; }
        public DateTime? DateAcquired { get; set; }
        public string SDCardCapacity { get; set; }

        public override string ToString()
        {
            return $"{AssignedName} ({Brand}-{Model})";
        }
    }
}
