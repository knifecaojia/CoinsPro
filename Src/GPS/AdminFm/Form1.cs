using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminFm
{
 
    public partial class Form1 : Form
    {
        DataTable Group;
        public Form1()
        {
            InitializeComponent();
            Group = GetDataTable(0, "", "");
            List<TreeNode> list = new List<TreeNode>();
            DataRow[] array = Group.Select("father_category_id='36'");
            foreach (DataRow dataRow in array)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = dataRow["category_name"].ToString();
                treeNode.Tag = dataRow["category_id"].ToString();
                treeNode.Nodes.AddRange(GetChildTN(treeNode.Tag.ToString()).ToArray());
                list.Add(treeNode);
            }
            treeView1.Nodes.AddRange(list.ToArray());
          
            treeView1.ExpandAll();
        }
        private List<TreeNode> GetChildTN(string pid)
        {
            List<TreeNode> list = new List<TreeNode>();
            DataRow[] array = Group.Select("father_category_id='" + pid + "'");
            if (array.Length != 0)
            {
                DataRow[] array2 = array;
                foreach (DataRow dataRow in array2)
                {
                    TreeNode treeNode = new TreeNode();
                    treeNode.Text = dataRow["category_name"].ToString();
                    treeNode.Tag = dataRow["category_id"].ToString();
                    treeNode.Nodes.AddRange(GetChildTN(treeNode.Tag.ToString()).ToArray());
                   
                    list.Add(treeNode);
                }
            }
            return list;
        }
        public static DataTable GetDataTable(int type, string i, string keyid)
        {
            string text = "";
            DataTable result = null;
            switch (type)
            {
                case 0:
                    text = AnyFunction.HttpGet("http://116.114.101.238:8883/data.aspx?action=group", "");
                    break;
                case 1:
                    text = AnyFunction.HttpGet("http://116.114.101.238:8883/data.aspx?action=TER", "");
                    break;
                case 2:
                    text = AnyFunction.HttpGet("http://116.114.101.238:8883/data.aspx?action=TRA&keyid=" + keyid + "&i=" + i, "");
                    break;
            }
            if (text.Length > 0)
            {
                result = JsonConvert.DeserializeObject<DataTable>(text);
            }
            return result;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Level == 0)
                {
                    foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                    {
                        bool flag = false;
                        for (int i = 0; i < listBox2.Items.Count; i++)
                        {
                            if (tn.Text == listBox2.Items[i].ToString())
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            listBox2.Items.Add(tn.Text);
                        }
                    }
                }
                else
                {
                    bool flag = false;
                    for (int i = 0; i < listBox2.Items.Count; i++)
                    {
                        if (treeView1.SelectedNode.Text == listBox2.Items[i].ToString())
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        listBox2.Items.Add(treeView1.SelectedNode.Text);
                    }

                }
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 2)
            {
                MessageBox.Show("用户名长度必须大于2位");
                return;
            }
            else if (textBox2.Text.Length < 6)
            {
                MessageBox.Show("密码长度必须大于6位");
                return;
            }
            string username = textBox1.Text;
            string password = textBox2.Text;
            string str = "";
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                if (i < listBox2.Items.Count - 1)
                {
                    str +=  listBox2.Items[i].ToString() + "|";
                }
                else
                {
                    str += listBox2.Items[i].ToString();
                }
            }
            StreamWriter sw = new StreamWriter(username+".txt", false, System.Text.Encoding.UTF8);
            sw.WriteLine(password);
            sw.WriteLine(str);
            sw.Close();
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + System.Windows.Forms.Application.StartupPath;
            System.Diagnostics.Process.Start(psi);
        }
    }
}
