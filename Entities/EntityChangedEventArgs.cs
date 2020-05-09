using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
   public class EntityChangedEventArgs:EventArgs
    {
        public string EntityType { get; private set; }
        public object BSCEntity { get; private set; }
        
        public EntityChangedEventArgs(string entityType, object bscEntity)
        {
            EntityType = entityType;
            BSCEntity = bscEntity;
        }
    }
}
