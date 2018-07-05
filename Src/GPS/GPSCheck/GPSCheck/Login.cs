using Common;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GPSCheck
{
	public class Login : Form
	{
		private IContainer components = null;

		private Label label1;

		private TextBox textBox1;

		private Button button1;

		private Label label2;

		private TextBox textBox2;

		private Button button2;

		public Login()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (textBox1.Text.Length < 2)
			{
				MessageBox.Show("用户名长度必须大于2位");
			}
			else if (textBox2.Text.Length < 6)
			{
				MessageBox.Show("密码长度必须大于6位");
			}
			else
			{
				GlobalVar.groupstr = Loginm(textBox1.Text, textBox2.Text);
				if (GlobalVar.groupstr.IndexOf(textBox2.Text) == 0)
				{
					GlobalVar.username = textBox1.Text;
					base.DialogResult = DialogResult.OK;
					Close();
				}
				else
				{
					MessageBox.Show("用户名或密码不正确，请重新输入！");
				}
			}
		}
        public string Loginm(string username, string password)
        {
            string text = "";
            string raw = "";
            try
            {
                text= Common.AnyFunction.HttpGet("http://116.114.101.238:8883/gps/" + username + ".txt", "");
                raw = text;
                text = text.Split('\n')[1];
                text = text.Replace("\r", "");
                GlobalVar.GroupsList = new System.Collections.Generic.List<string>();
                GlobalVar.GroupsList.AddRange(text.Split('|'));
                return raw;
            }
            catch
            {
                return "error";
            }
        }
        private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			label1 = new Label();
			textBox1 = new TextBox();
			button1 = new Button();
			label2 = new Label();
			textBox2 = new TextBox();
			button2 = new Button();
			SuspendLayout();
			label1.AutoSize = true;
			label1.Location = new Point(27, 25);
			label1.Name = "label1";
			label1.Size = new Size(53, 12);
			label1.TabIndex = 0;
			label1.Text = "用户名：";
			textBox1.Location = new Point(86, 22);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(165, 21);
			textBox1.TabIndex = 1;
			button1.Location = new Point(108, 92);
			button1.Name = "button1";
			button1.Size = new Size(60, 23);
			button1.TabIndex = 3;
			button1.Text = "登陆";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			label2.AutoSize = true;
			label2.Location = new Point(27, 58);
			label2.Name = "label2";
			label2.Size = new Size(53, 12);
			label2.TabIndex = 3;
			label2.Text = "密  码：";
			textBox2.Location = new Point(86, 55);
			textBox2.Name = "textBox2";
			textBox2.PasswordChar = '*';
			textBox2.Size = new Size(165, 21);
			textBox2.TabIndex = 2;
			button2.Location = new Point(191, 92);
			button2.Name = "button2";
			button2.Size = new Size(60, 23);
			button2.TabIndex = 4;
			button2.Text = "取消";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(284, 137);
			base.ControlBox = false;
			base.Controls.Add(button2);
			base.Controls.Add(textBox2);
			base.Controls.Add(label2);
			base.Controls.Add(button1);
			base.Controls.Add(textBox1);
			base.Controls.Add(label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Name = "Login";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			Text = "Login";
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
