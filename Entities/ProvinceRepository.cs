﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using BSCTracker.Utilities;

namespace BSCTracker.Entities
{
    class ProvinceRepository
    {
        public List<Province> Provinces {get; set; }

        public ProvinceRepository()
        {
            Provinces = getProvinces();
        }

        private List<Province> getProvinces()
        {
            List<Province> listProvinces = new List<Province>();
            var dt = new DataTable();
            using (var conection = new OleDbConnection(Global.ConnectionString))
            {
                try
                {
                    conection.Open();
                    string query = $"Select * from Provinces order by ProvinceName";


                    var adapter = new OleDbDataAdapter(query, conection);
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        listProvinces.Clear();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Province p = new Province();
                            p.ProvinceID = Convert.ToInt32( dr["ProvNo"]);
                            p.ProvinceName = dr["ProvinceName"].ToString();

                            listProvinces.Add(p);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);

                }
                return listProvinces;
            }
        }

        public bool Add(Province p)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Insert into Provinces (ProvNo, ProvinceName)
                           Values 
                           ({p.ProvinceID}, '{p.ProvinceName}')";
                using (OleDbCommand update = new OleDbCommand(sql, conn))
                {
                    success = update.ExecuteNonQuery() > 0;
                }
            }
            return success;
        }

        public bool Update(Province  p)
        {
            bool success = false;
            using (OleDbConnection conn = new OleDbConnection(Global.ConnectionString))
            {
                conn.Open();
                var sql = $@"Update Provinces set
                                ProvinceName = '{p.ProvinceName}'
                            WHERE ProvNo = {p.ProvinceID}";
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
                var sql = $"Delete * from Provinces where ProvNo={ID}";
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
