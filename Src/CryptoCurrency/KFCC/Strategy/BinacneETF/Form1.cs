using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        }

        private void Config_UpdateConsoleEvent(Color c, string Msg)
        {
            ConsoleInfo(c, Msg);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

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
                textBox1.AppendText(DateTime.Now.ToString()+msg);
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
            Config.Exchange.Stop();
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
        
    }
}
