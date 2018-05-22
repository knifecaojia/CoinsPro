using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinacneETF
{
    public partial class MainWindow : Form
    {
        private TabControl QuoteSymbolTabC;

        public MainWindow()
        {
            InitializeComponent();
            Config.UpdateConsoleEvent += Config_UpdateConsoleEvent;
            Config.UpdateServiceStatusEvent += Config_UpdateServiceStatusEvent;
        }

        private void Config_UpdateServiceStatusEvent(Color c, string Msg)
        {
            ServiceStatusInfo(c, Msg);
        }

        private void Config_UpdateConsoleEvent(Color c, string Msg)
        {
            ConsoleInfo(c, Msg);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
        private delegate void UpdateServiceStatusEventHandle(Color c, string msg);
        private void UpdateServiceStatusMethod(Color c, string msg)
        {

            try
            {
                label2.ForeColor = c;
                label2.Text = msg;
            }
            catch
            { }
        }
        public void ServiceStatusInfo(Color c, string Msg)
        {
            if (label2.InvokeRequired)
            {
                UpdateServiceStatusEventHandle uc = new UpdateServiceStatusEventHandle(UpdateServiceStatusMethod);
                label2.Invoke(uc, new object[] { (object)c, Msg });
            }
            else
            {
                UpdateServiceStatusMethod(c, Msg);
            }
        }
        private delegate void UpdateConsole(Color c, string msg);
        private void UpdateConsoleMthod(Color c, string msg)
        {
            string[] lines = new string[200];
            if (textBox1.Lines.Length > 200)
            {
                try
                {
                    Array.Copy(textBox1.Lines, textBox1.Lines.Length - 200, lines, 0, 200);


                    textBox1.Lines = lines;
                }
                catch
                {

                }
            }
            try
            {
                textBox1.AppendText("\n");
                textBox1.SelectionColor = c;
                textBox1.AppendText(DateTime.Now.ToString() + msg);
                textBox1.Select(textBox1.TextLength, 0);//设置光标的位置到文本尾  
                textBox1.ScrollToCaret();//滚动到控件光标处  
            }
            catch
            { }
        }
        public void ConsoleInfo(Color c, string Msg)
        {
            if (textBox1.InvokeRequired)
            {
                UpdateConsole uc = new UpdateConsole(UpdateConsoleMthod);
                textBox1.Invoke(uc, new object[] { (object)c, Msg });
            }
            else
            {
                UpdateConsoleMthod(c, Msg);
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Config.Log.log("读取交易所信息");
            Config.RaiseUpdateConsoleEvent(Color.Black, "读取交易所信息");
            Config.Exchange = new ExchangeInfo();
            Config.Exchange.GetExchangeInfo();
            //Config.Log.log("读取交易所信息");
            Config.RaiseUpdateConsoleEvent(Color.Black, Config.Exchange.ToString());


            #region 读取历史数据
            //foreach (TradingSymbol t in Config.Exchange.Symbols)
            //{
            //    Config.RaiseUpdateConsoleEvent(Color.Blue, "读取"+ t.Symbol+"历史数据");
            //    Config.Exchange.GetHisData(t.Symbol, new DateTime(2018, 1, 1));
            //}
            #endregion
            timer1.Start();
        }

        private void 开始监测ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Exchange.Start();
            QuoteSymbolTabC = new TabControl();
            QuoteSymbolTabC.Dock = DockStyle.Fill;
            foreach (KeyValuePair<string, List<TradingSymbol>> Sbl in Config.Exchange.SymbolsClassByQuoteAsset)
            {
                TabPage tp = new TabPage(Sbl.Key);
                TradingInfo_Symbol tsd = new TradingInfo_Symbol(Sbl.Key);
                tsd.Dock = DockStyle.Fill;
                tp.Controls.Add(tsd);
                QuoteSymbolTabC.TabPages.Add(tp);
            }
            splitContainer2.Panel1.Controls.Add(QuoteSymbolTabC);

        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Config.Exchange.Stop();
            }
            catch
            { }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Config.strategyConfig.SaveConfig(Config.strategyConfig);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Config.IsStrategyWorking)
            {
                label1.ForeColor = Color.DarkBlue;
                label1.Text = "策略启动时间:" + Config.StrategyStartTime.ToString() + " 运行时常:" + ((DateTime.Now - Config.StrategyStartTime).ToString("h'h 'm'm 's's'"));
            }
            else
            {
                label1.ForeColor = Color.DarkRed;
                label1.Text = "策略未启动";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Config.IsStrategyWorking = true;
            Config.StrategyStartTime = DateTime.Now;
            Config.Exchange.LoadHisData(dateTimePicker1.Value);
            Config.UpdateTickerEvent += Config_UpdateTickerEvent;
            label2.Text = "监测:" + Config.strategyConfig.SelectedSymbols.Count + "种币";
        }

        private void Config_UpdateTickerEvent()
        {
            string str = "";
            double index = Config.Exchange.CacCoinIndex();
            str = "监测时间:" + DateTime.Now.ToString() + " CoinIndex:" + index;
            if (index > 1)
                UpdateWatch(Color.Red, str);
            else
                UpdateWatch(Color.Green, str);
        }



        private delegate void UpdateWatchHandle(Color c, string msg);
        private void UpdateWatchMthod(Color c, string msg)
        {

            label5.ForeColor = c;
            label5.Text = msg;
        }
        public void UpdateWatch(Color c, string Msg)
        {
            if (label5.InvokeRequired)
            {
                UpdateWatchHandle uc = new UpdateWatchHandle(UpdateWatchMthod);
                label5.Invoke(uc, new object[] { (object)c, Msg });
            }
            else
            {
                UpdateWatchMthod(c, Msg);
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = 1;
            foreach (TradingSymbol item in Config.Exchange.Symbols)
            {
                Config.Exchange.GetHisData(item.Symbol, new DateTime(2018, 1, 1));
                Config.RaiseUpdateConsoleEvent(Color.Black, i + "/" + Config.Exchange.Symbols.Count + "读取" + item.Symbol + "Kline信息");
                i++;
            }
        }


        long totall = 0;
        int num = 0;
        DateTime s;
        private void Config_UpdateTradeEvent(string symbol,CommonLab.Trade t)
        {
            double sec = (DateTime.Now - s).TotalSeconds;
            if (sec == 0) sec = 1;
            num++;
            string jsont = JsonConvert.SerializeObject(t);
            totall += jsont.Length;
            Config.RaiseUpdateConsoleEvent(System.Drawing.Color.Blue, t.ToString() + "\r\n avglen:" + totall / num + " deals:" + num / sec);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            //测试获取localbitcoin获取法币均价
            //double avg = CommonLab.LocalBitcoinPrice.getLocalBitcoinPrice("CNY",Config.Proxy);
            s = DateTime.Now;
            Config.UpdateTradeEvent += Config_UpdateTradeEvent;
            List<string> symbols = new List<string>();
            symbols.Add("ethbtc");
            Config.Exchange.StartCollectTrade(symbols);
        }
    }
    public class TradingSymbol
    {
        public string Symbol { get; set; }
        public string Status { get; set; }
        public string BaseAsset { get; set; }
        public int baseAssetPrecision { get; set; }
        public string QuoteAsset { get; set; }
        public int quoteAssetPrecision { get; set; }
        public double minPrice { get; set;}
        public double maxPrice { get; set; }
        public double minQty { get; set; }
        public double maxQty { get; set; }
        public BATicker Ticker { get; set; }
        /// <summary>
        /// 用于计算指数的历史数据，如果没有历史数据的化，就按能取到的最早数据进行测算
        /// </summary>
        public BATicker HisTicker { get; set; }
        public void LoadHisData(DateTime t)
        {
            string json = "";
            using (StreamReader sr = new StreamReader(@"HisData\" + Symbol + ".config"))
            {
                json = sr.ReadToEnd();
            }
            List<CommonLab.Ticker> tickers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CommonLab.Ticker>>(json);
            CommonLab.Ticker ticker=tickers.Where(item => item.ExchangeTimeStamp > CommonLab.TimerHelper.GetTimeStampMilliSeconds(t)).OrderBy(i=>i.ExchangeTimeStamp).ToArray()[0];
            HisTicker = new BATicker(ticker);
        }
    }
}
