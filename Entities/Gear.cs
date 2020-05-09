using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class Gear
    {
        public string GearName { get; set; }

        public Gear(){}

        public string GearID { get; set; }
        public Gear(string gearName, string code, string gearID)
        {
            GearName = gearName;
            Code = code;
            GearID = gearID;
        }

        public override string ToString()
        {
            return GearName;
        }

        public string Code { get; set; }


    }
}
