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
    public partial class OrderBook : UserControl
    {
        KFCC.ExchangeInterface.IExchanges exchange;
        
        public OrderBook(KFCC.ExchangeInterface.IExchanges e)
        {
            InitializeComponent();
            exchange = e;
            foreach (var item in exchange.SubscribedTradingPairs)
            {
                comboBox1.Items.Add(item.Key);
            }
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            e.SubscribedEvent += E_SubscribedEvent;
            e.DepthEvent += E_DepthEvent;
        }

        private void E_SubscribedEvent(object sender, CommonLab.SubscribeTypes st, CommonLab.TradePair tp)
        {
            if (comboBox1.InvokeRequired)
            {
                UpdateCombox uc = new UpdateCombox(UpdateComboxMethod);
                comboBox1.Invoke(uc);
            }
            else
                UpdateComboxMethod();
        }
        delegate void UpdateCombox();
        private void UpdateComboxMethod()
        {
            string s = comboBox1.Text;
            comboBox1.Items.Clear();
            foreach (var item in exchange.SubscribedTradingPairs)
            {
                comboBox1.Items.Add(item.Key);
            }
            comboBox1.Text = s;
        }
        delegate string GetComboxText();
        delegate void UpdateDataGDV(CommonLab.Depth d);
        private void UpdateDgv(CommonLab.Depth d)
        {
            if (dataGridView1.Rows.Count != 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    dataGridView1.Rows.Add();
                }
            }

            for (int i = 0; i < 10; i++)
            {

                if (i < 5)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                    dataGridView1.Rows[i].Cells[0].Value = "卖" + (5 - i).ToString();
                    if (d.Asks.Count > 4)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = d.Asks[4 - i].Price.ToString();
                        dataGridView1.Rows[i].Cells[2].Value = d.Asks[4 - i].Amount.ToString();
                    }
                }
                else
                {
                    dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Green;
                    dataGridView1.Rows[i].Cells[0].Value = "买" + (i - 4).ToString();
                    if (d.Bids.Count > 4)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = d.Bids[ i-5].Price.ToString();
                        dataGridView1.Rows[i].Cells[2].Value = d.Bids[i-5].Amount.ToString();
                    }

                }
            }

        }
        private string GetCtext()
        { return comboBox1.Text;        }
        private void E_DepthEvent(object sender, CommonLab.Depth d, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            string tradingpair = "";
            if (comboBox1.InvokeRequired)
            {
                GetComboxText gt = new GetComboxText(GetCtext);
                tradingpair= comboBox1.Invoke(gt).ToString();
            }
            if (exchange.GetLocalTradingPairString(tp, (CommonLab.SubscribeTypes)et) != tradingpair)
            {
                return;
            }
            if (dataGridView1.InvokeRequired)
            {
                UpdateDataGDV upd = new UpdateDataGDV(UpdateDgv);
                dataGridView1.Invoke(upd, new object[] { (object)d });

            }
            else
                UpdateDgv(d);
            
        }

    }

}
