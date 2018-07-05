using System;
using System.Windows.Forms;

namespace GPSCheck
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
            DialogResult result;
            using (var loginForm = new Login())
                result = loginForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                // login was successful
                Application.Run(new Form1());
            }
           
		}
	}
}
