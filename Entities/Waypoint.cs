using System;
using System.Collections.Generic;
using BSCTracker.Utilities;


namespace BSCTracker.Entities
{
    /// <summary>
    /// Waypoint class
    /// Since there is no WKT implementation of a waypoint, a waypoint will be saved as text using this format:
    /// POINT (123.00000, 11.00000) "Waypoint name" "Feb-02-2020 10:00"
    /// </summary>
    public class Waypoint
    {
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {

            return Name;
        }

        public string SerializeString()
        {
            return $"POINT ({X.ToString()} {Y.ToString()}) \"{Name}\" \"{TimeStamp.ToString("MMM-dd-yyyy HH:mm")}\"";
        }

        public Waypoint(double x, double y, string name, DateTime timeStamp)
        {
            X = x;
            Y = y;
            Name = name;
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Validate entities using expected values and rules based on the context of control name
        /// </summary>
        /// <param name="formValues">dictionary of variable name of control's content and control text</param>
        /// <param name="errorMessages"></param>
        /// <returns>boolean </returns>
        public static bool EntityValidated(Dictionary<string, string> formValues, out List<EntityValidationMessage> errorMessages)
        {
            errorMessages = new List<EntityValidationMessage>();

            string stringLongitude = formValues["txtLongitude"];
            string stringLatitude = formValues["txtLatitude"];
            string stringWaypointName = formValues["txtWaypoint_name"];
            string stringDateAcquired = formValues["dtpDate_acquired"];

            if(stringWaypointName.Length==0)
            {
                errorMessages.Add( new EntityValidationMessage("Waypoint name cannot be empty"));
            }

            if(stringDateAcquired.Length==0)
            {
                errorMessages.Add(new EntityValidationMessage("Date acquired cannot be empty"));
            }
            else
            {
                if(DateTime.TryParse(stringDateAcquired, out DateTime v))
                {
                    if(v>DateTime.Now)
                    {
                        errorMessages.Add(new EntityValidationMessage("Date acquired cannot be a future date"));
                    }
                }
                else
                {
                    errorMessages.Add(new EntityValidationMessage("Date acquired must be a correct date"));
                }
            }

            if (stringLongitude.Length > 0)
            {
                if (double.TryParse(stringLongitude, out double v))
                {
                    //reserved for future use
                }
                else
                {
                    errorMessages.Add(new EntityValidationMessage("Longitude must be a numeric value"));
                }
            }
            else
            {
                errorMessages.Add(new EntityValidationMessage("Longitude cannot be empty"));
            }

            if (stringLatitude.Length > 0)
            {
                if (double.TryParse(stringLatitude, out double v))
                {
                    //reserved for future use
                }
                else
                {
                    errorMessages.Add(new EntityValidationMessage("Latitude must be a numeric value"));
                }
            }
            else
            {
                errorMessages.Add(new EntityValidationMessage("Latitude cannot be empty"));
            }

            return errorMessages.Count == 0;
        }

        public static bool EntityValidated(Waypoint waypoint, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            if (waypoint.TimeStamp > DateTime.Now)
                errorMessages.Add("Waypoint timestamp cannot be a future date");

            if (waypoint.X < 118 || waypoint.X > 126)
                errorMessages.Add("Longitude is out of bounds");

            if (waypoint.Y < 4 || waypoint.Y > 20)
                errorMessages.Add("Latitude is out of bounnds");

            if (waypoint.Name.Length == 0)
                errorMessages.Add("Name cannot be empty");

            return errorMessages.Count == 0;
        }

        public Waypoint()
        {

        }
        public Waypoint(string wptText)
        {
            if (wptText.Length > 0)
            {
                using (var strm = wptText.ToStream())
                using (var parser = new NotVisualBasic.FileIO.CsvTextFieldParser(strm))
                {
                    parser.SetDelimiter(' ');
                    var csvLine = parser.ReadFields();
                    X = double.Parse((csvLine[1].Trim('(', ')')));
                    Y = double.Parse((csvLine[2].Trim('(', ')')));
                    Name = csvLine[3];
                    TimeStamp = DateTime.Parse(csvLine[4]);
                }
            }
        }
    }
}
