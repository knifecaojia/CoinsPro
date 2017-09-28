using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
namespace MinerDOG
{
    public partial class rmp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string res = "";
            string sid =System.Web.HttpUtility.UrlDecode(Request["sid"]);
            string hum = System.Web.HttpUtility.UrlDecode(Request["hum"]);
            string tmp = System.Web.HttpUtility.UrlDecode(Request["tmp"]);

            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), CommandType.Text, "insert into th(timestamp,sid,tmp,hum) values('" + DateTime.Now.ToString() + "','" + sid + "','" + tmp + "','" + hum + "')");
                res = "success";
            }
            catch (Exception err)
            {
                res = err.Message;
                Response.Write(res);
            }
            Response.Write(sid + tmp + hum);
            
        }
    }
}