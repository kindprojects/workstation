using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Xml;

namespace ProfileCut
{
    public class RConfig
    {
        public string ConnectionString { private set; get; }
        public string ModelCode { private set; get; }
        public string MasterCollectionPath { private set; get; }
        public string MasterItemTemplate { private set; get; }
        public string DetailTemplate { private set; get; }        
        public string Navigation { private set; get; }
        public string SelectedHtmlElementClass { private set; get; }
        public int MasterItemsUpdateIntervalMs { private set; get; }
        //public int PrintLevel { private set; get; }
        //public string PrintTemplate { private set; get; }
        //public string PrinterModule { private set; get; }
        //public string PrinterModuleNameSpace { private set; get; }
        //public string PrinterModuleClass { private set; get; }
        //public string PrinterName { private set; get; }
        ////public string AttrTemplate { private set; get; }

        //public string AttrPrintTemplate { private set; get; }

        public List<RConfigButton> PrinterButtons { private set; get; }

        public RConfig()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["FireBirdConnection"].ConnectionString;
            ModelCode = ConfigurationManager.AppSettings["ModelCode"];
            MasterCollectionPath = ConfigurationManager.AppSettings["MasterCollectionPath"];

            MasterItemTemplate = ConfigurationManager.AppSettings["MasterItemTemplate"];
            DetailTemplate = ConfigurationManager.AppSettings["DetailTemplate"];
            
            Navigation = ConfigurationManager.AppSettings["Navigation"];
            SelectedHtmlElementClass = ConfigurationManager.AppSettings["SelectedHtmlElementClass"];

            MasterItemsUpdateIntervalMs = _getInt("MasterItemsUpdateIntervalMs", null);

            //PrinterModule = _getString("PrinterModule", "");
            //PrinterModuleNameSpace = _getString("PrinterModuleNameSpace", "");
            //PrinterModuleClass = _getString("PrinterModuleClass", "");
            //PrinterName = _getString("PrinterName", "");
            //AttrPrintTemplate = _getString("AttrPrintTemplate", "");

            PrinterButtons = new List<RConfigButton>();
            RConfigRegisterButtons config = RConfigRegisterButtons.GetConfig();
            foreach (RConfigButton item in config.Buttons)
            {
                PrinterButtons.Add(item);
            }           
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

    //public class Handler: IConfigurationSectionHandler
    //{
    //    public object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
    //    {
    //        return null;
    //    }
    //}
}
