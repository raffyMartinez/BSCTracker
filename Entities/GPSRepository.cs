using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using BSCTracker.Utilities;

namespace BSCTracker.Entities
{
    public class GPSRepository
    {
        public List<GPS> GPSes { get; set; }

        public GPSRepository()
        {
            GPSes = getGPSes();
        }


        private List<GPS> getGPSes()
        {
            List<GPS> listGPSes = new List<GPS>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from GPS";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listGPSes.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            GPS g = new GPS();
                            g.ID = dr["GPSId"].ToString();
                            g.Brand = dr["Brand"].ToString();
                            g.Model = dr["Model"].ToString();
                            g.AssignedName = dr["AssignedName"].ToString();
                            if (dr["DateAcquired"].ToString().Length > 0)
                            {
                                g.DateAcquired = (DateTime)dr["DateAcquired"];
                            }
                            g.SDCardCapacity = dr["SDCardCapacity"].ToString();
                            listGPSes.Add(g);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }

            return listGPSes;
        }
        public bool Add(GPS g)
        {
            string dateAcquired = "null";
            if (g.DateAcquired != null)
            {
                dateAcquired = $"'{g.DateAcquired.ToString()}'";
            }
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into GPS (GPSId, Model,DateAcquired,SDCardCapacity,Brand,AssignedName)
                           Values 
                           ({{{g.ID}}},'{g.Model}', {dateAcquired},
                            '{g.SDCardCapacity}','{g.Brand}','{g.AssignedName}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(GPS g)
        {
            string dateAcquired = "null";
            if (g.DateAcquired != null)
            {
                dateAcquired = $"'{g.DateAcquired.ToString()}'";
            }
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update GPS set
                                Model = '{g.Model}',
                                Brand = '{g.Brand}',
                                SDCardCapacity='{g.SDCardCapacity}',
                                DateAcquired = {dateAcquired},
                                AssignedName = '{g.AssignedName}'
                            WHERE GPSId={{{g.ID}}}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(string id)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from GPS where GPSId={{{id}}}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    try
                    {
                        success = update.ExecuteNonQuery() > 0;
                    }
                    catch (OleDbException)
                    {
                        success = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        success = false;
                    }
                }
            }
            return success;
        }
    }
}
