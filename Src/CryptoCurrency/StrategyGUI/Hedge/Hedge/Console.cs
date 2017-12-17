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
    public partial class Console : UserControl
    {
        public Console(KFCC.ExchangeInterface.IExchanges e)
        {

            InitializeComponent();
            e.TickerEvent += new CommonLab.ExchangeEventWarper.TickerEventHander(Exchange_TickerEvent);
            //e.DepthEvent +=new CommonLab.ExchangeEventWarper.DepthEventHander(Exchange_DepthEvent);
        }
        //public void Update()
        private void Console_Resize(object sender, EventArgs e)
        {
            textBox1.Width = this.Width - 6;
            textBox1.Height = this.Height - 6;
            textBox1.Location = new Point(3, 3);
        }
        private delegate void UpdateConsole(Color c, string msg);
        private  void Exchange_DepthEvent(object sender, CommonLab.Depth d, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString() + ": " + d.ToString(5));
                if (textBox1.InvokeRequired)
                {
                    UpdateConsole uc = new UpdateConsole(UpdateConsoleMthod);
                    textBox1.Invoke(uc, new object[] { (object)Color.Blue, (object)sb.ToString() });
                }
                else
                {
                    UpdateConsoleMthod(Color.Blue, sb.ToString());
                }
            }
            catch
            { }
        }
        private void UpdateConsoleMthod(Color c, string msg)
        {
            string[] lines = new string[200] ;
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
                textBox1.AppendText(msg);
                textBox1.Select(textBox1.TextLength, 0);//设置光标的位置到文本尾  
                textBox1.ScrollToCaret();//滚动到控件光标处  
            }
            catch
            { }
        }
        private  void Exchange_TickerEvent(object sender, CommonLab.Ticker t, CommonLab.EventTypes et,  CommonLab.TradePair tp)
        {
           
        
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString() +tp.FromSymbol+"/"+tp.ToSymbol+ ": " + t.ToString());
            if (textBox1.InvokeRequired)
            {
                UpdateConsole uc = new UpdateConsole(UpdateConsoleMthod);
                textBox1.Invoke(uc, new object[] { (object)Color.Red, (object)sb.ToString() });
            }
            else
            {
                UpdateConsoleMthod(Color.Blue, sb.ToString());
            }
        }
    }
}
