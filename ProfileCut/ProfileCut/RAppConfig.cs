using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServiceStack.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace ProfileCut
{
    public class RAppConfig
    {
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

        private string _fileName;

        public RAppConfig()
        {
            _fileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\config.json";
        }

        private void Save()
        {
            string json = JsonSerializer.SerializeToString<RAppConfig>(this);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(_fileName))
            {
                file.Write(json);
            }
        }

        public RAppConfig Load()
        {
            string json = "";
            try
            {
                json = System.IO.File.ReadAllText(_fileName);
            }
            catch(Exception ex)
            {
                throw new Exception(String.Format("Неудалось открыть файл {0}. {1}", _fileName, ex.Message));
            }

            RAppConfig conf = JsonSerializer.DeserializeFromString<RAppConfig>(json);

            if (conf == null || conf.Version == null)            
                throw new Exception(String.Format("Неудалось загрузить конфигурацию из файла {0}\nНеверный формат", _fileName));
            
            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (conf.Version != version)
                throw new Exception(String.Format(
                    "Версия указаная в конфигурационном файле {0} не совпадает с версией приложения {1}", 
                    conf.Version, version));
            
            return JsonSerializer.DeserializeFromString<RAppConfig>(json);
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

            config.Commands.Buttons.Add(new RAppButton()
            {
                Text = "Печать",
                AttrTemplate = "PRINT_STICKERS",
                TemplateOverloads = new RAppTemplateOverloads()
                {
                    PrinterName = "ZDesigner LP 2844"
                }
            });

            return config;
        }
    }

    public class RAppCommands
    {
        public List<RAppButton> Buttons { set; get; }
    }

    public class RAppButton
    {
        public string Text { set; get; }
        public string AttrTemplate { set; get; }
        public RAppTemplateOverloads TemplateOverloads {set; get; }
    }

    public class RAppTemplateOverloads
    {
        public string PrinterName { set; get; }
    }
}
