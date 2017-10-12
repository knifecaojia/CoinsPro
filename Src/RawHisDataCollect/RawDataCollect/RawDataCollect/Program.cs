using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KFSoft.CommonLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace RawDataCollect
{
    /// <summary>
    /// 从
    /// 表名His2Hour_Bitstamp_BTCUSD_RAW
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Start collect CryptoCurrency market history hour data......");
            Console.WriteLine("Please input exchange name:");
            string exchangename = Console.ReadLine();
            Console.WriteLine("Please input from symbol to collect:");
            string fromsymbol = Console.ReadLine().ToUpper();
            Console.WriteLine("Please input to symbol to collect:");
            string tosymbol = Console.ReadLine().ToUpper();
            //Console.WriteLine("Please input to time(gmttimestamp) to collect:");
             
            int maxcount = 167;//每周采集一次
            int totalmax = 20000;
            int insertnum = 0;
            double totime = TimeHelper.GetTimeStamp(DateTime.Now);
            string url = @"https://min-api.cryptocompare.com/data/histohour?fsym=" + fromsymbol.ToUpper() + "&tsym=" + tosymbol.ToUpper() + "&limit=" + maxcount.ToString() + "&e="+ exchangename + "&toTs=" + totime.ToString();
            //check sql data table
          
            string sqltablename = "His2Hour_" + exchangename + "_" + fromsymbol + tosymbol + "_RAW";

            if (CheckDataTableName(sqltablename))
            {
                //
                Console.WriteLine(sqltablename + " exist.start collect......");
            }
            else
            {
                Console.WriteLine(sqltablename + " dose not exist.create new table......");
                CreatDataTable(sqltablename);
            }

            bool flag = true;
            HttpResult hr = new HttpResult();
            HttpHelper hh = new HttpHelper();
            HttpItem hi = new HttpItem();
            while (flag)
            {
                try
                {

                    hi.URL = url;
                    hr = hh.GetHtml(hi);
                    string rawjson = hr.Html;
                    JObject jo = JObject.Parse(rawjson);
                   
                    string response = jo["Response"].ToString();
                    if (response == "Success")
                    {
                        JArray jlist = JArray.Parse(jo["Data"].ToString()); //JArray解析这个JObject的字符串  
                        for (int i = 0; i < jlist.Count; ++i)  //遍历JArray  
                        {
                            JObject tempo = JObject.Parse(jlist[i].ToString());
                            string sql = "insert into "+ sqltablename + " values('" + TimeHelper.ConvertStringToDateTime(tempo["time"].ToString()) + "'," + tempo["close"].ToString() + "," + tempo["high"].ToString() + "," + tempo["open"].ToString() + "," + tempo["low"].ToString() + "," + tempo["volumefrom"].ToString() + "," + tempo["volumeto"].ToString() + ")";
                            try
                            {
                                Console.WriteLine("ts:"+TimeHelper.ConvertStringToDateTime(tempo["time"].ToString()) + "',close:" + tempo["close"].ToString() + ",high:" + tempo["high"].ToString() + ",open:" + tempo["open"].ToString() + ",low:" + tempo["low"].ToString() + ",vf:" + tempo["volumefrom"].ToString() + ",vt:" + tempo["volumeto"].ToString());
                                SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), System.Data.CommandType.Text, sql);
                                insertnum++;
                                if (insertnum >= totalmax)
                                    flag = false ;
                            }
                            catch(Exception err)
                            {
                                Console.WriteLine(err.Message);
                            }
                        }

                        url = @"https://min-api.cryptocompare.com/data/histohour?fsym=" + fromsymbol.ToUpper() + "&tsym=" + tosymbol.ToUpper() + "&limit=" + maxcount.ToString() + "&e=" + exchangename + "&toTs=" +jo["TimeFrom"].ToString();
                    }
                    else
                    {
                        Console.WriteLine(rawjson);
                    }
                }
                catch
                {

                }
            }
            Console.WriteLine("20000 datas collected press any key to exit......");
            Console.ReadKey();
        }
        static bool CheckDataTableName(string tablename)
        {
            bool flag = false;
            var sdr = SqlHelper.ExecuteScalar(SqlHelper.GetConnSting(), System.Data.CommandType.Text, "select * from sys.tables where name ='" + tablename + "'");
            if (sdr != null)
                flag = true;
            return flag;
        }
        static void CreatDataTable(string tablename)
        {
            string sqlcontent = "CREATE TABLE [dbo].["+tablename+"](" +
                                 "[id][int] IDENTITY(1, 1) NOT NULL," +

                                    "[ts] [datetime] NULL," +
    
                                         "[close] [float] NULL," +
                                         "[high] [float] NULL," +
                                         "[open] [float] NULL," +
                                         "[low] [float] NULL," +
                                        "[volumefrom] [float] NULL," +
                                         "[volumeto] [float] NULL," +
                                         "CONSTRAINT[IX_"+tablename+"] UNIQUE NONCLUSTERED" +
                                        "(" +
                                "[ts] ASC" +
                                ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]" +
                                ") ON[PRIMARY]";
            SqlHelper.ExecuteNonQuery(SqlHelper.GetConnSting(), System.Data.CommandType.Text, sqlcontent);
        }
    }
    class OCHL
    {
        public DateTime ts;
        public double close;
        public double high;
        public double low;
        public double open;
        public double volumefrom;
        public double volumeto;
    }
}
