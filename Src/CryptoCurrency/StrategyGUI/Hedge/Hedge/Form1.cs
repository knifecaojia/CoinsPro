using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KFCC.ExchangeInterface;
using CommonLab;

namespace Hedge
{
    public partial class Form1 : Form
    {
        Dictionary<string, TradePair> WatchingList;
        Dictionary<string, IExchanges> Exchanges;
        Dictionary<string, TabPage> TabPages;
        public Form1()
        {
            InitializeComponent();
            WatchingList = new Dictionary<string, TradePair>();
            Exchanges = new Dictionary<string, IExchanges>();
            TabPages = new Dictionary<string, TabPage>();
            LoadExchanges();
            InitExchangesTab();
            TradingPairControl tpc = new TradingPairControl("FUCK");
            tpc.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(tpc);
        }
        private void InitExchangesTab()
        {
            foreach (var item in Exchanges)
            {
                TabPage t = new TabPage(item.Value.Name);
                SplitContainer sc = new SplitContainer();
                t.Controls.Add(sc);
                tabControl1.TabPages.Add(t);
                TabPages.Add(item.Key, t);

                ExchangeSetup es = new ExchangeSetup(item.Value);
                sc.Panel1.Controls.Add(es);
                sc.SplitterMoving += Sc_SplitterMoving;
                es.Dock = DockStyle.Fill;
                sc.FixedPanel = FixedPanel.Panel1;
                sc.BorderStyle = BorderStyle.FixedSingle;
                sc.Orientation = Orientation.Horizontal;
                sc.SplitterDistance = 160;
                sc.Layout += Sc_Layout;
                sc.Dock = DockStyle.Fill;
                sc.Name = "sc";


            }
        }

        private void Sc_Layout(object sender, LayoutEventArgs e)
        {
            ((SplitContainer)sender).SplitterDistance = 160;
        }

        private void Sc_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            toolStripStatusLabel1.Text = e.SplitY.ToString();
        }

        private void LoadExchanges()
        {
            List<string> pluginpath = FindPlugin();



            foreach (string filename in pluginpath)
            {
                try
                {
                    //获取文件名
                    string asmfile = filename;
                    string asmname = Path.GetFileNameWithoutExtension(asmfile);
                    if (asmname != string.Empty)
                    {
                        // 利用反射,构造DLL文件的实例
                        Assembly asm = Assembly.LoadFile(asmfile);
                        //利用反射,从程序集(DLL)中,提取类,并把此类实例化
                        Type[] t = asm.GetExportedTypes();
                        foreach (Type type in t)
                        {
                            if (type.GetInterface("IExchanges") != null)
                            {
                                IExchanges exchange = (IExchanges)Activator.CreateInstance(type);
                                Exchanges.Add(exchange.Name, exchange);
                                //Console.Write(exchange.Name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fsy, tsy;
            fsy = comboBox1.Text.ToLower();
            tsy = comboBox2.Text.ToLower();
            if (string.IsNullOrEmpty(fsy) || string.IsNullOrEmpty(tsy))
            {
                MessageBox.Show("CryptoCurrency symbol can't be NULL!");
                return;
            }
            if (fsy == tsy)
            {
                MessageBox.Show("Fromsymbol  and Tosymbol can't be same!");
                return;
            }
            string tradingpair = fsy + "/" + tsy;
            if (WatchingList.ContainsKey(tradingpair))
            {
                MessageBox.Show(tradingpair + " already in watching list!");
                return;
            }
            WatchingList.Add(tradingpair, new CommonLab.TradePair(fsy, tsy));
            listView1.Items.Add(tradingpair);


            //订阅
            foreach (var item in Exchanges)
            {
                item.Value.Subscribe(new CommonLab.TradePair(fsy, tsy), CommonLab.SubscribeTypes.WSS);
                Console c = new Console(item.Value);
                c.Dock = DockStyle.Fill;
                for (int i = 0; i < TabPages[item.Key].Controls.Count; i++)
                {
                    if (TabPages[item.Key].Controls[i].Name == "sc")
                    {
                        ((SplitContainer)TabPages[item.Key].Controls[i]).Panel2.Controls.Add(c);
                        break;
                    }
                }
            }
        }
        //查找所有插件的路径
        private List<string> FindPlugin()
        {
            List<string> pluginpath = new List<string>();
            try
            {
                //获取程序的基目录
                string path = AppDomain.CurrentDomain.BaseDirectory;
                //合并路径，指向插件所在目录。
                path = Path.Combine(path, "Plugins");
                foreach (string filename in Directory.GetFiles(path, "*.dll"))
                {
                    pluginpath.Add(filename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return pluginpath;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in Exchanges)
            {
                foreach (var it in WatchingList)
                {
                    try
                    {
                        item.Value.DisSubcribe(it.Value, SubscribeTypes.WSS);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
