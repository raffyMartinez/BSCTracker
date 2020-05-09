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
   public  class FisherRepository
    {
        public List<Fisher> Fishers { get; set; }

        public FisherRepository()
        {
            Fishers = getFishers();
        }

        private List<Fisher> getFishers()
        {
            List<Fisher> listFishers = new List<Fisher>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from Fisher";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listFishers.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Fisher f = new Fisher();
                            f.FisherID= dr["FisherId"].ToString();
                            f.FisherName = dr["FisherName"].ToString();
                            f.FishingBoatName = dr["FishingBoatName"].ToString();
                            var ls = BSCEntities.LandingSiteViewModel.GetLandingSite(dr["LandingSite"].ToString());
                            if (ls != null)
                            {
                                f.LandingSite = ls;
                            }
                            listFishers.Add(f);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }

            return listFishers;
        }

        public bool Add(Fisher f)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into Fisher (FisherName, LandingSite,FisherId, FishingBoatName)
                           Values 
                           ('{f.FisherName}',{{{f.LandingSite.LandingSiteID}}},{{{f.FisherID}}}, '{f.FishingBoatName}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(Fisher f)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update Fisher set
                                FisherName = '{f.FisherName}',
                                LandingSite = {{{f.LandingSite.LandingSiteID}}},
                                FishingBoatName = '{f.FishingBoatName}'
                            WHERE FisherId={{{f.FisherID}}}";
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
                var sql = $"Delete * from Fisher where FisherId={{{id}}}";
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
