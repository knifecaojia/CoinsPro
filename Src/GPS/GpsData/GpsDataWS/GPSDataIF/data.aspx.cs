using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace GPSDataIF
{
    public partial class data : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"];
            if ((!string.IsNullOrEmpty(action)) && action == "group")
            {
                DataTable Group = GetDataTable("select * from lde_category where category_regioncode='2'");
                string str = JsonConvert.SerializeObject(Group);
                Response.Write(str);
            }
            if ((!string.IsNullOrEmpty(action)) && action == "TER")
            {
                DataTable Group = GetDataTable("select * from lde_device");
                string str = JsonConvert.SerializeObject(Group);
                Response.Write(str);
            }
            if ((!string.IsNullOrEmpty(action)) && action == "TRA")
            {
                string keyid = Request["keyid"];
                //int i = Convert.ToInt32(Request["i"]);
                string start = Request["s"];
                string end = Request["e"];
                
                DateTime starttime = Convert.ToDateTime(DateTime.ParseExact(start, "yyyyMMdd",
               CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                DateTime endtime = Convert.ToDateTime(DateTime.ParseExact(end, "yyyyMMdd",
               CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                int days = Convert.ToInt32((endtime - starttime).TotalDays)+1;
                string str = "";
                //gps_coords_20160401
                DataTable all = null;
                for (int i = 0; i < days; i++)
                {
                    starttime=starttime.AddDays(i);
                    string tablename = "gps_coords_"+starttime.Year+starttime.ToString("MM")+ starttime.ToString("dd") ;
                   
                    DataTable Group = GetDataTable("select location_x,location_y,location_direction,location_speed,location_time from "+ tablename + " where device_id = '" + keyid + "'");

                    if (Group.Rows.Count > 0)
                    {
                        if (all == null)
                            all = Group.Clone();
                        foreach (DataRow dr in Group.Rows)
                        {
                           
                                all.Rows.Add(dr.ItemArray);
                        }

                    }
                }


                str = JsonConvert.SerializeObject(all);
                str = str.Replace("location_x", "x");
                str = str.Replace("location_y", "y");
                str = str.Replace("location_direction", "d");
                str = str.Replace("location_speed", "s");
                str = str.Replace("location_time", "t");
                Response.Write(str);
            }
        }
        DataTable GetDataTable(string str)
        {
            var connString = "Host=10.18.20.60;Username=gpsws;Password=1235gpsws1235;Database=lde_sysdb";
           

            try
            {
                string data;
                string schema;
                GetSchema(connString, str, out data, out schema);
                DataSet ds = new DataSet();
                ds.ReadXml(new XmlTextReader(new StringReader(data)));
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
          
           
   
        }
        private void GetSchema(string connectionString, string query, out string data, out string schema)
        {
            using (var conn = new Npgsql.NpgsqlConnection(connectionString))
            using (var da = new Npgsql.NpgsqlDataAdapter(query, conn))
            using (var ds = new DataSet())
            using (var dataStream = new MemoryStream())
            using (var schemaStream = new MemoryStream())
            {
                conn.Open();
                da.Fill(ds);
                ds.WriteXml(dataStream);
                ds.WriteXmlSchema(schemaStream);
                dataStream.Position = 0;
                schemaStream.Position = 0;
                using (var dataReader = new StreamReader(dataStream))
                using (var schemaReader = new StreamReader(schemaStream))
                {
                    data = dataReader.ReadToEnd();
                    schema = schemaReader.ReadToEnd();
                }
            }
        }
    }
}