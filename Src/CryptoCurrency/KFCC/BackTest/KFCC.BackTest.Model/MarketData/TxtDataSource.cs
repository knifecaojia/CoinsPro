using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.BackTest.Model.MarketData
{
    public class TxtDataSource
    {
        string Path;
        string ExchangeName;
        string TradingPair;
        DateTime StartTime;
        DateTime EndTime;
        public TxtDataSource(string path
            , string exchangename
            , string tradingpair
            , DateTime starttime
            , DateTime endtime)
        {
            this.Path = path;
            this.ExchangeName = exchangename;
            this.TradingPair = tradingpair;
            this.StartTime = starttime;
            this.EndTime = endtime;
        }
        public IList<CommonLab.Ticker> GetTickers(CommonLab.TickerType type)
        {
            IList<CommonLab.Ticker> TickerList = new List<CommonLab.Ticker>();
            int totaldays = (int)Math.Floor((EndTime - StartTime).TotalDays);
            for (int i = 0; i <= totaldays; i++)
            {
                string filepath = $"{this.Path}/{this.ExchangeName}/{TradingPair}/{(StartTime.AddDays(i)).Year}/{(StartTime.AddDays(i)).Month}/{(StartTime.AddDays(i)).Day}.txt";
                try
                {
                    StreamReader sr = new StreamReader(filepath);
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("xml template:" + line);
                    }
                    if (sr != null) sr.Close();
                }
                catch
                {

                }
            }
            return TickerList;

        }
    }
}
