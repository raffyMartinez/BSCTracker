using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using BSCTracker.Entities;
using BSCTracker.Utilities;

namespace BSCTracker.Entities
{
    class FisherGPSRepository
    {

        public List<FisherGPS> FisherGPSes { get; set; }


        public FisherGPSRepository()
        {
            FisherGPSes = getFisherGPSes();
        }
        private List<FisherGPS> getFisherGPSes()
        {
            string query;
            List<FisherGPS> listFisherGPSes = new List<FisherGPS>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))

            {
                try
                {
                    conection.Open();

                    query = $@"SELECT * from fisher_gps";
                    

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listFisherGPSes.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            FisherGPS fg = new FisherGPS();
                            fg.RowID = dr["RowID"].ToString();
                            fg.GPS = BSCEntities.GPSViewModel.GetGPS(dr["GPS"].ToString());
                            fg.ProjectSetting = BSCEntities.ProjectSettingViewModel.GetProjectSetting(dr["Project"].ToString());
                            fg.Fisher = BSCEntities.FisherViewModel.GetFisher(dr["Fisher"].ToString());
                            fg.DateAssigned = (DateTime)dr["DateAssigned"];
                            if (dr["DateReturned"].ToString().Length > 0)
                            {
                                fg.DateReturned = (DateTime)dr["DateReturned"];
                            }
                            listFisherGPSes.Add(fg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }

            return listFisherGPSes;
        }




        public bool Add(FisherGPS fg)
        {
            string dateAssigned = "null";
            if (fg.DateAssigned != null)
            {
                dateAssigned = $"'{((DateTime)fg.DateAssigned).ToString()}'";
            }
            string dateReturned = "null";
            if (fg.DateReturned != null)
            {
                dateReturned = $"'{((DateTime)fg.DateReturned).ToString()}'";
            }
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into fisher_gps (GPS, RowID, Project, DateAssigned,DateReturned,Fisher)
                           Values 
                           ({{{fg.GPS.ID}}},{{{fg.RowID}}},{{{fg.ProjectSetting.ProjectID}}},{dateAssigned},{dateReturned}, {{{fg.Fisher.FisherID}}})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }


        public bool Update(FisherGPS fg)
        {
            bool success = false;
            string dateAssigned = "null";
            if (fg.DateAssigned != null)
            {
                dateAssigned = $"'{((DateTime)fg.DateAssigned).ToString()}'";
            }
            string dateReturned = "null";
            if (fg.DateReturned != null)
            {
                dateReturned = $"'{fg.DateReturned.ToString()}'";
            }
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update fisher_gps set
                                GPS = {{{fg.GPS.ID}}},
                                Project = {{{fg.ProjectSetting.ProjectID}}},
                                DateAssigned ={dateAssigned},
                                DateReturned= {dateReturned},
                                Fisher={{{fg.Fisher.FisherID}}}
                            WHERE RowID={{{fg.RowID}}}";
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
                var sql = $"Delete * from fisher_gps where RowID={{{id}}}";
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
