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
    public class SamplingRepository
    {
        public List<Sampling> Samplings { get; set; }

        public SamplingRepository()
        {
            Samplings = getSamplings();
        }

        private List<Sampling> getSamplings()
        {
            
            if (BSCEntities.ProjectSettingViewModel == null)
            {
                BSCEntities.ProjectSettingViewModel = new ProjectSettingViewModel();
            }
            List<Sampling> listSamplings = new List<Sampling>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from Sampling";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listSamplings.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sampling s = new Sampling
                            {
                                FisherGPS = BSCEntities.FisherGPSViewModel.GetFisherGPS(dr["FisherGPSID"].ToString()),
                                RowID = dr["RowID"].ToString(),
                                DateTimeDeparted = (DateTime)dr["DateTimeDeparted"],
                                DateTimeArrived = (DateTime)dr["DateTimeArrived"],
                                DateTimeSampled = (DateTime)dr["DateTimeSampled"],
                                NSAPSamplingID = dr["NSAPSamplingID"].ToString(),
                                Gear = BSCEntities.GearViewModel.GetGear(dr["GearID"].ToString()),
                                ProjectSetting = BSCEntities.ProjectSettingViewModel.GetProjectSetting(dr["Project"].ToString()),
                                LandingSite = BSCEntities.LandingSiteViewModel.GetLandingSite(dr["LandingSite"].ToString()),
                                DateAdded=(DateTime)dr["DateAdded"]
                            };

                            listSamplings.Add(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }

            return listSamplings;
        }
        public bool Add(Sampling s)
        {

            bool success = false;
            s.DateAdded = DateTime.Now;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into Sampling (FisherGPSID, RowID, DateTimeDeparted, DateTimeArrived,
                                NSAPSamplingID, GearID, Project, DateTimeSampled, LandingSite, DateAdded)
                           Values 
                           ({{{s.FisherGPS.RowID}}},{{{s.RowID}}},'{s.DateTimeDeparted}', '{s.DateTimeArrived}',
                               '{s.NSAPSamplingID}',{{{s.Gear.GearID}}}, {{{s.ProjectSetting.ProjectID}}},
                               '{s.DateTimeSampled}', {{{s.LandingSite.LandingSiteID}}},'{s.DateAdded}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(Sampling s)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update Sampling set
                                FisherGPSID = {{{s.FisherGPS.RowID}}},
                                DateTimeDeparted = '{s.DateTimeDeparted}',
                                DateTimeArrived='{s.DateTimeArrived}',
                                NSAPSamplingID = '{s.NSAPSamplingID}',
                                Project = {{{s.ProjectSetting.ProjectID}}},
                                GearID = {{{s.Gear.GearID}}},
                                LandingSite = {{{s.LandingSite.LandingSiteID}}},
                                DateTimeSampled = '{s.DateTimeSampled}'
                            WHERE RowID={{{s.RowID}}}";
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
                var sql = $"Delete * from Sampling where RowID={{{id}}}";
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
