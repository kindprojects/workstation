using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using NLog;

namespace ImportService
{
    static class Program
    {
        public static Logger Logger { set; get; }
        public static ServiceConfig Config { set; get; }
        public static WebHost Host { set; get; }
        
        [STAThread]
        static void Main()
        {            
            Config = new ServiceConfig().Load();
            Logger = LogManager.GetCurrentClassLogger();

            try
            {
                Host = new WebHost();
                Host.Start();
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex.Message);

                Application.Exit();
            }

            Logger.Trace("Сервис запущен");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FormService());

            Logger.Trace("Сервис остановлен");
        }   
    }
}
