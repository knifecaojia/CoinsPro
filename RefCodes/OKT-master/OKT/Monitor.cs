using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OKCoinAPI;

namespace OKT
{
    class Monitor
    {
        Thread thread = null;
        bool threadCancel;
                    
        string api_key;
        string secret_key;

        OKCoin okc;
        DataFilter dataFilter = new DataFilter();

        public bool IsRunning
        {
            get { return !threadCancel; }
        }

        public Monitor()
        {
            api_key = "a1c83045-2543-44ab-aedf-260b4e5c282b";
            secret_key = "292C2F689482E5EDFEA9486520D6DC4B";
            threadCancel = true;

            okc = new OKCoin(api_key, secret_key);

        }

        public void Start()
        {
            threadCancel = false;

            thread = new Thread(new ThreadStart(ThreadProc));
            thread.Name = "Monitor.ThreadProc";
            thread.IsBackground = true;
            thread.Start();

        }

        public void Stop()
        {
            threadCancel = true;
        }

        void ThreadProc()
        {            
            List<Order> orderList = okc.OrderInfo("-1");
            
            while (!threadCancel)
            {
                //Console.WriteLine(DateTime.Now);
                Thread.Sleep(2000);

                Ticker ticker =  okc.GetTicker();
                if (ticker != null)
                {
                    dataFilter.Enqueue(ticker);

                    //计算最新的值
                    Decimal avgValue = dataFilter.AvgValue;

                }

                //foreach (Order order in orderList)
                //{
                    
                //}

            }

            Console.WriteLine("stop");
        }



    }
}

