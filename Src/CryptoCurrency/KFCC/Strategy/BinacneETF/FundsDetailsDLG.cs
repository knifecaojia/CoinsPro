using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace BinacneETF
{
    public partial class FundsDetailsDLG : Form
    {
        string symbol;
        private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("10.18.20.108:9527,abortConnect=false,ssl=false,password=...");
        IDatabase db = null;
        CommonLab.TimePeriodType timeperiodtype = CommonLab.TimePeriodType.m30;
        DateTime timestart, timeend;
        public FundsDetailsDLG(string _symobl)
        {
            symbol = _symobl;
            int databaseNumber = 0;
            object asyncState = new object();
            db = redis.GetDatabase(databaseNumber, asyncState);
            InitializeComponent();
            Updateinfo();
        }
        private void Updateinfo()
        {
            timestart = CommonLab.TimerHelper.GetStartTimeStampByPreiod(timeperiodtype, DateTime.UtcNow);
            timeend = CommonLab.TimerHelper.GetEndTimeStampByPreiod(timeperiodtype, timestart);
            string key = CommonLab.RedisKeyConvert.GetRedisKey(CommonLab.RedisKeyType.Trade, timeperiodtype, CommonLab.ExchangeNameConvert.GetShortExchangeName("binance"), symbol, timestart);
            if (db.KeyExists(key))
            {
                string json = db.StringGet(key);
                CommonLab.TradePeriod tp = JsonConvert.DeserializeObject<CommonLab.TradePeriod>(json);
                label20.Text = symbol;
                label19.Text = timestart.ToString()+"--"+timeend.ToString();
                label18.Text = tp.tradecountBuy.ToString();
                label17.Text = tp.baseSymbolVolumBuy.ToString();
                label16.Text = tp.quoteSymbolVolumBuy.ToString();
                label15.Text = tp.tradecountSell.ToString();
                label14.Text = tp.baseSymbolVolumSell.ToString();
                label13.Text = tp.quoteSymbolVolumSell.ToString();
                label12.Text = tp.CNYVolumBuy.ToString();
                label11.Text = tp.CNYVolumSell.ToString();
                double cnydiff = tp.CNYVolumBuy - tp.CNYVolumSell;
                if (cnydiff > 0)
                    label21.ForeColor = Color.Green;
                else if(cnydiff< 0)
                    label21.ForeColor = Color.Red;
                chart1.Series[0].Points.Clear();
                chart1.Series[0].Points.AddY(tp.CNYVolumBuy);
                chart1.Series[0].Points.AddY(tp.CNYVolumSell);
                chart1.Series[0].Points[0].Color = Color.Green;
                chart1.Series[0].Points[1].Color = Color.Red;
                label21.Text = cnydiff.ToString();
            }
            else
            {
                MessageBox.Show("Data not found！", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void minToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.m1;
            Updateinfo();
        }

        private void minToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.m5; Updateinfo();
        }

        private void minToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.m10; Updateinfo();
        }

        private void minToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.m30; Updateinfo();
        }

        private void hToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.h1; Updateinfo();
        }

        private void hToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.h4; Updateinfo();
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeperiodtype = CommonLab.TimePeriodType.d1; Updateinfo();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updateinfo();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
