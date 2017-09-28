using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.IO;

namespace MinerDOG
{
    public partial class index : System.Web.UI.Page
    {
        string sss = "";
        string kurl = @"https://pool.viabtc.com/user/api/5090f3564b29b7df3fc74405e7d47575/";
        protected void Page_Load(object sender, EventArgs e)
        {

            
            if (!this.IsPostBack)
            {
                DataTable Miners = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from miner").Tables[0];
                try
                {

                    for (int i = 0; i < Miners.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(Miners.Rows[i]["IsR"]) == 0)
                        {
                            try
                            {
                                if (!TestNetConnected(Miners.Rows[i]["IPaddress"].ToString(), 1, 3))
                                {
                                    Miners.Rows[i]["StatusType"] = "Timeout";
                                    // Console.WriteLine("Mi:{0},STA:{1} ELP:{1},G/5s:{2},G/AVG:{3},Ctep1:{4},Ctep2:{5},Ctep3:{6}，FAN1:{7},FAN2:{8}", minerarr[i].IPaddress, "-", "-", "-", "-", "-", "-", "-", minerarr[i].StatusType);
                                    continue;
                                }
                                else
                                    Miners.Rows[i]["StatusType"] = "Success";
                                Miners.Rows[i]["TimeStamp"] = DateTime.Now.ToString();
                                List<string> l = cgminer_api_stats_list(Miners.Rows[i]["IPaddress"].ToString());
                                Miners.Rows[i]["ctep1"] = l[10];
                                Miners.Rows[i]["ctep2"] = l[11];
                                Miners.Rows[i]["ctep3"] = l[12];
                                Miners.Rows[i]["fspd1"] = l[13];
                                Miners.Rows[i]["fspd2"] = l[14];
                                Miners.Rows[i]["freq"] = l[4];
                                Miners.Rows[i]["Elapsed"] = l[5];
                                Miners.Rows[i]["Gper5s"] = l[6];
                                Miners.Rows[i]["Gavg"] = l[7];
                                Miners.Rows[i]["HW"] = l[8];
                                Miners.Rows[i]["HWP"] = l[9];
                                Miners.Rows[i]["Mtype"] = l[2];
                            }
                            catch
                            { }
                        }
                        else
                        {
                            Miners.Rows[i]["Gper5s"] = Miners.Rows[i]["Gper5s"].ToString().Replace("\"", "");
                            Miners.Rows[i]["Gavg"] = Miners.Rows[i]["Gavg"].ToString().Replace("\"", "");
                        }
                    }
                }
                catch (Exception err)
                {
                    Response.Write(err.Message);
                }
                SqlCommand sqlcmd = new SqlCommand("select * from miner", SqlHelper.GetConnection());
                SqlDataAdapter sda = new SqlDataAdapter(sqlcmd);
                SqlCommandBuilder sqlcmdB = new SqlCommandBuilder(sda);
                sda.Update(Miners);

                GridView1.DataSource = Miners.DefaultView;
                GridView1.DataBind();
                
            }
            try
            {

                DataTable Miners_remote = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from miner_remote").Tables[0];
                DataColumn dc0 = new DataColumn("NO", typeof(string));

                Miners_remote.Columns.Add(dc0);
                Miners_remote.Columns["NO"].SetOrdinal(0);
                GridView2.DataSource = Miners_remote.DefaultView;
                GridView2.DataBind();
             
            }
            catch
            {
 
            }
            //GridView1.Columns[3].Visible = false;
            //GridView1.Columns[5].Visible = false;
            //GridView1.Columns[6].Visible = false;
          
            string lb="";
           
