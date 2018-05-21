using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (this.txtpath.Text == "") 
            {
                MessageBox.Show("请输入文件夹路径");
                return;
            }
            if (this.txtName.Text == "") 
            {
                MessageBox.Show("请输入前缀名称");
                return;
            }
            RenameFile(this.txtpath.Text, this.txtName.Text);
        }

        public static void RenameFile(string ParentDir, string stringFront)
        {
            string[] files = Directory.GetFiles(ParentDir, "*.cs", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                string pathname = Path.GetDirectoryName(file);

                    filename = stringFront + filename;
                    FileInfo fi = new FileInfo(file);
                    fi.MoveTo(Path.Combine(pathname, filename));
            }
            string[] dirs = Directory.GetDirectories(ParentDir);
            foreach (string dir in dirs)
            {
                RenameFile(dir, stringFront);
            }
            MessageBox.Show("修改成功!");

        }  
    }
}
