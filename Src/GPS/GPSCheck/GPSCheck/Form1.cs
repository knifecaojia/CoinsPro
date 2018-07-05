using Common;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GPSCheck
{
	public class Form1 : Form
	{
		private DataTable Group = null;

		private DataTable Devices = null;

		private GeocodingProvider gp;

		private int TotalDevicesCount = 0;

		private int TotalSelectDevicesCount = 0;

		private int TotalDataReadDeviceCount = 0;

		private int TotalAnalysisedDevicesCount = 0;

		private List<Device> SelectedDevices = new List<Device>();

		private DateTime StartRunTime;

		private int RunState = 0;

		public DataTable ResaultDT;

		public string AnyDeviceName;

		public TimeSpan TotalTimeUsed;

		private int TotalReportDeviceCount = 0;

		private TimeSpan TotalReportTimeUsed;

		private DateTime StartRunReportTime;

		private List<object[]> lastRow = new List<object[]>();

		private int colindex = 0;

		private bool sort = true;

		private IContainer components = null;

		private SplitContainer splitContainer1;

		private SplitContainer splitContainer2;

		private TreeView treeView1;

		private Button button1;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel toolStripStatusLabel1;

		private DateTimePicker dateTimePicker2;

		private Label label2;

		private Label label1;

		private DateTimePicker dateTimePicker1;

		private Button button2;

		private ToolStripProgressBar toolStripProgressBar1;

		private System.Windows.Forms.Timer timer1;

		private SplitContainer splitContainer3;

		private GMapControl gMapControl1;

		private DataGridView dataGridView1;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem exportxlsxToolStripMenuItem;

		private Button button3;

		public Form1()
		{
			InitializeComponent();
			Control.CheckForIllegalCrossThreadCalls = false;
			gMapControl1.CacheLocation = Application.StartupPath;
			gMapControl1.MapProvider = GMapProviders.AMap;
			gMapControl1.Manager.Mode = AccessMode.ServerAndCache;
			gMapControl1.MinZoom = 1;
			gMapControl1.MaxZoom = 23;
			gMapControl1.Zoom = 15.0;
			gMapControl1.ShowCenter = false;
			gMapControl1.DragButton = MouseButtons.Left;
			gMapControl1.Position = new PointLatLng(40.657, 109.84);
			gp = GMapProviders.GoogleChinaMap;
			ResaultDT = new DataTable();
			DataColumn column = new DataColumn("ID", Type.GetType("System.Int32"));
			DataColumn column2 = new DataColumn("GruopName", Type.GetType("System.String"));
			DataColumn column3 = new DataColumn("Device", Type.GetType("System.String"));
			DataColumn column4 = new DataColumn("RawPoints", Type.GetType("System.Int32"));
			DataColumn column5 = new DataColumn("Distance", Type.GetType("System.Double"));
			DataColumn column6 = new DataColumn("Stops", Type.GetType("System.Int32"));
			ResaultDT.Columns.Add(column);
			ResaultDT.Columns.Add(column2);
			ResaultDT.Columns.Add(column3);
			ResaultDT.Columns.Add(column4);
			ResaultDT.Columns.Add(column5);
			ResaultDT.Columns.Add(column6);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			treeView1.Nodes.Clear();
			TotalDevicesCount = 0;
			Group = GetDataTable(0, "", "");
			Devices = GetDataTable(1, "", "");
			List<TreeNode> list = new List<TreeNode>();
			DataRow[] array = Group.Select("father_category_id='36'");
			DataRow[] array2 = array;
			foreach (DataRow dataRow in array2)
			{
				TreeNode treeNode = new TreeNode();
				treeNode.Text = dataRow["category_name"].ToString();
				treeNode.Tag = dataRow["treecode"].ToString();
				treeNode.Nodes.AddRange(GetChildTN(dataRow["category_id"].ToString()).ToArray());
				list.Add(treeNode);
			}
			treeView1.Nodes.AddRange(list.ToArray());
			toolStripStatusLabel1.Text = "共有设备:" + TotalDevicesCount.ToString();
			treeView1.ExpandAll();
			dateTimePicker1.Enabled = true;
			dateTimePicker2.Enabled = true;
			button2.Enabled = true;
		}
        public static string GetDataTable(string s,string e, string keyid)
        {
            string text = "";
            text = AnyFunction.HttpGet("http://116.114.101.238:8883/data.aspx?action=TRA&keyid=" + keyid + "&s=" + s+"&e="+e,"");
            try
            {
                return text;
            }
            catch
            {
                return null;
            }
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

        private List<TreeNode> GetDevice(string pid)
		{
			List<TreeNode> list = new List<TreeNode>();
			DataRow[] array = Devices.Select("category_id='" + pid + "'");
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					TreeNode treeNode = new TreeNode();
					if (dataRow["device_name"].ToString().IndexOf(" ") > 0)
					{
						treeNode.Text = dataRow["device_name"].ToString().Split(' ')[1];
					}
					else
					{
						treeNode.Text = dataRow["device_name"].ToString();
					}
					treeNode.Tag = dataRow["device_id"].ToString();
					treeNode.ToolTipText = dataRow["device_id"].ToString();
					TotalDevicesCount++;
					list.Add(treeNode);
				}
			}
			return list;
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
					treeNode.Tag = dataRow["treecode"].ToString();
					treeNode.Nodes.AddRange(GetChildTN(dataRow["category_id"].ToString()).ToArray());
                    //var gname = GlobalVar.GroupsList.Select(x => x.Equals(dataRow["category_name"].ToString())).ToList();
                    foreach (string item in GlobalVar.GroupsList)
                    {
                        if (item.Equals(dataRow["category_name"].ToString()))
                        {
                            treeNode.Nodes.AddRange(GetDevice(treeNode.Tag.ToString()).ToArray());
                            break;
                        }
                    }
                   
                   
					list.Add(treeNode);
				}
			}
			return list;
		}

		private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Action != 0)
			{
				CheckAllChildNodes(e.Node, e.Node.Checked);
				bool @checked = true;
				if (e.Node.Parent != null)
				{
					for (int i = 0; i < e.Node.Parent.Nodes.Count; i++)
					{
						if (!e.Node.Parent.Nodes[i].Checked)
						{
							@checked = false;
						}
					}
					e.Node.Parent.Checked = @checked;
				}
			}
		}

		public void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
		{
			foreach (TreeNode node in treeNode.Nodes)
			{
				node.Checked = nodeChecked;
				if (node.Nodes.Count > 0)
				{
					CheckAllChildNodes(node, nodeChecked);
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			DateTime value = dateTimePicker2.Value;
			int dayOfYear = value.DayOfYear;
			value = dateTimePicker1.Value;
			if (dayOfYear < value.DayOfYear)
			{
				MessageBox.Show("结束日期必须大于开始日期");
			}
			else if ((dateTimePicker2.Value - dateTimePicker1.Value).TotalDays > 3.0)
			{
				MessageBox.Show("系统限制，分析日期间隔不能大于3天");
			}
			else
			{
				value = dateTimePicker1.Value;
				int dayOfYear2 = value.DayOfYear;
				value = dateTimePicker2.Value;
				int dayOfYear3 = value.DayOfYear;
				TotalSelectDevicesCount = 0;
				TotalDataReadDeviceCount = 0;
				TotalAnalysisedDevicesCount = 0;
				SelectedDevices.Clear();
				toolStripProgressBar1.Visible = true;
				FindNode(treeView1.Nodes);
				TotalSelectDevicesCount = SelectedDevices.Count;
				if (TotalSelectDevicesCount == 0)
				{
					MessageBox.Show("没有选择设备！");
				}
				else
				{
					toolStripStatusLabel1.Text = "共有设备:" + TotalDevicesCount.ToString() + "台 待分析设备:" + TotalSelectDevicesCount.ToString() + "台";
					button2.Enabled = false;
					StartRunTime = DateTime.Now;
					timer1.Start();
					Thread thread = new Thread(Run);
					thread.Start();
				}
			}
		}

		private void Run()
		{
			RunState = 1;
			for (int i = 0; i < SelectedDevices.Count; i++)
			{
				try
				{
					DateTime value = dateTimePicker1.Value;
					int num = value.DayOfYear;
					while (true)
					{
						int num2 = num;
						value = dateTimePicker2.Value;
						if (num2 <= value.DayOfYear)
						{
							string jsonarrstr = GetDataTable(dateTimePicker1.Value.ToString("yyyyMMdd"),dateTimePicker2.Value.ToString("yyyyMMdd"), SelectedDevices[i].KeyID);
                            if (jsonarrstr.Length > 10)
                            {
                                JArray rawarray = JArray.Parse(jsonarrstr);
                                for (int j = 0; j < rawarray.Count; j++)
                                {
                                    JObject pt = JObject.Parse(rawarray[j].ToString());
                                    GPSPoint gpt = new GPSPoint();
                                    gpt.Direction = Convert.ToInt32(pt["d"].ToString());
                                    gpt.Lat= Convert.ToDouble(pt["y"].ToString());
                                    gpt.Lat_old = Convert.ToDouble(pt["y"].ToString());
                                    gpt.Lon = Convert.ToDouble(pt["x"].ToString());
                                    gpt.Lon_old = Convert.ToDouble(pt["x"].ToString());
                                    gpt.Speed = Convert.ToDouble(pt["s"].ToString());
                                    gpt.Time = Convert.ToDateTime(pt["t"].ToString());
                                    SelectedDevices[i].GpsPoints.Add(gpt);
                                }
                            }
							num++;
							continue;
						}
						break;
					}
				}
				catch
				{
				}
				TotalDataReadDeviceCount++;
			}
			RunState = 2;
			ResaultDT.Rows.Clear();
			for (int j = 0; j < SelectedDevices.Count; j++)
			{
				try
				{
					AnyDeviceName = SelectedDevices[j].Name;
					if (SelectedDevices[j].GpsPoints.Count < 2)
					{
						string[] obj2 = new string[5]
						{
							"http://apis.map.qq.com/ws/geocoder/v1/?location=",
							null,
							null,
							null,
							null
						};
						double num3 = SelectedDevices[j].LastLat;
						obj2[1] = num3.ToString();
						obj2[2] = ",";
						num3 = SelectedDevices[j].LastLon;
						obj2[3] = num3.ToString();
						obj2[4] = "&coord_type=1&key=LXDBZ-P2435-K6OI3-QMBPJ-P3A6V-F6BSW";
						string text = HttpGet(string.Concat(obj2), "");
						if (text.Length > 10)
						{
							JObject jObject = JObject.Parse(text);
							try
							{
								SelectedDevices[j].LastAddress = jObject["result"]["address"].ToString();
								string text2 = jObject["result"]["address_reference"]["crossroad"]["title"].ToString();
								double num4 = Convert.ToDouble(jObject["result"]["address_reference"]["crossroad"]["_distance"]);
								string text3 = jObject["result"]["address_reference"]["crossroad"]["_dir_desc"].ToString();
								if (text2.Length > 0)
								{
									Device device = SelectedDevices[j];
									device.LastAddress = device.LastAddress + "(" + text2 + " " + text3 + num4.ToString() + "米)";
								}
								Thread.Sleep(200);
							}
							catch
							{
							}
						}
					}
					SelectedDevices[j].AnyResault = AnyFunction.AnyPoints(SelectedDevices[j].GpsPoints, gp);
					DataRow dataRow = ResaultDT.NewRow();
					dataRow[0] = j + 1;
					dataRow[1] = SelectedDevices[j].GroupName;
					dataRow[2] = SelectedDevices[j].Name;
					dataRow[3] = SelectedDevices[j].GpsPoints.Count;
					double num5 = 0.0;
					int num6 = 0;
					for (int k = 0; k < SelectedDevices[j].AnyResault.Count; k++)
					{
						if (SelectedDevices[j].AnyResault[k].type == MovingType.Run)
						{
							num5 += SelectedDevices[j].AnyResault[k].distance;
							SelectedDevices[j].Distance += SelectedDevices[j].AnyResault[k].distance;
						}
						else
						{
							SelectedDevices[j].Stops++;
							num6++;
						}
					}
					dataRow[4] = Math.Round(num5, 2);
					dataRow[5] = num6;
					ResaultDT.Rows.Add(dataRow);
				}
				catch
				{
				}
				TotalAnalysisedDevicesCount++;
			}
		}

		private string HttpGet(string Url, string postDataStr)
		{
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url + ((postDataStr == "") ? "" : "?") + postDataStr);
				httpWebRequest.Method = "GET";
				httpWebRequest.ContentType = "text/html;charset=UTF-8";
				httpWebRequest.Timeout = 5000;
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				Stream responseStream = httpWebResponse.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
				string result = streamReader.ReadToEnd();
				streamReader.Close();
				responseStream.Close();
				return result;
			}
			catch
			{
				return "";
			}
		}

		private void FindNode(TreeNodeCollection Nodes)
		{
			foreach (TreeNode Node in Nodes)
			{
				if (Node.ToolTipText.Length > 0 && Node.Checked)
				{
					string toolTipText = Node.ToolTipText;
					DataRow[] array = Devices.Select("device_id='" + toolTipText + "'");
					if (array.Length == 1)
					{
						Device device = new Device();
						device.KeyID = toolTipText;
						device.Name = Node.Text;
						device.GroupID = Node.Parent.Tag.ToString();
						device.GroupName = Node.Parent.Text;
						device.IMEI = array[0]["device_imei"].ToString();
						//if (array[0]["TER_OLD_Y"] != DBNull.Value)
						//{
						//	device.LastLat = Convert.ToDouble(array[0]["TER_OLD_Y"]);
						//	device.LastLon = Convert.ToDouble(array[0]["TER_OLD_X"]);
						//	device.LastTime = Convert.ToDateTime(array[0]["TER_REFRESH_DATE"]);
						//}
						//else
						//{
						//	device.LastLat = 0.0;
						//	device.LastLon = 0.0;
						//	device.LastAddress = "ERROR";
						//}
						SelectedDevices.Add(device);
					}
				}
				if (Node.Nodes.Count > 0)
				{
					FindNode(Node.Nodes);
				}
			}
		}

		private void CountNode(TreeNodeCollection Nodes)
		{
			foreach (TreeNode Node in Nodes)
			{
				if (Node.ToolTipText.Length > 0 && Node.Checked)
				{
					string toolTipText = Node.ToolTipText;
					DataRow[] array = Devices.Select("device_id='" + toolTipText + "'");
					if (array.Length == 1)
					{
						TotalSelectDevicesCount++;
					}
				}
				if (Node.Nodes.Count > 0)
				{
					CountNode(Node.Nodes);
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (RunState == 1)
			{
				TotalTimeUsed = DateTime.Now - StartRunTime;
				toolStripStatusLabel1.Text = "共有:" + TotalDevicesCount.ToString() + "台 待分析:" + TotalSelectDevicesCount.ToString() + "台 数据读取:" + TotalDataReadDeviceCount.ToString() + "台 分析:" + TotalAnalysisedDevicesCount.ToString() + "台 耗时" + TotalTimeUsed.ToString("hh\\:mm\\:ss");
				ToolStripStatusLabel toolStripStatusLabel = toolStripStatusLabel1;
				toolStripStatusLabel.Text = toolStripStatusLabel.Text + " 分析设备:" + AnyDeviceName;
				toolStripProgressBar1.Value = (int)((double)TotalDataReadDeviceCount / (double)TotalSelectDevicesCount * 100.0);
			}
			if (RunState == 2)
			{
				TotalTimeUsed = DateTime.Now - StartRunTime;
				toolStripStatusLabel1.Text = "共有:" + TotalDevicesCount.ToString() + "台 待分析:" + TotalSelectDevicesCount.ToString() + "台 数据读取:" + TotalDataReadDeviceCount.ToString() + "台 分析:" + TotalAnalysisedDevicesCount.ToString() + "台 耗时" + TotalTimeUsed.ToString("hh\\:mm\\:ss");
				ToolStripStatusLabel toolStripStatusLabel2 = toolStripStatusLabel1;
				toolStripStatusLabel2.Text = toolStripStatusLabel2.Text + " 分析设备:" + AnyDeviceName;
				toolStripProgressBar1.Value = (int)((double)TotalAnalysisedDevicesCount / (double)TotalSelectDevicesCount * 100.0);
				if (toolStripProgressBar1.Value == 100)
				{
					timer1.Stop();
					dataGridView1.DataSource = ResaultDT;
					dataGridView1.Columns[0].FillWeight = 20f;
					dataGridView1.Columns[1].FillWeight = 60f;
					dataGridView1.Columns[2].FillWeight = 60f;
					dataGridView1.Columns[3].FillWeight = 60f;
					dataGridView1.Columns[4].FillWeight = 60f;
					dataGridView1.Columns[5].FillWeight = 60f;
					button3.Enabled = true;
					gMapControl1.MinZoom = 1;
					gMapControl1.MaxZoom = 23;
					gMapControl1.Zoom = 15.0;
					toolStripProgressBar1.Visible = false;
					toolStripStatusLabel1.Text = "共有:" + TotalDevicesCount.ToString() + "台 待分析:" + TotalSelectDevicesCount.ToString() + "台 分析完成:" + TotalAnalysisedDevicesCount.ToString() + "台 耗时" + TotalTimeUsed.ToString("hh\\:mm\\:ss");
				}
			}
			if (RunState == 3)
			{
				TotalReportTimeUsed = DateTime.Now - StartRunReportTime;
				toolStripStatusLabel1.Text = "共有:" + TotalDevicesCount.ToString() + "台 待分析:" + TotalSelectDevicesCount.ToString() + "台 数据读取:" + TotalDataReadDeviceCount.ToString() + "台 分析:" + TotalAnalysisedDevicesCount.ToString() + "台 报告:" + TotalReportDeviceCount.ToString() + "台 耗时" + TotalReportTimeUsed.ToString("hh\\:mm\\:ss");
				toolStripProgressBar1.Value = (int)((double)TotalReportDeviceCount / (double)TotalSelectDevicesCount * 100.0);
				if (toolStripProgressBar1.Value == 100)
				{
					toolStripProgressBar1.Visible = false;
					toolStripStatusLabel1.Text = "共有:" + TotalDevicesCount.ToString() + "台 待分析:" + TotalSelectDevicesCount.ToString() + "台 分析完成:" + TotalAnalysisedDevicesCount.ToString() + "台 耗时" + TotalTimeUsed.ToString("hh\\:mm\\:ss") + "报告:" + TotalReportDeviceCount.ToString() + "台 耗时" + TotalReportTimeUsed.ToString("hh\\:mm\\:ss");
				}
			}
		}

		private void lwolf_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex < 0 && dataGridView1.Rows.Count != 0 && lastRow.Count == 0)
			{
				colindex = e.ColumnIndex;
				int index = dataGridView1.Rows.Count - 1;
				lastRow.Add(((DataTable)dataGridView1.DataSource).Rows[index].ItemArray);
				dataGridView1.Rows.Remove(dataGridView1.Rows[dataGridView1.Rows.Count - 1]);
			}
		}

		private void lwolf_Sorted(object sender, EventArgs e)
		{
			if (lastRow.Count != 0)
			{
				DataTable dataTable = (DataTable)dataGridView1.DataSource;
				DataView defaultView = dataTable.DefaultView;
				if (sort)
				{
					sort = false;
					defaultView.Sort = dataTable.Columns[colindex].ColumnName + " asc";
				}
				else
				{
					sort = true;
					defaultView.Sort = dataTable.Columns[colindex].ColumnName + " desc";
				}
				dataTable = defaultView.ToTable();
				dataTable.Rows.Add(lastRow[0]);
				lastRow.Clear();
				dataGridView1.DataSource = dataTable;
			}
		}

		private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
		{
			DataTable dataTable = (DataTable)dataGridView1.DataSource;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (dataTable.Rows[i][0].ToString() == "")
				{
					return;
				}
			}
			DataRow dataRow = dataTable.NewRow();
			for (int j = 1; j < dataTable.Columns.Count; j++)
			{
				double num = 0.0;
				if (j == 5 || j == 3 || j == 4)
				{
					for (int k = 0; k < dataTable.Rows.Count; k++)
					{
						num += (double)Convert.ToInt32(dataTable.Rows[k][j]);
					}
					dataRow[j] = num;
				}
			}
			dataTable.Rows.Add(dataRow);
		}

		private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2 && e.RowIndex <= dataGridView1.Rows.Count - 1)
			{
				DrawLine(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value) - 1, 0);
			}
			if (e.ColumnIndex == 0 && e.RowIndex <= dataGridView1.Rows.Count - 1)
			{
				DrawLine(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value) - 1, 1);
			}
		}

		private void DrawLine(int index, int show)
		{
			if (SelectedDevices[index].img != null && show == 1)
			{
				Form2 form = new Form2(SelectedDevices[index], SelectedDevices[index].img);
				form.ShowDialog();
			}
			else
			{
				gMapControl1.Overlays.Clear();
				double zoom = gMapControl1.Zoom;
				GMapControl gMapControl = gMapControl1;
				Point location = gMapControl1.Location;
				int x = location.X;
				location = gMapControl1.Location;
				PointLatLng pointLatLng = gMapControl.FromLocalToLatLng(x, location.Y);
				GMapControl gMapControl2 = gMapControl1;
				location = gMapControl1.Location;
				int x2 = location.X + gMapControl1.Width;
				location = gMapControl1.Location;
				PointLatLng pointLatLng2 = gMapControl2.FromLocalToLatLng(x2, location.Y + gMapControl1.Height);
				double num = 200.0;
				double num2 = 200.0;
				double num3 = 0.0;
				double num4 = 0.0;
				GMapOverlay gMapOverlay = new GMapOverlay("markers");
				gMapControl1.Overlays.Add(gMapOverlay);
				GMapOverlay gMapOverlay2 = new GMapOverlay("routes");
				GMapMarker gMapMarker = null;
				GMapMarker gMapMarker2 = null;
				DateTime dateTime;
				if (SelectedDevices[index].AnyResault.Count == 0 && SelectedDevices[index].LastLat > 0.0)
				{
					string lastAddress = SelectedDevices[index].LastAddress;
					dateTime = SelectedDevices[index].LastTime;
					string toolTipText = "选择时间范围内设备未移动，位置:" + lastAddress + "\r\n时间:" + dateTime.ToString();
					GMapMarker gMapMarker3 = new GMarkerGoogle(WGSGCJLatLonHelper.WGS84ToGCJ02(SelectedDevices[index].LastLat, SelectedDevices[index].LastLon), GMarkerGoogleType.blue_small);
					gMapMarker3.ToolTipText = toolTipText;
					gMapOverlay.Markers.Add(gMapMarker3);
					gMapControl1.ZoomAndCenterRoutes("markers");
				}
				for (int i = 0; i < SelectedDevices[index].AnyResault.Count; i++)
				{
					if (SelectedDevices[index].AnyResault[i].type == MovingType.Stop)
					{
						string[] obj = new string[8]
						{
							"开始：",
							SelectedDevices[index].AnyResault[i].time.ToString(),
							"结束",
							null,
							null,
							null,
							null,
							null
						};
						dateTime = SelectedDevices[index].AnyResault[i].time + SelectedDevices[index].AnyResault[i].timespan;
						obj[3] = dateTime.ToString();
						obj[4] = "\r\n停止时间:";
						obj[5] = SelectedDevices[index].AnyResault[i].timespan.TotalMinutes.ToString("f2");
						obj[6] = "分钟\r\n 地址:";
						obj[7] = SelectedDevices[index].AnyResault[i].address;
						string toolTipText2 = string.Concat(obj);
						GMapMarker gMapMarker4 = new GMarkerGoogle(WGSGCJLatLonHelper.WGS84ToGCJ02(SelectedDevices[index].AnyResault[i].stoppoint.Lat_old, SelectedDevices[index].AnyResault[i].stoppoint.Lon_old), GMarkerGoogleType.blue_small);
						gMapMarker4.ToolTipText = toolTipText2;
						gMapOverlay.Markers.Add(gMapMarker4);
					}
					if (SelectedDevices[index].AnyResault[i].type == MovingType.Run && SelectedDevices[index].AnyResault[i].runpoints.Count > 1)
					{
						if (gMapMarker == null)
						{
							gMapMarker = new GMarkerGoogle(WGSGCJLatLonHelper.WGS84ToGCJ02(SelectedDevices[index].AnyResault[i].runpoints[0].Lat_old, SelectedDevices[index].AnyResault[i].runpoints[0].Lon_old), GMarkerGoogleType.green_small);
							gMapMarker.ToolTipText = "记录起点，时间:" + SelectedDevices[index].AnyResault[i].runpoints[0].Time.ToString();
							gMapOverlay.Markers.Add(gMapMarker);
						}
						GMapRoute gMapRoute = new GMapRoute("route" + i.ToString());
						gMapRoute.Stroke = new Pen(Brushes.Blue, 2f);
						gMapOverlay2.Routes.Add(gMapRoute);
						gMapControl1.Overlays.Add(gMapOverlay2);
						for (int j = 0; j < SelectedDevices[index].AnyResault[i].runpoints.Count; j++)
						{
							if (SelectedDevices[index].AnyResault[i].runpoints[j].Lat_old > num3)
							{
								num3 = SelectedDevices[index].AnyResault[i].runpoints[j].Lat_old;
							}
							if (SelectedDevices[index].AnyResault[i].runpoints[j].Lat_old < num)
							{
								num = SelectedDevices[index].AnyResault[i].runpoints[j].Lat_old;
							}
							if (SelectedDevices[index].AnyResault[i].runpoints[j].Lon_old > num4)
							{
								num4 = SelectedDevices[index].AnyResault[i].runpoints[j].Lon_old;
							}
							if (SelectedDevices[index].AnyResault[i].runpoints[j].Lon_old < num2)
							{
								num2 = SelectedDevices[index].AnyResault[i].runpoints[j].Lon_old;
							}
							gMapRoute.Points.Add(WGSGCJLatLonHelper.WGS84ToGCJ02(SelectedDevices[index].AnyResault[i].runpoints[j].Lat_old, SelectedDevices[index].AnyResault[i].runpoints[j].Lon_old));
						}
						gMapControl1.UpdateRouteLocalPosition(gMapRoute);
					}
				}
				int num5 = SelectedDevices[index].AnyResault.Count - 1;
				while (num5 >= 0)
				{
					if (SelectedDevices[index].AnyResault[num5].type != 0 || SelectedDevices[index].AnyResault[num5].runpoints.Count <= 1 || gMapMarker2 != null)
					{
						num5--;
						continue;
					}
					gMapMarker2 = new GMarkerGoogle(WGSGCJLatLonHelper.WGS84ToGCJ02(SelectedDevices[index].AnyResault[num5].runpoints[SelectedDevices[index].AnyResault[num5].runpoints.Count - 1].Lat_old, SelectedDevices[index].AnyResault[num5].runpoints[SelectedDevices[index].AnyResault[num5].runpoints.Count - 1].Lon_old), GMarkerGoogleType.red_small);
					gMapMarker2.ToolTipText = "记录终点，时间:" + SelectedDevices[index].AnyResault[num5].runpoints[SelectedDevices[index].AnyResault[num5].runpoints.Count - 1].Time.ToString();
					gMapOverlay.Markers.Add(gMapMarker2);
					break;
				}
				gMapControl1.ZoomAndCenterRoutes("routes");
				Thread.Sleep(2000);
				Image img = gMapControl1.ToImage();
				SelectedDevices[index].img = img;
				if (show == 1)
				{
					Form2 form2 = new Form2(SelectedDevices[index], img);
					form2.ShowDialog();
				}
			}
		}

		private void exportxlsxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
			ContextMenuStrip contextMenuStrip = (ContextMenuStrip)toolStripMenuItem.GetCurrentParent();
			DataGridView dataGridView = (DataGridView)contextMenuStrip.SourceControl;
			if (dataGridView.Rows.Count > 0)
			{
				SaveAs(dataGridView);
			}
		}

		private void SaveAs(DataGridView dgvAgeWeekSex)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Execl files (*.csv)|*.csv";
			saveFileDialog.FilterIndex = 0;
			saveFileDialog.RestoreDirectory = true;
			saveFileDialog.CreatePrompt = true;
			saveFileDialog.Title = "Export Excel File To";
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				Stream stream = saveFileDialog.OpenFile();
				StreamWriter streamWriter = new StreamWriter(stream, Encoding.GetEncoding(0));
				string text = "";
				try
				{
					for (int i = 0; i < dgvAgeWeekSex.ColumnCount; i++)
					{
						if (i > 0)
						{
							text += "\t";
						}
						text += dgvAgeWeekSex.Columns[i].HeaderText;
					}
					streamWriter.WriteLine(text);
					for (int j = 0; j < dgvAgeWeekSex.Rows.Count; j++)
					{
						string text2 = "";
						for (int k = 0; k < dgvAgeWeekSex.Columns.Count; k++)
						{
							if (k > 0)
							{
								text2 += "\t";
							}
							string text3 = dgvAgeWeekSex.Rows[j].Cells[k].Value.ToString();
							if (text3.Contains(',') || text3.Contains('"') || text3.Contains('\r') || text3.Contains('\n') || text3.Contains('\t'))
							{
								text3 = $"\"{text3}\"";
							}
							text2 += text3;
						}
						streamWriter.WriteLine(text2);
					}
					streamWriter.Close();
					stream.Close();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
				finally
				{
					streamWriter.Close();
					stream.Close();
				}
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (dataGridView1.Rows.Count != 0)
			{
				DateTime dateTime = dateTimePicker1.Value;
				string newValue = dateTime.ToShortDateString();
				dateTime = dateTimePicker2.Value;
				string newValue2 = dateTime.ToShortDateString();
				dateTime = DateTime.Now;
				string newValue3 = dateTime.ToShortDateString();
				string newValue4 = "共有:" + TotalDevicesCount.ToString() + "台 分析:" + TotalSelectDevicesCount.ToString() + "台  耗时" + TotalTimeUsed.ToString("hh\\:mm\\:ss");
				string htmlStrFromDGV = AnyFunction.GetHtmlStrFromDGV(dataGridView1);
				string text = "";
				string text2 = "";
				FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
				folderBrowserDialog.Description = "请选择报告导出文件夹";
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
					{
						MessageBox.Show(this, "文件夹路径不能为空", "提示");
						return;
					}
					text2 = folderBrowserDialog.SelectedPath;
				}
				StartRunReportTime = DateTime.Now;
				RunState = 3;
				toolStripProgressBar1.Value = 0;
				toolStripProgressBar1.Visible = true;
				timer1.Start();
				DelectDir(text2);
				Directory.CreateDirectory(text2);
				Directory.CreateDirectory(text2 + "/img");
				SelectedDevices.Sort();
				for (int i = 0; i < SelectedDevices.Count; i++)
				{
					if (SelectedDevices[i].img != null)
					{
						SelectedDevices[i].img.Save(text2 + "/img/" + SelectedDevices[i].Name + ".jpg");
					}
					else
					{
						DrawLine(i, 0);
						SelectedDevices[i].img.Save(text2 + "/img/" + SelectedDevices[i].Name + ".jpg");
					}
					if (SelectedDevices[i].Distance == 0.0)
					{
						if (SelectedDevices[i].LastLat == 0.0)
						{
							text += "<p style = \"text-align:left;\" color=red><strong>";
							text = text + "设备:<a name=\"" + SelectedDevices[i].Name + "\">" + SelectedDevices[i].Name + "</a>无数据，无初始定位&nbsp;&nbsp;<a color=#CCCCCC href=\"#top\">TOP</a>";
							text += "</strong></p>";
						}
						else
						{
							text += "<p style = \"text-align:left;\" color=blue><strong>";
							string[] obj = new string[10]
							{
								text,
								"设备:<a name=\"",
								SelectedDevices[i].Name,
								"\">",
								SelectedDevices[i].Name,
								"</a>无数据，位置:",
								SelectedDevices[i].LastAddress,
								"时间:",
								null,
								null
							};
							dateTime = SelectedDevices[i].LastTime;
							obj[8] = dateTime.ToString();
							obj[9] = "&nbsp;&nbsp;<a color=#CCCCCC href=\"#top\">TOP</a>";
							text = string.Concat(obj);
							text += "</strong></p>";
						}
					}
					else
					{
						string str = "";
						str += "<p style = \"text-align:left;\"><strong>";
						str = str + "设备:<a name=\"" + SelectedDevices[i].Name + "\">" + SelectedDevices[i].Name + "</a> 行驶里程:" + SelectedDevices[i].Distance.ToString("f2") + "公里 停车:" + SelectedDevices[i].Stops.ToString() + "次\r\n 第一条记录时间:" + SelectedDevices[i].GpsPoints[0].Time.ToString() + "最后一条记录时间" + SelectedDevices[i].GpsPoints[SelectedDevices[i].GpsPoints.Count - 1].Time.ToString() + "&nbsp;&nbsp;<a color=#CCCCCC href=\"#top\">TOP</a>";
						str += "</strong></p>";
						str += "<p style = \"text -align:center;\">";
						str = str + "<img src = \"img/" + SelectedDevices[i].Name + ".jpg\" />";
						str += "</p>";
						str += GetDeviceRunStopHtml(SelectedDevices[i]);
						str += "<p style = \"text -align:left;\">";
						str += "<br/>";
						str += "</p>";
						text += str;
					}
					text += "<hr/>";
					TotalReportDeviceCount++;
				}
				StreamReader streamReader = File.OpenText("Temp.html");
				string text3 = "";
				string str2;
				while ((str2 = streamReader.ReadLine()) != null)
				{
					text3 += str2;
				}
				streamReader.Close();
				text3 = text3.Replace("$1", newValue);
				text3 = text3.Replace("$2", newValue2);
				text3 = text3.Replace("$3", newValue3);
				text3 = text3.Replace("$4", newValue4);
				text3 = text3.Replace("$5", htmlStrFromDGV);
				text3 = text3.Replace("$6", text);
				StreamWriter streamWriter = new StreamWriter(text2 + "/report.html");
				streamWriter.Write(text3);
				streamWriter.Close();
				Process.Start(text2 + "/report.html");
				timer1.Stop();
			}
		}

		private static string GetDeviceRunStopHtml(Device dev)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<p style = \"text -align:left;\">");
			stringBuilder.AppendLine("行驶统计：");
			stringBuilder.AppendLine("</p>");
			stringBuilder.AppendLine("<table border=\"1\" bordercolor=\"#000000\">");
			stringBuilder.AppendLine("  <tr>");
			stringBuilder.AppendLine("    <td>序号</td>");
			stringBuilder.AppendLine("    <td>开始时间</td>");
			stringBuilder.AppendLine("    <td>结束时间</td>");
			stringBuilder.AppendLine("    <td>距离</td>");
			stringBuilder.AppendLine("  </tr>");
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("<p style = \"text -align:left;\">");
			stringBuilder2.AppendLine("停车统计：");
			stringBuilder2.AppendLine("</p>");
			stringBuilder2.AppendLine("<table border=\"1\" bordercolor=\"#000000\">");
			stringBuilder2.AppendLine("  <tr>");
			stringBuilder2.AppendLine("    <td>序号</td>");
			stringBuilder2.AppendLine("    <td>开始时间</td>");
			stringBuilder2.AppendLine("    <td>结束时间</td>");
			stringBuilder2.AppendLine("    <td>停车时间</td>");
			stringBuilder2.AppendLine("    <td>停车位置</td>");
			stringBuilder2.AppendLine("  </tr>");
			int num = 1;
			int num2 = 1;
			for (int i = 0; i < dev.AnyResault.Count; i++)
			{
				DateTime dateTime;
				if (dev.AnyResault[i].type == MovingType.Run)
				{
					stringBuilder.AppendLine("  <tr>");
					stringBuilder.AppendLine("    <td width=\"50px\">" + num.ToString() + "</td>");
					stringBuilder.AppendLine("    <td width=\"80px\">" + dev.AnyResault[i].time.ToString() + "</td>");
					StringBuilder stringBuilder3 = stringBuilder;
					dateTime = dev.AnyResault[i].time + dev.AnyResault[i].timespan;
					stringBuilder3.AppendLine("    <td width=\"80px\">" + dateTime.ToString() + "</td>");
					stringBuilder.AppendLine("    <td width=\"250px\">" + dev.AnyResault[i].distance.ToString() + "</td>");
					num++;
				}
				else
				{
					stringBuilder2.AppendLine("  <tr>");
					stringBuilder2.AppendLine("    <td width=\"50px\">" + num2.ToString() + "</td>");
					stringBuilder2.AppendLine("    <td width=\"80px\">" + dev.AnyResault[i].time.ToString() + "</td>");
					StringBuilder stringBuilder4 = stringBuilder2;
					dateTime = dev.AnyResault[i].time + dev.AnyResault[i].timespan;
					stringBuilder4.AppendLine("    <td width=\"80px\">" + dateTime.ToString() + "</td>");
					stringBuilder2.AppendLine("    <td width=\"80px\">" + dev.AnyResault[i].timespan.ToString("hh\\:mm\\:ss") + "</td>");
					stringBuilder2.AppendLine("    <td width=\"250px\">" + dev.AnyResault[i].address + "</td>");
					num2++;
				}
			}
			stringBuilder.AppendLine("</table>");
			stringBuilder2.AppendLine("</table>");
			return stringBuilder.ToString() + "<br>" + stringBuilder2.ToString();
		}

		public static void DelectDir(string srcPath)
		{
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(srcPath);
				FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
				FileSystemInfo[] array = fileSystemInfos;
				foreach (FileSystemInfo fileSystemInfo in array)
				{
					if (fileSystemInfo is DirectoryInfo)
					{
						DirectoryInfo directoryInfo2 = new DirectoryInfo(fileSystemInfo.FullName);
						directoryInfo2.Delete(true);
					}
					else
					{
						File.Delete(fileSystemInfo.FullName);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void treeView1_MouseClick(object sender, MouseEventArgs e)
		{
			TotalSelectDevicesCount = 0;
			CountNode(treeView1.Nodes);
			toolStripStatusLabel1.Text = "共有设备:" + TotalDevicesCount.ToString() + "台 待分析设备:" + TotalSelectDevicesCount.ToString() + "台";
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			Login login = new Login();
			if (login.ShowDialog() == DialogResult.OK)
			{
				if (GlobalVar.username == "user")
				{
					MessageBox.Show("您现在使用的是测试账户:user 左侧列表只显示单位 请记录需要申请权限的单位名称 报指挥中心申请新用户和权限！");
				}
			}
			else
			{
				Close();
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportxlsxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(1054, 682);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.button3);
            this.splitContainer2.Panel2.Controls.Add(this.button2);
            this.splitContainer2.Panel2.Controls.Add(this.dateTimePicker2);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.dateTimePicker1);
            this.splitContainer2.Panel2.Controls.Add(this.button1);
            this.splitContainer2.Size = new System.Drawing.Size(219, 682);
            this.splitContainer2.SplitterDistance = 512;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(15, 3, 3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(217, 510);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseClick);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(139, 121);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(51, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Report";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(71, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "analysis";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Enabled = false;
            this.dateTimePicker2.Location = new System.Drawing.Point(11, 78);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "结束时间";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "开始时间";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Enabled = false;
            this.dateTimePicker1.Location = new System.Drawing.Point(11, 35);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(51, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.gMapControl1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer3.Size = new System.Drawing.Size(831, 660);
            this.splitContainer3.SplitterDistance = 480;
            this.splitContainer3.TabIndex = 1;
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(0, 0);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(829, 478);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(829, 174);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataSourceChanged += new System.EventHandler(this.dataGridView1_DataSourceChanged);
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportxlsxToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(152, 26);
            // 
            // exportxlsxToolStripMenuItem
            // 
            this.exportxlsxToolStripMenuItem.Name = "exportxlsxToolStripMenuItem";
            this.exportxlsxToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.exportxlsxToolStripMenuItem.Text = "Export(*.xlsx)";
            this.exportxlsxToolStripMenuItem.Click += new System.EventHandler(this.exportxlsxToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 660);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(831, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 682);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "GPS历史数据分析工具(by:knifeandcj)";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

		}

  
    }
}
