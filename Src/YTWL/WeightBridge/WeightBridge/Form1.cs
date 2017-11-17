using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeightBridge
{
    public partial class Form1 : Form
    {
        public int LastID = 0;
        public int LastCarIndex = -1;
        const int ID = 100000;
        public string html_o = "";
        //初始化绑定默认关键词（此数据源可以从数据库取）
        List<string> listOnit = new List<string>();
        //输入key之后，返回的关键词
        List<string> listNew = new List<string>();
        public Form1()
        {
            InitializeComponent();
            label18.Text = dateTimePicker1.Value.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
            html_o=  File.ReadAllText("t.html");
            UpdateHtml();
            StreamReader sr = new StreamReader("cars.txt");
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                listOnit.Add(line);
            }
            this.comboBox1.Items.AddRange(listOnit.ToArray());
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double mz = Convert.ToDouble(textBox7.Text);
                double pz = Convert.ToDouble(textBox8.Text);

                textBox9.Text = (mz - pz).ToString("f2");
            }
            catch
            {

            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GaussianRNG g = new GaussianRNG();
            double d = Math.Abs(g.Next());
            Random ra = new Random(DateTime.Now.Second);
            DateTime pt = dateTimePicker1.Value.AddSeconds(ra.Next(3, 25));
            label18.Text = pt.ToString("yyyy-MM-dd HH:mm:ss");
            if (checkBox1.Checked)
            {
                if (LastID > 0)
                {
                    int add = (int)(d * 3 + 1);
                    textBox1.Text = "A" + pt.ToString("yyyyMMdd") + (ID + LastID + add).ToString().Substring(1);
                }
                else
                {
                    int add = (int)(d * 75 + 1);
                    textBox1.Text = "A" + pt.ToString("yyyyMMdd") + (ID  + add).ToString().Substring(1);
                }
                
            }
            
               
            

           
            if (checkBox4.Checked)
            {
                if (LastCarIndex == -1)
                {
                    LastCarIndex = 0;
                   
                }
                else
                {
                    if (LastCarIndex == listOnit.Count - 1)
                    {
                        LastCarIndex = 0;
                      
                    }
                    else
                    {
                        LastCarIndex++;
                       
                    }
                }
                comboBox1.SelectedIndex = LastCarIndex;
            }
            d = Math.Abs(g.Next());
            int i = (int)(d * 14400 + 3600);
            DateTime mt = dateTimePicker1.Value.AddSeconds(-i);
            label19.Text = i.ToString();
            textBox12.Text = mt.ToString("yyyy/MM/dd HH:mm:ss");
            UpdateHtml();
        }
        internal static double[] NormalDistribution()
        {
            Random rand = new Random();
            double[] y;
            double u1, u2, v1 = 0, v2 = 0, s = 0, z1 = 0, z2 = 0;
            while (s > 1 || s == 0)
            {
                u1 = rand.NextDouble();
                u2 = rand.NextDouble();
                v1 = 2 * u1 - 1;
                v2 = 2 * u2 - 1;
                s = v1 * v1 + v2 * v2;
            }
            z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
            z2 = Math.Sqrt(-2 * Math.Log(s) / s) * v2;
            y = new double[] { z1, z2 };
            return y; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            //string keyName = @"Software\Microsoft\Internet Explorer\PageSetup";
            //using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
            //{
            //    if (key != null)
            //    {
            //        string old_footer = key.GetValue("footer").ToString();
            //        string old_header = key.GetValue("header").ToString();
            //        key.SetValue("footer", "");
            //        key.SetValue("header", "");

            //        key.SetValue("footer", old_footer);
            //        key.SetValue("header", old_header);
            //    }
            //}
            webBrowser1.ShowPrintPreviewDialog();
            string lastid = textBox1.Text;
            if (lastid.Length > 5)
                lastid = lastid.Substring(lastid.Length - 5);
            int id = 0;
            int.TryParse(lastid, out id);
            LastID = id;


        }
        private void UpdateHtml()
        {
            string html = html_o;
            html = html.Replace("$1$",textBox1.Text);
            html = html.Replace("$2$", label18.Text);
            html = html.Replace("$3$", textBox2.Text);
            html = html.Replace("$4$", textBox6.Text);
            html = html.Replace("$5$", textBox11.Text);
            html = html.Replace("$6$", comboBox1.Text);
            html = html.Replace("$7$", textBox7.Text);
            html = html.Replace("$8$", textBox12.Text);
            html = html.Replace("$9$", textBox3.Text);
            html = html.Replace("$10$", textBox8.Text);
            html = html.Replace("$11$", dateTimePicker1.Value.ToString("yyyy/MM/dd HH:mm:ss"));
            html = html.Replace("$12$", textBox4.Text);
            html = html.Replace("$13$", textBox9.Text);
            html = html.Replace("$14$", textBox13.Text);
            html = html.Replace("$15$", textBox5.Text);
            html = html.Replace("$16$", textBox10.Text);
            html = html.Replace("$17$", textBox14.Text);
            webBrowser1.DocumentText = html;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double mz = Convert.ToDouble(textBox7.Text);
                double pz = Convert.ToDouble(textBox8.Text);

                textBox9.Text = (mz - pz).ToString();
            }
            catch
            {

            }
        }
        public static bool IsVehicleNumber(string vehicleNumber)
        {
            bool result = false;
            if (vehicleNumber.Length == 7)
            {
                string express = @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$";
                result = Regex.IsMatch(vehicleNumber, express);
            }
            return result;
        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double mz = 0; //Convert.ToDouble(textBox7.Text);
                double pz = 0; //Convert.ToDouble(textBox8.Text);
                double jz = 0;
                double.TryParse(textBox7.Text, out mz);
                double.TryParse(textBox8.Text, out pz);
                double.TryParse(textBox9.Text,out jz);
                GaussianRNG g = new GaussianRNG();

                double d = g.Next();
                if (jz > 0)
                {
                    
                        pz = 18 +2*d;
                    
                    
                   
                }
                mz = pz + jz;
                textBox7.Text = mz.ToString("f2");
                textBox8.Text= pz.ToString("f2");
                
            }
            catch
            {

            }
            UpdateHtml();
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            //清空combobox
            this.comboBox1.Items.Clear();
            //清空listNew
            listNew.Clear();
            //遍历全部备查数据
            bool flag = true;
            foreach (var item in listOnit)
            {
                if (item.Contains(this.comboBox1.Text))
                {
                    //符合，插入ListNew
                    listNew.Add(item);
                    flag = false;
                }
            }
            if (flag)
                listNew.Add(this.comboBox1.Text);
            try
            {
                //combobox添加已经查到的关键词
                this.comboBox1.Items.AddRange(listNew.ToArray());
                //设置光标位置，否则光标位置始终保持在第一列，造成输入关键词的倒序排列
                //this.comboBox1.SelectionStart = this.comboBox1.Text.Length;
                //保持鼠标指针原来状态，有时候鼠标指针会被下拉框覆盖，所以要进行一次设置。
                Cursor = Cursors.Default;
                //自动弹出下拉框
                this.comboBox1.DroppedDown = true;
                UpdateHtml();
            }
            catch
            { }
        }

       

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            if (!IsVehicleNumber(this.comboBox1.Text))
            {
                comboBox1.Focus();
                label20.Text = "格式？";
                return;
            }
            label20.Text = "";
            bool flag = true;
            foreach (var item in listOnit)
            {
                if (item.Contains(this.comboBox1.Text))
                {
                   
                    flag = false;
                    break;
                }
            }
            if (flag)
                listOnit.Add(this.comboBox1.Text);
            UpdateHtml();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPageSetupDialog();
        }
    }
    public class GaussianRNG
  {
    int iset;
    double gset;
    Random r1, r2;
    
    public GaussianRNG()
    {
      r1 = new Random(unchecked((int)DateTime.Now.Ticks));
      r2 = new Random(~unchecked((int)DateTime.Now.Ticks));
      iset = 0;
    }
    
    public double Next()
    {
      double fac, rsq, v1, v2;    
      if (iset == 0) {
        do {
          v1 = 2.0 * r1.NextDouble() - 1.0;
          v2 = 2.0 * r2.NextDouble() - 1.0;
          rsq = v1*v1 + v2*v2;
        } while (rsq >= 1.0 || rsq == 0.0);
        
        fac = Math.Sqrt(-2.0*Math.Log(rsq)/rsq);
        gset = v1*fac;
        iset = 1;
        return v2*fac;
      } else {
        iset = 0;
        return gset;
      }
    }
  }
}
