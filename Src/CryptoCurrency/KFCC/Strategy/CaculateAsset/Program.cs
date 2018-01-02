using KFCC.ExchangeInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CaculateAsset
{
    class Program
    {
        static Dictionary<string, IExchanges> Exchanges=new Dictionary<string, IExchanges>();
        static void Main(string[] args)
        {
            LoadExchanges();
            ExchangeSetup();
            CheckAccount();
            Console.ReadKey();
        }
        private static void LoadExchanges()
        {
            List<string> pluginpath = FindPlugin();



            foreach (string filename in pluginpath)
            {
                try
                {
                    //获取文件名
                    string asmfile = filename;
                    string asmname = Path.GetFileNameWithoutExtension(asmfile);
                    if (asmname != string.Empty)
                    {
                        // 利用反射,构造DLL文件的实例
                        Assembly asm = Assembly.LoadFile(asmfile);
                        //利用反射,从程序集(DLL)中,提取类,并把此类实例化
                        Type[] t = asm.GetExportedTypes();
                        foreach (Type type in t)
                        {
                            if (type.GetInterface("IExchanges") != null)
                            {
                                IExchanges exchange = (IExchanges)Activator.CreateInstance(type);
                                Exchanges.Add(exchange.Name, exchange);
                                //Console.Write(exchange.Name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        //查找所有插件的路径
        private static List<string> FindPlugin()
        {
            List<string> pluginpath = new List<string>();
            try
            {
                //获取程序的基目录
                string path = AppDomain.CurrentDomain.BaseDirectory;
                //合并路径，指向插件所在目录。
                path = Path.Combine(path, "Plugins");
                foreach (string filename in Directory.GetFiles(path, "*.dll"))
                {
                    pluginpath.Add(filename);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return pluginpath;
        }
        static public void ExchangeSetup()
        {
            string s1="", s2 = "", s3 = "", s4 = "", s5 = "", s6 = "";
            foreach (KeyValuePair<string, IExchanges> e in Exchanges)
            {
                IExchanges exchange = e.Value;
               
                if (exchange.Name == "OkCoin")
                {
                    s1 = "a8716cf5-8e3d-4037-9a78-6ad59a66d6c4";
                    s2 = "CF44F1C9F3BB23B148523B797B862D4C";
                    s3 = "";
                    s4 = "";
                    s5 = "-0.1";
                    s6 = "0.1";
                }
                else if (exchange.Name == "Bitstamp")
                {
                    s1 = "SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa";
                    s2 = "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l";
                    s3 = "rqno1092";
                    s4 = "caojia";
                    s5 = "0.25";
                    s6 = "0.25";
                }
                else if (exchange.Name == "Huobi")
                {
                    s1 = "cbf0909f-7842f68b-8c0db43c-04172";
                    s2 = "7e022c00-19e4e4a8-2b3ed1d9-312e0";
                    s3 = "0";
                    s4 = "caojia";
                    s5 = "0.2";
                    s6 = "0.2";
                }
                //else if (exchange.Name == "Quoine")
                //{
                //    s1 = "cbf0909f-7842f68b-8c0db43c-04172";
                //    s2 = "7e022c00-19e4e4a8-2b3ed1d9-312e0";
                //    s3 = "0";
                //    s4 = "caojia";
                //    s5 = "0.2";
                //    s6 = "0.2";
                //}
                else if (exchange.Name == "Binance")
                {
                    s1 = "EspHWtI5WbB3FVUoywxqpE9SkawJKQcrb3q2vu54b428uGdNdIyZlESi29DIBS4n";
                    s2 = "BT5OJjq1IQuVmfp8yInJMfiy8aMBdFbRIHSQoB8QyRMucbBQmjWPdI1Plzdz54o3";
                    s3 = "0";
                    s4 = "caojia";
                    s5 = "0.2";
                    s6 = "0.2";
                }
                else if (exchange.Name == "ZB")
                {
                    s1 = "16de7c10-2315-454d-b023-048058a6aed5";
                    s2 = "1b3f8111-6dfe-4160-8eab-143986e04629";
                    s3 = "0";
                    s4 = "caojia";
                    s5 = "0.2";
                    s6 = "0.2";
                }
                exchange.SetupExchage(s1, s2, s3, s4);
                exchange.SetupFee(s5, s6);
            }
        }
        static  double rawbtc=0;//原始btc
        static double ebtc = 0;//折算btc
        static double btcusdrate = 0;
        static double btccnyrate = btcusdrate * 6.5;
        static private void CheckAccount()
        {
            foreach (KeyValuePair<string, IExchanges> e in Exchanges)
            {
                string raw;
                var a = new CommonLab.Account();
                try
                {
                     a = e.Value.GetAccount(out raw);
                }
                catch
                {
                    Console.WriteLine(e.Key + " GetAccount faile");
                }
                if (e.Value.Name == "Bitstamp")
                {
                    CommonLab.Ticker btcusdticker = e.Value.GetTicker(e.Value.GetLocalTradingPairString(new CommonLab.TradePair("btc", "usd")), out raw);
                    btcusdrate = btcusdticker.Buy;
                    btccnyrate = btcusdrate * 6.5;
                }
                if (a != null)
                {
                    foreach (KeyValuePair<string, CommonLab.Balance> item in a.Balances)
                    {
                        if (item.Value.balance > 0)
                        {
                            Console.WriteLine(e.Key + " has " + item.Key + " balance:" + item.Value.balance.ToString());
                            if (item.Key.ToLower() == "btc")
                            {
                                Console.WriteLine("raw btc waiting for exchange...");
                               rawbtc += item.Value.balance;
                            }
                            else
                            {
                                CommonLab.TradePair tp = new CommonLab.TradePair(item.Key, "btc");
                                try
                                {
                                    CommonLab.Ticker t = e.Value.GetTicker(e.Value.GetLocalTradingPairString(tp), out raw);
                                    ebtc += item.Value.balance * t.Last;
                                    Console.WriteLine("ebtc equals:" + (item.Value.balance * t.Buy) + " btc");
                                }
                                catch(Exception ee)

                                {
                                    Console.WriteLine(ee.Message);
                                }
                                
                            }
                        }
                    }
                }
            }
            Console.WriteLine("-------------------------------");
            Console.WriteLine("rawbtc:" + rawbtc + " ebtc:" + ebtc);
            Console.WriteLine("total btc:" + (rawbtc + ebtc));
            Console.WriteLine("equls cny:" + ((rawbtc + ebtc) * btccnyrate));
        }
    }
}
