using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class Municipality
    {
        public int MunicipalityID { get; set; }
        public string MunicipalityName { get; set; }
        public Province Province { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public bool IsCoastal { get; set; }

        public override string ToString()
        {
            return $"{MunicipalityName}, {Province.ProvinceName}";
        }

    }
}
