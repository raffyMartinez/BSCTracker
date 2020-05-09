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
    public class MunicipalityRepository
    {
        public List<Municipality> Municipalities{ get; set; }

        public MunicipalityRepository()
        {
            Municipalities = getMunicipalities();
        }

        private List<Municipality> getMunicipalities()
        {
            List<Municipality> listMunicipalities = new List<Municipality>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from Municipalities";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listMunicipalities.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Municipality m = new Municipality();
                            m.Province = BSCEntities.ProvinceViewModel.GetProvince(Convert.ToInt32( dr["ProvNo"]));
                            m.MunicipalityID = (int)dr["MunNo"];
                            m.MunicipalityName = dr["MunicipalityName"].ToString();
                            m.Latitude = (Double)dr["yCoord"];
                            m.Longitude = (double)dr["xCoord"];
                            m.IsCoastal = (bool)dr["IsCoastal"];
                            listMunicipalities.Add(m);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return listMunicipalities;
            }
        }

        public bool Add(Municipality m)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into Municipalities(ProvNo, MunNo, MunicipalityName, xCoord, yCoord, IsCoastal)
                           Values
                           ({m.Province.ProvinceID}, {m.MunicipalityID}, '{m.MunicipalityName}', {m.Longitude}, {m.Latitude}, {m.IsCoastal})";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(Municipality m)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update Municipalities set
                                ProvNo = {m.Province.ProvinceID},
                                MunicipalityName='{m.MunicipalityName}',
                                xCoord = {m.Longitude},
                                yCoord = {m.Latitude},
                                IsCoastal = {m.IsCoastal}
                            WHERE MunNo = {m.MunicipalityID}";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Delete(int ID)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $"Delete * from Municipalities where MunNo={ID}";
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
