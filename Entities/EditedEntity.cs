using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCTracker.Entities
{
    public enum EditAction
    {
        Add,
        Update,
        Delete
    }

    public class EditedEntity
    {
        public EditAction EditAction { get; set; }

        public object BSCEntity { get; set; }
        public EditedEntity(EditAction editAction,  object bscEntity)
        {
            EditAction = editAction;
            string typeName = bscEntity.GetType().Name;
            if (typeName == "ProjectSetting"
                || typeName == "Gear"
                || typeName == "LandingSite"
                || typeName == "Fisher"
                || typeName == "Sampling")
            {
                BSCEntity = bscEntity;
            }
            else
            {
                throw new Exception("Error: BSCEntity is of the wrong type");
            }
        }
    }

}
