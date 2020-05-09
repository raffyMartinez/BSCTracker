using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace BSCTracker.Utilities
{
    public static class Global
    {
        public static string MDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}/bsc_tracking.mdb";
        public static bool AppProceed { get; private set; }

        public static string ConnectionString { get; private set; }
        static Global()
        {

            AppProceed = File.Exists(MDBPath);
            if (AppProceed)
            {
                ConnectionString = "Provider=Microsoft.JET.OLEDB.4.0;data source=" + MDBPath;
            }

        }

        public static bool ParsedCoordinateIsValid(string coordToParse, string x_or_y, out double result)
        {
            result = 0;
            bool success = false;
            switch (x_or_y)
            {
                case "x":
                case "X":
                case "y":
                case "Y":
                    switch (x_or_y)
                    {
                        case "x":
                        case "X":
                            if (double.TryParse(coordToParse, out double v))
                            {
                                if (v >= 0 && v <= 180)
                                {
                                    result = v;
                                    success = true;
                                }
                            }
                            break;
                        case "y":
                        case "Y":
                            if (double.TryParse(coordToParse, out v))
                            {
                                if (v >= -90 && v <= 90)
                                {
                                    result = v;
                                    success = true;
                                }
                            }
                            break;
                    }
                    break;
                default:
                    throw new Exception("Error: expected value must either be x,X,y,Y");

            }


            return success;
        }
        public static bool ParsedDateIsValid(string dateToParse, out DateTime result)
        {
            bool success = false;
            result = DateTime.Now;
            if (DateTime.TryParse(dateToParse, out DateTime inDate))
            {
                if (inDate <= DateTime.Now)
                {
                    result = inDate;
                    success = true;
                }
            }
            return success;
        }
        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}
