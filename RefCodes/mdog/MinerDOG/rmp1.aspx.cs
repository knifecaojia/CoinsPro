using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MinerDOG
{
    public partial class rmp1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string json = System.Web.HttpUtility.UrlDecode(Request["json"]);
            string res = "";
            try
            {

                DataTable Miners = JsonConvert.DeserializeObject<DataTable>(json);


                for (int j = 0; j < Miners.Rows.Count; j++)
                {
                    string updatestr = "UPDATE miner_remote set ";
                    for (int i = 1; i < Miners.Columns.Count; i++)
                    {
                        updatestr += Miners.Columns[i].ColumnName + "='" + Miners.Rows[j][i].ToString() + "',";
                    }
                    string insert = "insert into miner_remote_Log(TimeStamp,IPaddress,Elapsed,Gper5s,Gavg,ctep1,ctep2,ctep3,fspd1,fspd2,freq,HW,HWP) values('" + Miners.Rows[j]["TimeStamp"].ToString() + "','" + Miners.Rows[j]["IPaddress"].ToString() + "','" + Miners.Rows[j]["Elapsed"].ToString() + "','" + Miners.Rows[j]["Gper5s"].ToString() + "','" + Miners.Rows[j]["Gavg"].ToString() + "','" + Miners.Rows[j]["ctep1"].ToString() + "','" + Miners.Rows[j]["ctep2"].ToString() + "','" + Miners.Rows[j]["ctep3"].ToString() + "','" + Miners.Rows[j]["fspd1"].ToString() + "','" + Miners.Rows[j]["fspd2"].ToString() + "','" + Miners.Rows[j]["freq"].ToString() + "','" + Miners.Rows[j]["HW"].ToString() + "','" + Miners.Rows[j]["HWP"].ToString() + "')";
                    updatestr = updatestr.Substring(0, updatestr.Length - 1);
                    updatestr += " where IPaddress='" + Miners.Rows[j]["IPaddress"].ToString() + "'";
                    try
                    {

                        int i = SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, updatestr);
                        if (Miners.Rows[j]["StatusType"].ToString() == "Success")
                            SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, insert);
                        if (i == 0)
                        {
                            SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into miner_remote(IPaddress) values('" + Miners.Rows[j]["IPaddress"].ToString() + "')");
                            res += "insert into table ipaddress=" + Miners.Rows[j]["IPaddress"].ToString();
                        }

                    }
                    catch (Exception updateerr)
                    {
                        SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into miner_remote(IPaddress) values('" + Miners.Rows[j]["IPaddress"].ToString() + "')");

                        res += updateerr;
                    }
                }

                Response.Write(res + "success");
            }
            catch (Exception err)
            {
                // SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into miner_remote(IPaddress) values('"++"')");
                Response.Write(res + "failed" + "    " + err);
            }
        }
    }
}