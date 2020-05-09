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
    public class GearRepository
    {
        public List<Gear> Gears{ get; set; }

        public GearRepository()
        {
            Gears = getGears();
        }

        private List<Gear>getGears()
        {
            List<Gear> listGears = new List<Gear>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from Gear";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listGears.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Gear g = new Gear();
                            g.GearID = dr["GearID"].ToString();
                            g.GearName = dr["GearName"].ToString();
                            g.Code = dr["Code"].ToString();    
                            

                            listGears.Add(g);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return listGears;
            }
        }

        public bool Add(Gear g)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into Gear (GearName,Code,GearID)
                           Values 
                           ('{g.GearName}','{g.Code}',{{{g.GearID}}})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(Gear g)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update Gear set
                                GearName = '{g.GearName}',
                                Code = '{g.Code}'
                            WHERE GearID = {{{g.GearID}}}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(string gearName)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from Gear where GearName='{gearName}'";
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
