using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OKCoinAPI;

namespace OKT
{
    public partial class Form1 : Form
    {
        Monitor monitor = new Monitor();

        public Form1()
        {
            //TestF();
            //Test();
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (monitor.IsRunning)
            {
                //Cancel
                monitor.Stop();
                btnStart.Text = "Start";
            }
            else
            {
                //Start
                monitor.Start();
                btnStart.Text = "Stop";
            }

        }

        void Test()
        {
            //Monitor mon = new Monitor();
            //mon.Start();

            //System.Threading.Thread.Sleep(10000);
            //mon.Stop();


            //return;
            ////填写API key
            //string api_key = "";
            //string secret_key = "";

            //OKCoin ok = new OKCoin(api_key, secret_key);

            //ok.GetTicker();
            //ok.GetDepth();
            //ok.GetUserInfo();
            //ok.OrderInfo("-1");
            //ok.CancelOrder("109696287");
            //ok.Trade(new Trade(TradeType.buy, Decimal.Parse("10.7"), Decimal.Parse("0.1")));
            

        }


        void TestQ()
        {
            Queue<int> queueList = new Queue<int>();

            for (int i = 0; i < 20; i++)
                queueList.Enqueue(i);

            foreach (int n in queueList)
            {
                Console.WriteLine(n);
            }

            queueList.Dequeue();
            queueList.Dequeue();

        }

        void TestF()
        {
            DataFilter dataFilter = new DataFilter();

            Random rand = new Random();

            for (int i = 0; i < 30; i++)
            {
                Ticker ticker = new Ticker();
                ticker.Last = (Decimal)(rand.NextDouble() * 100);

                dataFilter.Enqueue(ticker);

            }

        }

    }
}
