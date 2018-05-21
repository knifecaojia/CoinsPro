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
        public double[] c7;
        public double[] c11;
        public double[] c12;
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
                        dataGridView1.Rows[j].Cells[7].Value = symbols[i].Ticker.Last;
                        dataGridView1.Rows[j].Cells[8].Value = symbols[i].Ticker.Open;
                        dataGridView1.Rows[j].Cells[9].Value = symbols[i].Ticker.High;
                        dataGridView1.Rows[j].Cells[10].Value = symbols[i].Ticker.Low;
                        dataGridView1.Rows[j].Cells[11].Value = symbols[i].Ticker.VolumeBase;
                        dataGridView1.Rows[j].Cells[12].Value = symbols[i].Ticker.Volume;
                        dataGridView1.Rows[j].Cells[13].Value = symbols[i].Symbol;

                    }
                }
                if (find)
                    continue;
                else
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value = symbols[i].BaseAsset;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = Config.strategyConfig.IsSymbolInETF(symbols[i].Symbol);
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value = symbols[i].Ticker.Buy;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value = symbols[i].Ticker.BuyQuantity;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[4].Value = symbols[i].Ticker.Sell;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Value = symbols[i].Ticker.SellQuantity;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].Value = symbols[i].Ticker.PriceChange;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[7].Value = symbols[i].Ticker.Last;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[8].Value = symbols[i].Ticker.Open;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[9].Value = symbols[i].Ticker.High;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[10].Value = symbols[i].Ticker.Low;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[11].Value = symbols[i].Ticker.VolumeBase;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[12].Value = symbols[i].Ticker.Volume;
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[13].Value = symbols[i].Symbol;
                }
            }
            c7 = new double[dataGridView1.Rows.Count];
            c11 = new double[dataGridView1.Rows.Count];
            c12 = new double[dataGridView1.Rows.Count];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                c7[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[7].Value);
                c11[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[11].Value);
                c12[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[12].Value);
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[1];
                Boolean flag = Convert.ToBoolean(checkCell.Value);

                checkCell.Value = !flag;
                Config.strategyConfig.UpdateSymbol(dataGridView1.Rows[e.RowIndex].Cells[13].Value.ToString(), !flag);
                Config.RaiseUpdateServiceStatusEvent(Color.DarkBlue, "监测:" + Config.strategyConfig.SelectedSymbols.Count + "种币");
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
           
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
    
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (c7 == null) return;
            DataGridViewCell ee = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (e.ColumnIndex == 7 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
            {
                if (e.ColumnIndex == 7 && ((Convert.ToDouble(ee.Value) - c7[e.RowIndex]) >= 0))
                {
                    ee.Style.ForeColor = Color.Red;
                }
                else if (e.ColumnIndex == 7 && ((Convert.ToDouble(ee.Value) - c7[e.RowIndex]) < 0))
                    ee.Style.ForeColor = Color.Green;
                if (e.ColumnIndex == 11 && ((Convert.ToDouble(ee.Value) - c11[e.RowIndex]) >= 0))
                {
                    ee.Style.ForeColor = Color.Red;
                }
                else if (e.ColumnIndex == 11 && ((Convert.ToDouble(ee.Value) - c11[e.RowIndex]) < 0))
                    ee.Style.ForeColor = Color.Green;
                if (e.ColumnIndex == 12 && ((Convert.ToDouble(ee.Value) - c12[e.RowIndex]) >= 0))
                {
                    ee.Style.ForeColor = Color.Red;
                }
                else if (e.ColumnIndex == 12 && ((Convert.ToDouble(ee.Value) - c12[e.RowIndex]) < 0))
                    ee.Style.ForeColor = Color.Green;
                //e.Handled = true;
            }
        }
    }
}
