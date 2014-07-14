using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfileCut
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.Run(new FormMain());

            }
			catch (Exception e)
			{
				MessageBox.Show(e.GetType().ToString() + ":\n" + e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
        }
    }
}
