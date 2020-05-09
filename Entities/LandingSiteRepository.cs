using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using BSCTracker;
using BSCTracker.Utilities;

namespace BSCTracker.Entities
{
    public class LandingSiteRepository

    {
        public List<LandingSite> landingSites { get; set; }

        public LandingSiteRepository()
        {
            landingSites = getLandingSites();
        }

        private List<LandingSite> getLandingSites()
        {
            List<LandingSite> listLandingSites = new List<LandingSite>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from LandingSite";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listLandingSites.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            LandingSite ls = new LandingSite();
                            ls.LandingSiteID = dr["LandingSiteId"].ToString();
                            ls.LandingSiteName = dr["LandingSiteName"].ToString();
                            if (dr["Municipality"].ToString().Length >0)
                            {
                                ls.Municipality = BSCEntities.MunicipalityViewModel.GetMunicipality(Convert.ToInt32(dr["Municipality"]));
                            }
                            ls.Waypoint = new Waypoint(dr["Waypoint"].ToString());
                            listLandingSites.Add(ls);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    
                }
            }

            return listLandingSites;
        }

        public bool Add(LandingSite ls)
        {
            string wpt = "";
            if(ls.Waypoint!=null)
            {
                wpt = ls.Waypoint.SerializeString();
            }

            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into LandingSite (LandingSiteID, LandingSiteName,Municipality,Waypoint)
                           Values 
                           ({{{ls.LandingSiteID}}},'{ls.LandingSiteName}',{ls.Municipality.MunicipalityID},'{wpt}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(LandingSite ls)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update LandingSite set
                                LandingSiteName = '{ls.LandingSiteName}',
                                Municipality = '{ls.Municipality.MunicipalityID}',
                                Waypoint = '{ls.Waypoint.SerializeString()}'
                            WHERE LandingSiteId={{{ls.LandingSiteID}}}";
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
                var sql = $"Delete * from LandingSite where LandingSiteId={{{id}}}";
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
