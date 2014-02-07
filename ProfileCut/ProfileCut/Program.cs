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
#if !DEBUG
            try
            {
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FMain());
#if !DEBUG
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetType().ToString() + ":\n" + e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }
    }
}
