using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ServiceStack.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using ModuleConnect;

namespace ProfileCut
{
    public class RAppConfig
    {
		protected string fileName;
        public string Version { set; get; }
        public string ConnectionString { set; get; }
        public string ModelCode { set; get; }
        public string MasterItemTemplate { set; get; }
        public string MasterCollectionPath { set; get; } //collect1:index/collect2:index/collect31 -->
        public string DetailTemplate { set; get; }
        public string SelectedHtmlElementClass { set; get; }
        public string Navigation  { set; get; }
        public int MasterItemsUpdateIntervalMs { set; get; }
        public bool Debug { set; get; }
        public RAppCommands Commands { set; get; }

		public List<RAppConfigVar> HostVars;
	
        private void Save()
        {
            string json = JsonSerializer.SerializeToString<RAppConfig>(this);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.fileName))
            {
                file.Write(json);
            }
        }

        static public RAppConfig Load(string fileName)
        {
            string json = "";
            try
            {
                json = System.IO.File.ReadAllText(fileName);
            }
            catch(Exception ex)
            {
                throw new Exception(String.Format("Неудалось открыть файл {0}. {1}", fileName, ex.Message));
            }

            RAppConfig conf = JsonSerializer.DeserializeFromString<RAppConfig>(json);

            if (conf == null || conf.Version == null)            
                throw new Exception(String.Format("Неудалось загрузить конфигурацию из файла {0}\nНеверный формат", fileName));
            
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (conf.Version != version)
                throw new Exception(String.Format(
                    "Версия указаная в конфигурационном файле {0} не совпадает с версией приложения {1}", 
                    conf.Version, version));
            conf.fileName = fileName;
            return conf;
        }

        private RAppConfig CreateDefault()
        {
            RAppConfig config = new RAppConfig();
            config.ModelCode = "podst";
            config.MasterItemTemplate = "OptItem";
            config.MasterCollectionPath = "Optimizations";
            config.DetailTemplate = "OptView";
            config.SelectedHtmlElementClass = "selectedObject";
            config.Navigation = "profiles:профиль/canes:хлыст";
            config.MasterItemsUpdateIntervalMs = 10000;
            config.Debug = true;

			RAppConfigVar printerName = new RAppConfigVar();
			printerName.ParamName = "PrinterName";
			printerName.Value = "ZDesigner LP 2844";

			RAppCommand print = new RAppCommand();
			print.Name = "Печать";
			print.TargetAttr = "PRINT_STICKERS";
			print.TemplateOverloads.Add(printerName);

			config.Commands.Buttons.Add(print);

            return config;
        }
		public static void ParseNavigationSetup(string path, out List<string> navLevels, out List<string> levelsAliases)
		{
			navLevels = new List<string>();
			levelsAliases = new List<string>();
			try
			{
				MatchCollection matches = Regex.Matches(path, @"([^:/]+):([^:/]+)");
				foreach (Match match in matches)
				{
					navLevels.Add(match.Groups[1].Value.ToLower());
					levelsAliases.Add(match.Groups[2].Value);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Неверный формат пути. " + ex.Message);
			}
		}
	}

    public class RAppCommands
    {
        public List<RAppCommand> Buttons { set; get; }
    }

    public class RAppCommand : IMValueGetter
    {
        public string Name { set; get; }
        public string TargetAttr { set; get; }
        public List<RAppConfigVar> TemplateOverloads {set; get; }

		bool IMValueGetter.QueryValue(string varName, bool caseSensitive, out string value)
		{
			string lowerName = varName.ToLower();
			foreach (RAppConfigVar var in TemplateOverloads)
			{
				if (caseSensitive && var.ParamName == varName || var.ParamName.ToLower() == lowerName)
				{
					value = var.Value;
					return true;
				}
			}
			value = null;
			return false;
		}
	}

	public class RAppConfigVar
	{
		public string ParamName { set; get; }
		public string Value { set; get; }
	}	
}
