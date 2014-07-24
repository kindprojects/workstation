using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using ServiceStack.Text;

namespace ImportService
{          
    internal class ServiceConfig
    {
        private string _fileName;
        public int port { set; get; }

        public string connectionString { set; get; }       

        public ServiceConfig()
        {
            _fileName = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\config.json";
        }

        public ServiceConfig Load()
        {
            bool ok = true;
            ServiceConfig conf = null;
            string json;
            try
            {
                json = System.IO.File.ReadAllText(_fileName);
                conf = JsonSerializer.DeserializeFromString<ServiceConfig>(json);    
                if (conf == null)
                    ok = false;
            }
            catch 
            {
                ok = false;
            }

            if (!ok)
            {
                Program.Logger.Error("Конфигурационный отсутствует, поврежден, или имеет неверный формат. Настройки установлены по умолчнию");

                return _getUndefinedConfig();
            }
            else
                return conf;
        }

        private void _save()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(_fileName))
            {
                file.Write(JsonSerializer.SerializeToString<ServiceConfig>(this));
            }
        }

        private ServiceConfig _getUndefinedConfig()
        {
            ServiceConfig conf = new ServiceConfig();
            conf.port = 80;
            conf.connectionString = "charset=WIN1251;datasource=localhost;database=ProfileCut;user=sysdba;password=masterkey";

            return conf;
        }
    }
}