            DataTable sids=SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "SELECT DISTINCT sid  FROM th").Tables[0];
            try
            {
                
                for (int i = 0; i < sids.Rows.Count; i++)
                {
                    string s = sids.Rows[i][0].ToString();
                    DataTable d = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "SELECT  top 1 *   FROM th where sid='" + s + "' order by id desc").Tables[0];
                    DataTable dd = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "SELECT  top 300 *   FROM th where sid='" + s + "' order by id desc").Tables[0];
                    double tmp = Convert.ToDouble(d.Rows[0]["tmp"]) / 100;
                    double hum = Convert.ToDouble(d.Rows[0]["hum"]) / 100;
                    DateTime time = Convert.ToDateTime(d.Rows[0]["timestamp"]);
                    string t = "";
                    string x = "[";
                    string y1 = "[";
                    string y2 = "[";
                    DataView dv = dd.DefaultView;
                    dv.Sort = "id asc";
                    dd = dv.ToTable();
                    for (int ii = 0; ii < dd.Rows.Count; ii++)
                    {


                        if (ii < dd.Rows.Count - 1)
                        {
                            x = x + "'" + dd.Rows[ii]["TimeStamp"].ToString() + "',";
                            y1 = y1 + Convert.ToDouble(dd.Rows[ii]["tmp"]) / 100 + ",";
                            y2 = y2 + Convert.ToDouble(dd.Rows[ii]["hum"]) / 100 + ",";
                            
                        }
                        else
                        {
                            x = x + "'" + dd.Rows[ii]["TimeStamp"].ToString() + "']";
                            y1 = y1 + Convert.ToDouble(dd.Rows[ii]["tmp"]) / 100 + "]";
                            y2 = y2 + Convert.ToDouble(dd.Rows[ii]["hum"]) / 100 + "]";
                            
                        }
                     
                        //Miners_remote.Rows[i]["IPaddress"] = "<a href='temp.aspx?ip='>" + Miners_remote.Rows[i]["IPaddress"].ToString()+"</a>";
                    }
                    string title = "";
                    if (s == "158d000149ad16")
                    {
                        t = "设备架2(" + time .ToShortTimeString()+ ")";
                        title = "设备架2";
                    }
                    if (s == "158d000149db66")
                    {
                        t = "稳压器后(" + time.ToShortTimeString() + ")";
                        title = "稳压器后";
                    }
                    if (s == "158d00014a0210")
                    {
                        t = "空调出风口(" + time.ToShortTimeString() + ")";
                        title = "空调出风口";
                    }
                    if (s == "158d00015ae989")
                    {
                        t = "设备架1(" + time.ToShortTimeString() + ")";
                        title = "设备架1";
                    }
                    lb += t + " 温度:" + tmp.ToString("f2") + " 湿度:" + hum.ToString("f2") + "% &nbsp;";
                    sss += "chart1(" + x + "," + y1 + "," + y2 + "," + (i + 1) + ",\"" + title + "\");";
                }
            }
            catch(Exception er)
            {
                lb = er.Message;
            }
            Label1.Text = lb;
            if (!IsPostBack)
            {
                string btccom = GetWebRequest(@"https://btc.com/");

                //var doc = new HtmlDocument();
                //doc.LoadHtml(btccom);
                string url = "";
                string url1 = "";
                string url2 = "";
                string url3 = "";
                string url4 = "";
                 //url = doc.DocumentNode.SelectNodes("/html/body/div/div/div[2]/div[2]/div/div[2]/ul[1]/li[1]/dl/dd").First().InnerText;
                 //url1 = doc.DocumentNode.SelectNodes("/html/body/div/div/div[2]/div[2]/div/div[2]/ul[1]/li[2]/dl/dd/text()").First().InnerText;
                 //url2 = doc.DocumentNode.SelectNodes("/html/body/div/div/div[2]/div[2]/div/div[2]/ul[1]/li[3]/dl/dd").First().InnerText;
                 //url3 = doc.DocumentNode.SelectNodes("/html/body/div/div/div[2]/div[2]/div/div[2]/ul[2]/li[1]/dl/dd").First().InnerText;
                 //url4 = doc.DocumentNode.SelectNodes("/html/body/div/div/div[2]/div[2]/div/div[2]/ul[2]/li[2]/dl/dd").First().InnerText;
                url = Regex.Replace(url, "\\s{2,}", ",");
                url1 = Regex.Replace(url1, "\\s{2,}", ",");
                url2 = Regex.Replace(url2, "\\s{2,}", ",");
                url3 = Regex.Replace(url3, "\\s{2,}", ",");
                url4 = Regex.Replace(url4, "\\s{2,}", ",");
                url = url.Replace(",", "");
                url1 = url1.Replace(",", "");
                url2 = url2.Replace(",", "");
                url3 = url3.Replace(",", "");
                url4 = url4.Replace(",", "");

                //^[1-9]\d*\.\d*|0\.\d*[1-9]\d 提取每T收益
                Regex r = new Regex(@"^[1-9]\d*\.\d*|0\.\d*[1-9]\d");
                double pft = 0;
                if (r.IsMatch(url2))
                {
                    MatchCollection mc = r.Matches(url2);
                    pft = Convert.ToDouble(mc[0].Value.ToString());
                }
    
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>" + sss + "</script>");
                if (string.IsNullOrEmpty(Request["all"]))
                {
                    Label4.Text = "全网算力:" + url + " 当前难度:" + url1 + " 预计" + url4 + "后难度变化" + url3;
                    Label3.Text = DateTime.Now.ToString();
                    
                }
                else
                {
                    Label4.Text = "全网算力:" + url + " 当前难度:" + url1 + " 预计" + url4 + "后难度变化" + url3;
                    string rawstr = HttpGet(kurl, "");
                    JObject btc = JObject.Parse(JObject.Parse(rawstr)["btc"].ToString());
                    double account_balance = Convert.ToDouble(btc["account_balance"]);
                    double hashrate_last_10min = Convert.ToDouble(btc["hashrate_last_10min"]); //281299054850211,
                    double hashrate_last_1day = Convert.ToDouble(btc["hashrate_last_1day"]); //": 159429186027,
                    double hashrate_last_1hour = Convert.ToDouble(btc["hashrate_last_1hour"]); //": 60209256737013,
                    double payment_total = Convert.ToDouble(btc["payment_total"]);//": 0.0,
                    double pool_hashrate_last_10min = Convert.ToDouble(btc["pool_hashrate_last_10min"]);//": 871419295355403008,
                    double pool_hashrate_last_1hour = Convert.ToDouble(btc["pool_hashrate_last_1hour"]); //": 870609823312939136,
                    double profit_24hour = Convert.ToDouble(btc["profit_24hour"]); //": 0.0,
                    double profit_total = Convert.ToDouble(btc["profit_total"]);//": 0.0,
                    int worker_active = Convert.ToInt32(btc["worker_active"]); //": 20,
                    int worker_unactive = Convert.ToInt32(btc["worker_unactive"]); //": 0
                    string l = DateTime.Now.ToString() + "\r\n矿场情况 帐户余额:" + account_balance.ToString("f6") + "  已支付:" + payment_total.ToString("f6") + "  24小时收益:" + profit_24hour.ToString("f6") + "  总收益:" + profit_total.ToString("f6") + "  10分钟算力:" + ConvertHashrate(hashrate_last_10min, pft) + "  1小时算力:" + ConvertHashrate(hashrate_last_1hour, pft) + "  1天算力:" + ConvertHashrate(hashrate_last_1day, pft,true);
                    Label3.Text = l;
                }
              
                try
                {
                    string rawstr = HttpGet(@"https://pool.viabtc.com/user/api/81c60290759666773afac26ee159bf87/", "");
                    JObject btc=JObject.Parse(JObject.Parse(rawstr)["btc"].ToString());
                    double account_balance = Convert.ToDouble(btc["account_balance"]);
                    double hashrate_last_10min = Convert.ToDouble(btc["hashrate_last_10min"]); //281299054850211,
                    double hashrate_last_1day = Convert.ToDouble(btc["hashrate_last_1day"]); //": 159429186027,
                    double hashrate_last_1hour = Convert.ToDouble(btc["hashrate_last_1hour"]); //": 60209256737013,
                    double payment_total = Convert.ToDouble(btc["payment_total"]);//": 0.0,
                    double pool_hashrate_last_10min = Convert.ToDouble(btc["pool_hashrate_last_10min"]);//": 871419295355403008,
                    double pool_hashrate_last_1hour = Convert.ToDouble(btc["pool_hashrate_last_1hour"]); //": 870609823312939136,
                    double profit_24hour = Convert.ToDouble(btc["profit_24hour"]); //": 0.0,
                    double profit_total = Convert.ToDouble(btc["profit_total"]);//": 0.0,
                    int worker_active = Convert.ToInt32(btc["worker_active"]); //": 20,
                    int worker_unactive = Convert.ToInt32(btc["worker_unactive"]); //": 0
                    string l = "矿场情况 帐户余额:" + account_balance.ToString("f6") + "  已支付:" + payment_total.ToString("f6") + "  24小时收益:" + profit_24hour.ToString("f6") + "  总收益:" + profit_total.ToString("f6") + "  10分钟算力:" + ConvertHashrate(hashrate_last_10min, pft) + "  1小时算力:" + ConvertHashrate(hashrate_last_1hour, pft) + "  1天算力:" + ConvertHashrate(hashrate_last_1day, pft,true);
                    Label2.Text = l;
                }
                catch
                {

                }
            }
            
        }
        static private string GetWebRequest(string Url)
        {
            string ret = string.Empty;
            try
            {

                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(Url));
                webReq.Method = "GET";
                webReq.ContentType = "text/html;charset=UTF-8";



                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }
        private string ConvertHashrate(double d,double price,bool money=false)
        {
            double k = 1000;
            double m = k * 1000;
            double g = m * 1000;
            double t = g * 1000;
            double p = t * 1000;
            double e = p * 1000;
            if (d > k)//k
            {
                if (d > m)//m
                {
                    if (d > g)//g
                    {
                        if (d > t)//t
                        {
                            if (d > p)//p
                            {
                                if (d > e)//e
                                {
                                    return (d / e).ToString("f2") + "E";
                                }
                                else
                                {
                                    return (d / p).ToString("f2") + "P";
                                }
                            }
                            else
                            {
                                if (money)
                                {
                                    double x = d / t;
                                    return (d / t).ToString("f2") + "T" + "(收益预测:" + x.ToString("f2") + "CNY)";
                                }
                                else
                                {
                                    return (d / t).ToString("f2") + "T";
                                }
                            }
                        }
                        else
                        {
                            return (d / g).ToString("f2") + "G";
                        }
                    }
                    else
                    {
                        return (d / m).ToString("f2") + "M";
                    }
                }
                else
                {
                    return (d / k).ToString("f2") + "K";
                }
            }
            else
            {
                return d.ToString();
            }
        }
        private int no = 0;
        protected void OnRowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
               
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GridView2, "Select$" + e.Row.RowIndex);
             
                   
                try
                {
                    e.Row.Cells[8].ForeColor = Color.DarkGreen;
                    Convert.ToInt32(e.Row.Cells[12].Text);
                    e.Row.Cells[0].Text = (no + 1).ToString();
                    no++;
                    DateTime t = Convert.ToDateTime(e.Row.Cells[2].Text);
                    if ((DateTime.Now - t).TotalMinutes > 5)
                    {
                        e.Row.BackColor = Color.LightBlue;
                    }
                }
                catch
                {
                    e.Row.Cells[8].ForeColor = Color.DarkRed;
                    e.Row.Visible = false;
                }
                
            }
        }
        protected void OnRowDataBound1(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                


                try
                {
                    e.Row.Cells[7].ForeColor = Color.DarkGreen;
                    Convert.ToInt32(e.Row.Cells[11].Text);
                    
                   
                    DateTime t = Convert.ToDateTime(e.Row.Cells[1].Text);
                    if ((DateTime.Now - t).TotalMinutes > 5)
                    {
                        e.Row.BackColor = Color.LightBlue;
                    }
                }
                catch
                {
                    e.Row.Cells[7].ForeColor = Color.DarkRed;
                    e.Row.Visible = false;
                }

            }
        }
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridView2.Rows)
            {
                if (row.RowIndex == GridView2.SelectedIndex)
                {
                    row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
                    if (row.Cells[8].Text.ToString() == "Success")
                    {
                        //string redirect = "<script>window.open('temp.aspx?ip=" + row.Cells[4].Text.ToString() + "');</script>";
                        //Response.Write(redirect);
                        string ip = "";
                        double max = 0;
                        double min = 1000;
                        if (!(GridView2.SelectedRow.Cells[8].Text.ToString() == "Success"))
                            return;
                        DateTime start = DateTime.Now.AddDays(-1);
                        ip = GridView2.SelectedRow.Cells[5].Text.ToString();
                        DataTable Miners = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from miner_remote_Log where IPaddress='" + ip + "' and TimeStamp>'" + start.ToString() + "' and len(ctep1)>0 order by id asc").Tables[0];
                        string[] ctempstr = new string[Miners.Rows.Count];
                        double[] ctemp1 = new double[Miners.Rows.Count];


                        double[] ctemp2 = new double[Miners.Rows.Count];


                        double[] ctemp3 = new double[Miners.Rows.Count];
                        string x = "[";
                        string y1 = "[";
                        string y2= "[";
                        string y3 = "[";
                        for (int i = 0; i < Miners.Rows.Count; i++)
                        {
                            
                            ctempstr[i] = Miners.Rows[i]["TimeStamp"].ToString();
                            if (i < Miners.Rows.Count - 1)
                            {
                                x = x + "'" + Miners.Rows[i]["TimeStamp"].ToString() + "',";
                                y1 = y1  + Miners.Rows[i]["ctep1"].ToString() + ",";
                                y2 = y2  + Miners.Rows[i]["ctep2"].ToString() + ",";
                                y3 = y3  + Miners.Rows[i]["ctep3"].ToString() + ",";
                            }
                            else
                            {
                                x = x + "'" + Miners.Rows[i]["TimeStamp"].ToString() + "']";
                                y1 = y1 + Miners.Rows[i]["ctep1"].ToString() + "]";
                                y2 = y2 + Miners.Rows[i]["ctep2"].ToString() + "]";
                                y3 = y3 + Miners.Rows[i]["ctep3"].ToString() + "]";
                            }
                            try
                            {
                                ctemp1[i] = Convert.ToDouble(Miners.Rows[i]["ctep1"]);

                                ctemp2[i] = Convert.ToDouble(Miners.Rows[i]["ctep2"]);
                                ctemp3[i] = Convert.ToDouble(Miners.Rows[i]["ctep3"]);
                            }
                            catch
                            {
 
                            }
                            //Miners_remote.Rows[i]["IPaddress"] = "<a href='temp.aspx?ip='>" + Miners_remote.Rows[i]["IPaddress"].ToString()+"</a>";
                        }
                        try
                        {
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
                            //Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>chart("+x+","+y1+","+y2+","+y3+")</script>");
                            Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script>chart(" + x + "," + y1 + "," + y2 + "," + y3 + ");" + sss + "</script>");
                        }
                        catch
                        { }
                        //Chart1.ChartAreas[0].AxisY.Maximum = max + 2;
                        //Chart1.ChartAreas[0].AxisY.Minimum = min - 2;
                        //Chart1.Series[0].Points.DataBindXY(ctempstr, ctemp1);
                        //Chart1.Series[1].Points.DataBindXY(ctempstr, ctemp2);
                        //Chart1.Series[2].Points.DataBindXY(ctempstr, ctemp3);
                        //Panel1.Visible = true;
                        //Chart1.Width = Convert.ToInt32(Panel1.Width.Value)- 50;
                      
                        //Chart1.Style.Clear();
                        //Chart1.CssClass = "cs";
                        
                        //chart([一月, 二月], [1, 2, 3], [4, 5, 6], [7, 8, 9]);
                    }
                    row.ToolTip = string.Empty;
                }
                else
                {
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    row.ToolTip = "Click to select this row.";
                }
            }
        }
        public static bool TestNetConnected(string strIP, int WaitSecond, int iTestTimes)
        {
            for (int i = 0; i < iTestTimes - 1; i++)
            {
                if (TestNetConnectity(strIP))
                {
                    return true;
                }
                Thread.Sleep(WaitSecond * 100);
            }

            return false;
        }
        public static bool TestNetConnectity(string strIP)
        {
            bool result;
            try
            {
                Ping ping = new Ping();
                PingOptions pingOptions = new PingOptions();
                pingOptions.DontFragment = true;
                string s = "testtesttesttesttesttesttesttest";
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                int timeout = 500;
                PingReply pingReply = ping.Send(strIP, timeout, bytes, pingOptions);
                ping.Dispose();
                result = (pingReply.Status == IPStatus.Success);
            }
            catch
            {
                result = false;
            }
            return result;
        }
        // BitMainMinerTool.Form1
        private static List<string> cgminer_api_stats_list(string ip)
        {
            List<string> list = new List<string>();
            int num = 0;
            string item = "";
            string item2 = "";
            string item3 = "";
            string item4 = "";
            string item5 = "";
            try
            {
                string text = send_cgminer_command(ip, "{\"command\":\"pools\"}");
                string text2 = send_cgminer_command(ip, "{\"command\":\"stats\"}");
                send_cgminer_command(ip, "{\"command\":\"summary\"}");

                int startIndex = text2.IndexOf("}") + 1;
                startIndex = text2.IndexOf("}", startIndex) + 1;
                text2 = text2.Insert(startIndex, ",");

                JObject jObject = JsonConvert.DeserializeObject<JObject>(text2);
                Regex regex = new Regex("\"Type\":\"(.*?)\"");
                Match match = regex.Match(text2);
                Regex regex2 = new Regex("\"CompileTime\":\"(.*?)\"");
                Match match2 = regex2.Match(text2);
                Regex regex3 = new Regex("\"Elapsed\":(.*?),");
                Match match3 = regex3.Match(text2);
                Regex regex4 = new Regex("\"GHS 5s\":(.*?),");
                Match match4 = regex4.Match(text2);
                Regex regex5 = new Regex("\"GHS av\":(.*?),");
                Match match5 = regex5.Match(text2);
                Regex regex6 = new Regex("\"frequency\":(.*?),");
                Match match6 = regex6.Match(text2);
                Regex regex7 = new Regex("\"no_matching_work\":(.*?),");
                Match match7 = regex7.Match(text2);
                Regex regex8 = new Regex("\"Device Hardware%\":(.*?),");
                Match match8 = regex8.Match(text2);
                for (int i = 0; i < 16; i++)
                {
                    string key = "temp" + (i + 1).ToString();
                    if (!jObject["STATS"][1][key].ToString().Equals("0"))
                    {
                        switch (num)
                        {
                            case 0:
                                item = jObject["STATS"][1][key].ToString();
                                break;
                            case 1:
                                item2 = jObject["STATS"][1][key].ToString();
                                break;
                            case 2:
                                item3 = jObject["STATS"][1][key].ToString();
                                break;
                        }
                        num++;
                    }
                }
                num = 0;
                for (int i = 0; i < 8; i++)
                {
                    string key = "fan" + (i + 1).ToString();
                    if (!jObject["STATS"][1][key].ToString().Equals("0"))
                    {
                        switch (num)
                        {
                            case 0:
                                item4 = jObject["STATS"][1][key].ToString();
                                break;
                            case 1:
                                item5 = jObject["STATS"][1][key].ToString();
                                break;
                        }
                        num++;
                    }
                }
                string time = match2.Groups[1].ToString();
                string text3 = string.Empty;
                text3 = check_version(time, text3);
                Regex regex9 = new Regex("\"POOL\":0,\"URL\":\"(.*?)\",.*?\"User\":\"(.*?)\"");
                Match match9 = regex9.Match(text);
                Regex regex10 = new Regex("\"POOL\":1,\"URL\":\"(.*?)\",.*?\"User\":\"(.*?)\"");
                Match match10 = regex10.Match(text);
                Regex regex11 = new Regex("\"POOL\":2,\"URL\":\"(.*?)\",.*?\"User\":\"(.*?)\"");
                Match match11 = regex11.Match(text);
                List<string> list2 = new List<string>();
                list2.Add(match.Groups[1].ToString());
                list2.Add(text3);
                list2.Add(match6.Groups[1].ToString().Replace("\"", ""));
                list2.Add(elapsed_to_time(match3.Groups[1].ToString()));
                list2.Add(match4.Groups[1].ToString().Replace("\"",""));
                list2.Add(match5.Groups[1].ToString());
                list2.Add(match7.Groups[1].ToString());
                list2.Add(match8.Groups[1].ToString() + "%");
                list2.Add(item);
                list2.Add(item2);
                list2.Add(item3);
                list2.Add(item4);
                list2.Add(item5);
                list2.Add(match9.Groups[1].ToString());
                list2.Add(match9.Groups[2].ToString());
                list2.Add(match10.Groups[1].ToString());
                list2.Add(match10.Groups[2].ToString());
                list2.Add(match11.Groups[1].ToString());
                list2.Add(match11.Groups[2].ToString());
                if (text.Equals(""))
                {
                    list.Add("Fail to connected!");
                    return list;
                }
                list.Add(ip);
                list.Add("SUCCESS");
                list.AddRange(list2);
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(typeof(Form1), ex);
                list.Add("Fail to connected!");
            }
            return list;
        }
        // BitMainMinerTool.Form1
        private static string send_cgminer_command(string ip, string cmd)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string text = "";
            try
            {
                socket.Connect(ip, 4028);
                byte[] bytes = Encoding.ASCII.GetBytes(cmd);
                socket.Send(bytes, bytes.Length, SocketFlags.None);
                socket.ReceiveTimeout = 500;
                byte[] array = new byte[1024];
                while (true)
                {
                    int num = socket.Receive(array, array.Length, SocketFlags.None);
                    if (num <= 0)
                    {
                        break;
                    }
                    text += Encoding.ASCII.GetString(array, 0, num);
                }
                socket.Close();
            }
            catch (Exception ex)
            {

                text = "";
            }
            return text;
        }
        private static string check_version(string time, string version)
        {
            try
            {
                if (time.IndexOf("CST") != -1)
                {
                    version = DateTime.ParseExact(time.Replace("  ", " "), "ddd MMM d HH:mm:ss CST yyyy", CultureInfo.CreateSpecificCulture("en-US")).ToShortDateString();
                }
                else
                {
                    if (time.IndexOf("EST") == -1)
                    {
                        return "Unknown";
                    }
                    version = DateTime.ParseExact(time.Replace("  ", " "), "ddd MMM d HH:mm:ss EST yyyy", CultureInfo.CreateSpecificCulture("en-US")).ToShortDateString();
                }
                string[] array = version.Split(new char[]
                {
            '/'
                });
                if (array[2].Length == 1)
                {
                    array[2] = "0" + array[2];
                }
                if (array[1].Length == 1)
                {
                    array[1] = "0" + array[1];
                }
                version = array[0] + array[1] + array[2];
            }
            catch
            {
                version = "Unknown";
            }
            return version;
        }
        // BitMainMinerTool.Form1
        private static string elapsed_to_time(string group)
        {
            int num;
            if (!int.TryParse(group, out num))
            {
                num = 0;
            }
            string str = string.Empty;
            if (num / 86400 != 0)
            {
                str = str + (num / 86400).ToString() + "d";
                num %= 86400;
            }
            if (num / 3600 != 0)
            {
                str = str + (num / 3600).ToString() + "h";
                num %= 3600;
            }
            if (num / 60 != 0)
            {
                str = str + (num / 60).ToString() + "m";
                num %= 60;
            }
            return str + num.ToString() + "s";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            DataTable Miners_remote = SqlHelper.ExecuteDataset(SqlHelper.GetConnSting(), CommandType.Text, "select * from miner_remote").Tables[0];

            GridView2.DataSource = Miners_remote.DefaultView;
            GridView2.DataBind();
        }

        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}