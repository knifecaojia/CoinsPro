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
                textBox5.Text = "-0.1";
                textBox6.Text = "0.1";
            }
            else if (exchange.Name == "Bitstamp")
            {
                textBox1.Text = "SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa";
                textBox2.Text = "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l";
                textBox3.Text = "rqno1092";
                textBox4.Text = "caojia";
                textBox5.Text = "0.25";
                textBox6.Text = "0.25";
            }
            else if (exchange.Name == "Huobi")
            {
                textBox1.Text = "cbf0909f-7842f68b-8c0db43c-04172";
                textBox2.Text = "7e022c00-19e4e4a8-2b3ed1d9-312e0";
                textBox3.Text = "0";
                textBox4.Text = "caojia";
                textBox5.Text = "0.2";
                textBox6.Text = "0.2";
            }
            else if (exchange.Name == "Binance")
            {
                textBox1.Text = "EspHWtI5WbB3FVUoywxqpE9SkawJKQcrb3q2vu54b428uGdNdIyZlESi29DIBS4n";
                textBox2.Text = "BT5OJjq1IQuVmfp8yInJMfiy8aMBdFbRIHSQoB8QyRMucbBQmjWPdI1Plzdz54o3";
                textBox3.Text = "0";
                textBox4.Text = "caojia";
                textBox5.Text = "0.2";
                textBox6.Text = "0.2";
            }
            else if (exchange.Name == "ZB")
            {
                textBox1.Text = "16de7c10-2315-454d-b023-048058a6aed5";
                textBox2.Text = "1b3f8111-6dfe-4160-8eab-143986e04629";
                textBox3.Text = "0";
                textBox4.Text = "caojia";
                textBox5.Text = "0.2";
                textBox6.Text = "0.2";
            }
            Set();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Set();// exchange.SetupExchage(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
        }
        private void Set()
        {
            try
            {
                exchange.SetupExchage(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                exchange.SetupFee(textBox5.Text, textBox6.Text);
            }
            catch
            { }
        }
    }
}
