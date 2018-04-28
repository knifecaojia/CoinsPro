using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinacneETF
{
    public partial class TradingInfo_Symbol : UserControl
    {
        public string SymbolQuoteKey;
        public TradingInfo_Symbol(string symbolkey)
        {
            InitializeComponent();
            SymbolQuoteKey = symbolkey;
            Config.UpdateTickerEvent += Config_UpdateTickerEvent;
        }
        delegate void UpdateDataGDV(List<TradingSymbol> symbols);
        private void Config_UpdateTickerEvent()
        {
            UpdateDGV(Config.Exchange.SymbolsClassByQuoteAsset[SymbolQuoteKey]);
        }
        private void UpdateDGVMethod(List<TradingSymbol> symbols)
        {
            //dataGridView1.Rows.Clear();
            bool find = false;
            for (int i = 0; i < symbols.Count; i++)
            {
                //dataGridView1.Rows.Add();
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    if (dataGridView1.Rows[j].Cells[0].Value.ToString() == symbols[i].BaseAsset)
                    {
                        find = true;
                        //dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = symbols[i].BaseAsset;
                        //dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = false;
                        dataGridView1.Rows[j].Cells[2].Value = symbols[i].Ticker.Buy;
                        dataGridView1.Rows[j].Cells[3].Value = symbols[i].Ticker.BuyQuantity;
                        dataGridView1.Rows[j].Cells[4].Value = symbols[i].Ticker.Sell;
                        dataGridView1.Rows[j].Cells[5].Value = symbols[i].Ticker.SellQuantity;
                        dataGridView1.Rows[j].Cells[6].Value = symbols[i].Ticker.PriceChange;
                        dataGridView1.Rows[j].Cells[7].Value = symbols[i].Ticker.PriceChangePct;
                        dataGridView1.Rows[j].Cells[8].Value = symbols[i].Ticker.Open;
                        dataGridView1.Rows[j].Cells[9].Value = symbols[i].Ticker.High;
                        dataGridView1.Rows[j].Cells[10].Value = symbols[i].Ticker.Low;
                        dataGridView1.Rows[j].Cells[11].Value = symbols[i].Ticker.VolumeBase;
                        dataGridView1.Rows[j].Cells[12].Value = symbols[i].Ticker.Volume;

                    }
                }
                if (find)
                    continue;
                else
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = symbols[i].BaseAsset;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = false;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value = symbols[i].Ticker.Buy;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value = symbols[i].Ticker.BuyQuantity;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[4].Value = symbols[i].Ticker.Sell;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Value = symbols[i].Ticker.SellQuantity;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].Value = symbols[i].Ticker.PriceChange;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[7].Value = symbols[i].Ticker.PriceChangePct;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[8].Value = symbols[i].Ticker.Open;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[9].Value = symbols[i].Ticker.High;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[10].Value = symbols[i].Ticker.Low;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[11].Value = symbols[i].Ticker.VolumeBase;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[12].Value = symbols[i].Ticker.Volume;
                }
            }
        }
        private void UpdateDGV(List<TradingSymbol> symbols)
        {
            if (dataGridView1.InvokeRequired)
            {
                UpdateDataGDV upd = new UpdateDataGDV(UpdateDGVMethod);
                dataGridView1.Invoke(upd, new object[] { (object)symbols });
            }
        }
    }
}
