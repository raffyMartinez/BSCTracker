using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public class ProjectSetting
    {
        public ProjectSetting() { }
        public ProjectSetting(string projectID, string projectName, DateTime? dateStart)
        {
            ProjectID = projectID;
            ProjectName = projectName;
            DateStart = dateStart;

        }
        public DateTime? DateStart { get; set; }
        public string ProjectName { get; set; }
        public string ProjectID { get; set; }

        public override string ToString()
        {
            return $@"{ProjectName}  ({((DateTime)DateStart).ToString("MMM-dd-yyyy")})";
        }

    }
}
