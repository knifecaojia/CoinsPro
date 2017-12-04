using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hedge
{
    public partial class TradingPairControl : UserControl
    {
        string tradingpair;
        DevComponents.DotNetBar.Charts.ChartXy Chart;
        public TradingPairControl(string _tradingpair)
        {
            tradingpair = _tradingpair;
            InitializeComponent();
            Chart=(DevComponents.DotNetBar.Charts.ChartXy)chartControl1.ChartPanel.ChartContainers.ToArray()[0];
            Chart.Titles[0].Text = tradingpair;
           
        }
    }
}
