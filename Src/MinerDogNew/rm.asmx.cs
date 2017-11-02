using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace MinerDOG
{
    /// <summary>
    /// rm 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class rm : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string upload(string json)
        {
            try
            {

                DataTable Miners = JsonConvert.DeserializeObject<DataTable>(json);
               
                for (int j = 0; j < Miners.Rows.Count; j++)
                {
                    string updatestr = "UPDATE miner set ";
                    for (int i = 1; i < Miners.Columns.Count; i++)
                    {
                        updatestr += Miners.Columns[i].ColumnName + "='"+ Miners.Rows[j][i].ToString() +"',";
                    }
                    updatestr=updatestr.Substring(0,updatestr.Length-1);
                    updatestr += " where ID=" + Miners.Rows[j]["ID"].ToString();
                    SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, updatestr);
                }
                
                return "success";
            }
            catch(Exception err)
            {
                return "failed" + "    " + err;
            }
        }
        [WebMethod]
        public string upload_remote(string json)
        {
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

                return res+ "success";
            }
            catch (Exception err)
            {
               // SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into miner_remote(IPaddress) values('"++"')");
                return res+"failed" + "    " + err;
            }
        }
        [WebMethod]
        public string Updatetmpandhum(string sid, string tmp, string hum)
        {
            string res = "";
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into th(timestamp,sid,tmp,hum) values('" + DateTime.Now.ToString() + "','" + sid + "','" + tmp + "','" + hum + "')");
                res = "success";
            }
            catch(Exception e)
            {
                res = e.Message;
            }
            return res;
            
        }
    }
}
