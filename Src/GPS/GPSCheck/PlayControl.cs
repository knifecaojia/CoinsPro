using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPSCheck
{
    public partial class PlayControl : Form
    {
        public PlayControl()
        {
            InitializeComponent();
            GlobalVar.UpdatePointEvent += GlobalVar_UpdatePointEvent;
        }

        private void GlobalVar_UpdatePointEvent(DateTime t)
        {
            label3.Text = t.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double pct = (double)trackBar1.Value / 100;
            GlobalVar.RaisePlayPointEvent(pct);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalVar.RaiseStartPlayEvent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GlobalVar.RaisePausePlayEvent();
        }
    }
}
