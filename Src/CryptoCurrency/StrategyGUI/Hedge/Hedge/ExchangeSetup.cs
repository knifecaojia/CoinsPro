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
    public partial class ExchangeSetup : UserControl
    {
        KFCC.ExchangeInterface.IExchanges exchange;

        public ExchangeSetup(KFCC.ExchangeInterface.IExchanges _e)
        {
            exchange = _e;
            InitializeComponent();
            if (exchange.Name == "OkCoin")
            {
                textBox1.Text = "a8716cf5-8e3d-4037-9a78-6ad59a66d6c4";
                textBox2.Text = "CF44F1C9F3BB23B148523B797B862D4C";
                textBox3.Text = "";
                textBox4.Text = "";

            }
            else if (exchange.Name == "Bitstamp")
            {
                textBox1.Text = "SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa";
                textBox2.Text = "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l";
                textBox3.Text = "rqno1092";
                textBox4.Text = "caojia";
            }
            else if (exchange.Name == "Huobi")
            {
                textBox1.Text = "cbf0909f-7842f68b-8c0db43c-04172";
                textBox2.Text = "7e022c00-19e4e4a8-2b3ed1d9-312e0";
                textBox3.Text = "0";
                textBox4.Text = "caojia";
            }
            Set();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            exchange.SetupExchage(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
        }
        private void Set()
        {
            exchange.SetupExchage(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
        }
    }
}
