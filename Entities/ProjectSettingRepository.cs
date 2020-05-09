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
    public class ProjectSettingRepository
    {

        public List<ProjectSetting> ProjectSettings { get; set; }

        public ProjectSettingRepository()
        {
            ProjectSettings = getProjectSettings();
        }

        private List<ProjectSetting> getProjectSettings()
        {
            List<ProjectSetting> listProjectSettings = new List<ProjectSetting>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $@"SELECT * from ProjectSettings";

                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listProjectSettings.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            ProjectSetting ps = new ProjectSetting
                            {
                                ProjectName = dr["ProjectName"].ToString(),
                                DateStart = (DateTime)(dr["DateStart"]),
                                ProjectID = dr["ProjectID"].ToString()
                            };

                            listProjectSettings.Add(ps);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
            }   

            return listProjectSettings;
        }

        public bool Add(ProjectSetting ps)
        {

            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into ProjectSettings (ProjectID, ProjectName, DateStart)
                           Values 
                           ({{{ps.ProjectID}}},'{ps.ProjectName}','{ps.DateStart}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(ProjectSetting ps)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update ProjectSettings set
                                ProjectName = '{ps.ProjectName}',
                                DateStart = '{ps.DateStart}'
                            WHERE ProjectID={{{ps.ProjectID}}}";
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
                var sql = $"Delete * from ProjectSettings where ProjectID={{{id}}}";
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
