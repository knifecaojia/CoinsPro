using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar.Charts;
using DevComponents.DotNetBar.Charts.Style;
using KFCC.ExchangeInterface;

namespace Hedge
{
    public partial class TradingPairControl : UserControl
    {
        string tradingpair;
        private ChartControl _ChartControl;
        private ChartXy _ChartXy;
        private double _StartMilliseconds;
        Dictionary<string, ChartSeries> _TimeSeries = new Dictionary<string, ChartSeries>();
        Dictionary<string, IExchanges> Exchanges;

        private CommonLab.TradePair Tp;
        public TradingPairControl(CommonLab.TradePair _tp, Dictionary<string, IExchanges> exchanges)
        {
            Tp = _tp;
            Exchanges = exchanges;
            //SuspendLayout();
            InitializeComponent();
            _StartMilliseconds = DateTime.Now.TimeOfDay.TotalMilliseconds;
            //SetupChart();
            //ResumeLayout(false);
            //chartControl1.ChartPanel.ChartContainers.Clear();
            //_ChartControl.Dock = DockStyle.Fill;
            //AddChart();
            
            SetupSeries();
            chartControl1.Dock = DockStyle.Fill;
            chartControl1.ChartPanel.ChartContainers[0].Titles[0].Text = Tp.FromSymbol+"-"+Tp.ToSymbol;
        }
        /// <summary>
        /// Initializes the control chart.
        /// </summary>
        private void SetupChart()
        {
            _ChartControl = new ChartControl();

            _ChartControl.Name = "RealTimePlot";
            _ChartControl.Location = new Point(12, 12);
            _ChartControl.Size = new Size(690, 460);
            _ChartControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            
            AddChart();

            
        }
        private void AddChart()
        {
            // Create a new ChartXy.

            ChartXy chartXy = new ChartXy();

            _ChartXy = chartXy;

            chartXy.Name = "RealTime";
            chartXy.MinContentSize = new Size(300, 300);

            // We want to display the plot a a straight line, with points unsorted.

            chartXy.ChartLineDisplayMode =
                ChartLineDisplayMode.DisplayLine | ChartLineDisplayMode.DisplayUnsorted;

            // Setup various styles for the chart...
            SetupChartStyle(chartXy);
            SetupContainerStyle(chartXy);
            SetupChartAxes(chartXy);
            SetupChartLegend(chartXy);
            SetupSeries();

        }
        #region SetupChartAxes

        /// <summary>
        /// Sets up the chart axes.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupChartAxes(ChartXy chartXy)
        {
            // X Axis

            ChartAxis axis = chartXy.AxisX;
            axis.MinorTickmarks.TickmarkCount = 0;
            axis.AxisMargins = 0;
            axis.GridSpacing = 60;

            axis.MajorGridLines.GridLinesVisualStyle.LineColor = Color.Gainsboro;
            axis.MinorGridLines.GridLinesVisualStyle.LineColor = Color.WhiteSmoke;

            // Y Axis

            axis = chartXy.AxisY;

            axis.AxisAlignment = AxisAlignment.Far;
            //axis.MinorTickmarks.TickmarkCount = 0;
            //axis.GridSpacing = 50;
            
            axis.MajorGridLines.GridLinesVisualStyle.LineColor = Color.Gainsboro;
            axis.MinorGridLines.GridLinesVisualStyle.LineColor = Color.WhiteSmoke;

            // Display the alternate background.

            axis.ChartAxisVisualStyle.AlternateBackground = new Background(Color.FromArgb(250, 250, 250));

            axis.UseAlternateBackground = true;
        }

        #endregion

        #region SetupChartStyle

        /// <summary>
        /// Sets up the chart style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupChartStyle(ChartXy chartXy)
        {
            ChartXyVisualStyle xystyle = chartXy.ChartVisualStyle;

            xystyle.Background = new Background(Color.White);
            xystyle.BorderThickness = new Thickness(1);
            xystyle.BorderColor = new BorderColor(Color.Black);

            xystyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);
        }

        #endregion

        #region SetupContainerStyle

        /// <summary>
        /// Sets up the chart's container style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupContainerStyle(ChartXy chartXy)
        {
            ContainerVisualStyle dstyle = chartXy.ContainerVisualStyles.Default;

            dstyle.Background = new Background(Color.White);
            dstyle.BorderColor = new BorderColor(Color.DimGray);
            dstyle.BorderThickness = new Thickness(1);

            dstyle.DropShadow.Enabled = Tbool.True;
            dstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(6);
        }

        #endregion

        #region SetupChartLegend

        /// <summary>
        /// Sets up the Legend style.
        /// </summary>
        /// <param name="chartXy"></param>
        private void SetupChartLegend(ChartXy chartXy)
        {
            ChartLegend legend = chartXy.Legend;

            legend.Placement = Placement.Outside;
            legend.Alignment = Alignment.TopCenter;
            legend.AlignVerticalItems = true;
            legend.Direction = Direction.LeftToRight;

            ChartLegendVisualStyle lstyle = legend.ChartLegendVisualStyles.Default;

            lstyle.BorderThickness = new Thickness(1);
            lstyle.BorderColor = new BorderColor(Color.Crimson);

            lstyle.Margin = new DevComponents.DotNetBar.Charts.Style.Padding(8);
            lstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);

            lstyle.Background = new Background(Color.FromArgb(200, Color.White));
        }

        #endregion
        private void SetupSeries()
        {
            ChartXy chartXy = (ChartXy)chartControl1.ChartPanel.ChartContainers[0];
            _ChartXy = chartXy;
            SetupChartStyle(chartXy);
            SetupContainerStyle(chartXy);
            SetupChartAxes(chartXy);
            SetupChartLegend(chartXy);
            chartXy.ChartSeries.Clear();
            foreach (var item in Exchanges)
            {
                ChartSeries series = new ChartSeries(item.Key, SeriesType.Line);

                RegressionLine rl = new RegressionLine("RegLine");

                rl.ShowInLegend = false;
                rl.Visible = false;

                series.ChartIndicators.Add(rl);

                chartXy.ChartSeries.Add(series);
                _TimeSeries.Add(item.Key,series);
                item.Value.DepthEvent += Value_DepthEvent; //+= Value_TickerEvent;
            }
    
        }

        private void Value_DepthEvent(object sender, CommonLab.Depth d, CommonLab.EventTypes et,  CommonLab.TradePair tp)
        {
            if (!this.Tp.Compare(tp))
                return;
            double msecs = (DateTime.Now.TimeOfDay.TotalMilliseconds - _StartMilliseconds)/1000;
            try
            {
                _TimeSeries[((IExchanges)sender).Name].SeriesPoints.Add(new SeriesPoint(msecs, d.Asks[0].Price));
                if (_ChartXy != null && msecs > 1000)
                    _ChartXy.AxisX.MinValue = msecs - 1000;
            }
            catch
            { }
        }

        private void Value_TickerEvent(object sender, CommonLab.Ticker t, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            if (!this.Tp.Compare(tp))
                return;
            double msecs = DateTime.Now.TimeOfDay.TotalMilliseconds - _StartMilliseconds;
            _TimeSeries[((IExchanges)sender).Name].SeriesPoints.Add(new SeriesPoint(msecs, t.Buy));
        }

        private void chartControl1_Click(object sender, EventArgs e)
        {

        }
    }
        

}
