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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Config.Exchange = new ExchangeInfo();
            Config.Exchange.GetExchangeInfo();

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
        
    }
}
