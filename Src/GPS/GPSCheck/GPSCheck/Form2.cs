using Common;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GPSCheck
{
	public class Form2 : Form
	{
		private IContainer components = null;

		private SplitContainer splitContainer1;

		private PictureBox pictureBox1;

		private SplitContainer splitContainer2;

		private ToolStrip toolStrip1;

		private ToolStripLabel toolStripLabel1;

		private ToolStrip toolStrip2;

		private ToolStripLabel toolStripLabel2;

		private DataGridView dataGridView1;

		private DataGridViewTextBoxColumn Column1;

		private DataGridViewTextBoxColumn Column2;

		private DataGridViewTextBoxColumn Column3;

		private DataGridViewTextBoxColumn Column4;

		private DataGridView dataGridView2;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

		public Form2(Device dev, Image img)
		{
			InitializeComponent();
			Text = dev.Name + "分析结果窗口";
			pictureBox1.Image = img;
			for (int i = 0; i < dev.AnyResault.Count; i++)
			{
				int count;
				DateTime dateTime;
				if (dev.AnyResault[i].type == MovingType.Run)
				{
					dataGridView1.Rows.Add();
					DataGridViewCell dataGridViewCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
					count = dataGridView1.Rows.Count;
					dataGridViewCell.Value = count.ToString();
					dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[1].Value = dev.AnyResault[i].time.ToString();
					DataGridViewCell dataGridViewCell2 = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2];
					dateTime = dev.AnyResault[i].time + dev.AnyResault[i].timespan;
					dataGridViewCell2.Value = dateTime.ToString();
					dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value = dev.AnyResault[i].distance;
				}
				else
				{
					dataGridView2.Rows.Add();
					DataGridViewCell dataGridViewCell3 = dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[0];
					count = dataGridView2.Rows.Count;
					dataGridViewCell3.Value = count.ToString();
					dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[1].Value = dev.AnyResault[i].time.ToString();
					DataGridViewCell dataGridViewCell4 = dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[2];
					dateTime = dev.AnyResault[i].time + dev.AnyResault[i].timespan;
					dataGridViewCell4.Value = dateTime.ToString();
					dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[3].Value = dev.AnyResault[i].address;
				}
			}
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
			splitContainer1 = new SplitContainer();
			pictureBox1 = new PictureBox();
			splitContainer2 = new SplitContainer();
			toolStrip1 = new ToolStrip();
			toolStripLabel1 = new ToolStripLabel();
			toolStrip2 = new ToolStrip();
			toolStripLabel2 = new ToolStripLabel();
			dataGridView1 = new DataGridView();
			dataGridView2 = new DataGridView();
			Column1 = new DataGridViewTextBoxColumn();
			Column2 = new DataGridViewTextBoxColumn();
			Column3 = new DataGridViewTextBoxColumn();
			Column4 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
			((ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((ISupportInitialize)pictureBox1).BeginInit();
			((ISupportInitialize)splitContainer2).BeginInit();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			toolStrip1.SuspendLayout();
			toolStrip2.SuspendLayout();
			((ISupportInitialize)dataGridView1).BeginInit();
			((ISupportInitialize)dataGridView2).BeginInit();
			SuspendLayout();
			splitContainer1.Dock = DockStyle.Fill;
			splitContainer1.Location = new Point(0, 0);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Panel1.Controls.Add(pictureBox1);
			splitContainer1.Panel2.Controls.Add(splitContainer2);
			splitContainer1.Size = new Size(1387, 712);
			splitContainer1.SplitterDistance = 930;
			splitContainer1.TabIndex = 0;
			pictureBox1.BorderStyle = BorderStyle.FixedSingle;
			pictureBox1.Dock = DockStyle.Fill;
			pictureBox1.Location = new Point(0, 0);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(930, 712);
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			splitContainer2.Dock = DockStyle.Fill;
			splitContainer2.Location = new Point(0, 0);
			splitContainer2.Name = "splitContainer2";
			splitContainer2.Orientation = Orientation.Horizontal;
			splitContainer2.Panel1.Controls.Add(dataGridView1);
			splitContainer2.Panel1.Controls.Add(toolStrip1);
			splitContainer2.Panel2.Controls.Add(dataGridView2);
			splitContainer2.Panel2.Controls.Add(toolStrip2);
			splitContainer2.Size = new Size(453, 712);
			splitContainer2.SplitterDistance = 371;
			splitContainer2.TabIndex = 0;
			toolStrip1.Items.AddRange(new ToolStripItem[1]
			{
				toolStripLabel1
			});
			toolStrip1.Location = new Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new Size(453, 25);
			toolStrip1.TabIndex = 0;
			toolStrip1.Text = "toolStrip1";
			toolStripLabel1.Name = "toolStripLabel1";
			toolStripLabel1.Size = new Size(80, 22);
			toolStripLabel1.Text = "设备行驶记录";
			toolStrip2.Items.AddRange(new ToolStripItem[1]
			{
				toolStripLabel2
			});
			toolStrip2.Location = new Point(0, 0);
			toolStrip2.Name = "toolStrip2";
			toolStrip2.Size = new Size(453, 25);
			toolStrip2.TabIndex = 1;
			toolStrip2.Text = "toolStrip2";
			toolStripLabel2.Name = "toolStripLabel2";
			toolStripLabel2.Size = new Size(80, 22);
			toolStripLabel2.Text = "设备停止记录";
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Columns.AddRange(Column1, Column2, Column3, Column4);
			dataGridView1.Dock = DockStyle.Fill;
			dataGridView1.Location = new Point(0, 25);
			dataGridView1.MultiSelect = false;
			dataGridView1.Name = "dataGridView1";
			dataGridView1.ReadOnly = true;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.RowTemplate.Height = 23;
			dataGridView1.Size = new Size(453, 346);
			dataGridView1.TabIndex = 1;
			dataGridView2.AllowUserToAddRows = false;
			dataGridView2.AllowUserToDeleteRows = false;
			dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView2.Columns.AddRange(dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4);
			dataGridView2.Dock = DockStyle.Fill;
			dataGridView2.Location = new Point(0, 25);
			dataGridView2.MultiSelect = false;
			dataGridView2.Name = "dataGridView2";
			dataGridView2.ReadOnly = true;
			dataGridView2.RowHeadersVisible = false;
			dataGridView2.RowTemplate.Height = 23;
			dataGridView2.Size = new Size(453, 312);
			dataGridView2.TabIndex = 2;
			Column1.FillWeight = 20f;
			Column1.HeaderText = "ID";
			Column1.Name = "Column1";
			Column1.ReadOnly = true;
			Column2.HeaderText = "StartTime";
			Column2.Name = "Column2";
			Column2.ReadOnly = true;
			Column3.HeaderText = "EndTime";
			Column3.Name = "Column3";
			Column3.ReadOnly = true;
			Column4.HeaderText = "Distance";
			Column4.Name = "Column4";
			Column4.ReadOnly = true;
			dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewTextBoxColumn1.FillWeight = 20f;
			dataGridViewTextBoxColumn1.HeaderText = "ID";
			dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			dataGridViewTextBoxColumn1.ReadOnly = true;
			dataGridViewTextBoxColumn1.Width = 42;
			dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewTextBoxColumn2.HeaderText = "StartTime";
			dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			dataGridViewTextBoxColumn2.ReadOnly = true;
			dataGridViewTextBoxColumn2.Width = 84;
			dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewTextBoxColumn3.HeaderText = "EndTime";
			dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			dataGridViewTextBoxColumn3.ReadOnly = true;
			dataGridViewTextBoxColumn3.Width = 72;
			dataGridViewTextBoxColumn4.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewTextBoxColumn4.FillWeight = 180f;
			dataGridViewTextBoxColumn4.HeaderText = "Address";
			dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
			dataGridViewTextBoxColumn4.ReadOnly = true;
			dataGridViewTextBoxColumn4.Width = 72;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1387, 712);
			base.Controls.Add(splitContainer1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Name = "Form2";
			Text = "Form2";
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			((ISupportInitialize)pictureBox1).EndInit();
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel1.PerformLayout();
			splitContainer2.Panel2.ResumeLayout(false);
			splitContainer2.Panel2.PerformLayout();
			((ISupportInitialize)splitContainer2).EndInit();
			splitContainer2.ResumeLayout(false);
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			toolStrip2.ResumeLayout(false);
			toolStrip2.PerformLayout();
			((ISupportInitialize)dataGridView1).EndInit();
			((ISupportInitialize)dataGridView2).EndInit();
			ResumeLayout(false);
		}
	}
}
