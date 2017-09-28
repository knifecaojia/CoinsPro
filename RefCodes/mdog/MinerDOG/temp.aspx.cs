using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace MinerDOG
{
    public partial class temp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ip = "";
            double max = 0;
            double min = 1000;
            if (Request["ip"] == null)
                return;
            DateTime start = DateTime.Now.AddDays(-1);
            ip = Request["ip"].ToString();
            DataTable Miners = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from miner_remote_Log where IPaddress='"+ip+"' and TimeStamp>'"+start.ToString()+"' order by id asc").Tables[0];
            string []ctempstr=new string[Miners.Rows.Count];
            double[] ctemp1 = new double[Miners.Rows.Count];


            double[] ctemp2 = new double[Miners.Rows.Count];

  
            double[] ctemp3 = new double[Miners.Rows.Count];
            for (int i = 0; i < Miners.Rows.Count; i++)
            {
                
                ctempstr[i] = Miners.Rows[i]["TimeStamp"].ToString();
                ctemp1[i] = Convert.ToDouble(Miners.Rows[i]["ctep1"]);
                
                ctemp2[i] = Convert.ToDouble(Miners.Rows[i]["ctep2"]);
                ctemp3[i] = Convert.ToDouble(Miners.Rows[i]["ctep3"]);
                //Miners_remote.Rows[i]["IPaddress"] = "<a href='temp.aspx?ip='>" + Miners_remote.Rows[i]["IPaddress"].ToString()+"</a>";
            }
            double max1 = ctemp1.Max();
            double max2 = ctemp2.Max();
            double max3 = ctemp3.Max();
            max = Math.Max(max1, max2);
            max = Math.Max(max, max3);
            double min1 = ctemp1.Min();
            double min2 = ctemp2.Min();
            double min3 = ctemp3.Min();
            min = Math.Min(min1, min2);
            min = Math.Min(min, min3);
            Chart1.ChartAreas[0].AxisY.Maximum = max + 2;
            Chart1.ChartAreas[0].AxisY.Minimum = min - 2;
            Chart1.Series[0].Points.DataBindXY(ctempstr, ctemp1);
            Chart1.Series[1].Points.DataBindXY(ctempstr, ctemp2);
            Chart1.Series[2].Points.DataBindXY(ctempstr, ctemp3);
        }
    }
}