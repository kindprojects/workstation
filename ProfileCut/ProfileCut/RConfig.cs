using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace ProfileCut
{
    public class RConfig
    {
        public string ConnectionString { private set; get; }
        public string ModelCode { private set; get; }
        public string MasterCollection { private set; get; }
        public string MasterItemTemplate { private set; get; }
        public string DetailTemplate { private set; get; }        
        public string Navigation { private set; get; }
        public string SelectedHtmlElementClass { private set; get; }
        public int MasterItemsUpdateIntervalMs { private set; get; }
        public int PrintLevel { private set; get; }
        //public string PrintTemplate { private set; get; }
        public string PrinterModule { private set; get; }
        public string PrinterModuleNameSpace { private set; get; }
        public string PrinterModuleClass { private set; get; }
        public string PrinterName { private set; get; }
        //public string AttrTemplate { private set; get; }

        public string AttrPrintTemplate { private set; get; }

        public RConfig()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["FireBirdConnection"].ConnectionString;
            ModelCode = ConfigurationManager.AppSettings["ModelCode"];
            MasterCollection = ConfigurationManager.AppSettings["MasterCollection"];

            MasterItemTemplate = ConfigurationManager.AppSettings["MasterItemTemplate"];
            DetailTemplate = ConfigurationManager.AppSettings["DetailTemplate"];
            
            Navigation = ConfigurationManager.AppSettings["Navigation"];
            SelectedHtmlElementClass = ConfigurationManager.AppSettings["SelectedHtmlElementClass"];

            MasterItemsUpdateIntervalMs = _getInt("MasterItemsUpdateIntervalMs", null);

            PrintLevel = _getInt("PrintLevel", -1);
            
            
            //PrintTemplate = _getString("PrintTemplate", "");

            PrinterModule = _getString("PrinterModule", "");
            PrinterModuleNameSpace = _getString("PrinterModuleNameSpace", "");
            PrinterModuleClass = _getString("PrinterModuleClass", "");
            PrinterName = _getString("PrinterName", "");

            AttrPrintTemplate = _getString("AttrPrintTemplate", "");

            //AttrTemplate = ConfigurationManager.AppSettings["AttrTemplate"];

            //HardwareCommands = new RConfigHardwareCommands();
            //RConfigHardwareCommandsSection section = (RConfigHardwareCommandsSection)ConfigurationManager.GetSection("startupHardwareCommands");
            //foreach (RHardwareCommandElement item in section.HardwareCommandItems)
            //{
                //HardwareCommands.items.Add(new RConfigHardwareCommand()
                //{
                //    Name = item.Key,
                //    ApplyTo = item.ApplyTo,
                //    List = item.List,
                //    Step = item.Step,
                //    Send = item.Send,
                //    Module = item.Module,
                //    Func = item.Func,
                //    Text = item.Text
                //});
            //}
        }

        private string _getString(string key, string defaultValue)
        {
            string val = ConfigurationManager.AppSettings[key];
            if (val != null && val != "")
            {
                return val;
            }
            else if (defaultValue != null)
                return defaultValue;
            else
                throw new Exception("Параметр '" + key + "' не задан!");
        }

        private int _getInt(string key, int? defaultValue)
        {
            string s = _getString(key, (defaultValue == null)?null:defaultValue.ToString());
            try
            {
                return Convert.ToInt32(s);
            }catch{
                throw new Exception("Значение параметра '" + key + "' имеет неверный формат");
            }
        }
    }
}
