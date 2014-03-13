using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace ModuleConnect
{
    public class MCallData
    {
        public string ModuleName {set; get; }
        Dictionary<string, string> ModuleParams {set; get; }
        public string Commands {set; get; }

        public MCallData(string moduleCall)
        {
            ModuleParams = new Dictionary<string, string>();

            if (moduleCall == "")
                throw new Exception("Не задан параметр moduleCall конструктора MCallData");
            
            string[] aCommands = moduleCall.Split('\n');
            if (aCommands.Length > 1)
            {
                int p = moduleCall.IndexOf('\n');
                if (p > 0)
                    this.Commands = moduleCall.Substring(p);
            }

            string moduleCommand = aCommands[0];

            Regex r = new Regex(@"^@MODULE\s+(.*)$");
            Match m = r.Match(moduleCommand);
            if (m.Groups.Count == 2)
            {
                string sParams = m.Groups[1].Value.ToString().Trim();
                r = new Regex(@"(\S+)\s*:\s*" + '"' + "([^\"]*)\"");

                m = r.Match(sParams);
                while (m.Groups.Count == 3)
                {
                    string paramName = m.Groups[1].Value.ToString().ToLower();
                    string paramValue = m.Groups[2].Value.ToString();
                    if (paramName == "name")
                        this.ModuleName = paramValue;
                    else
                        this.ModuleParams.Add(paramName, paramValue);
                    m = m.NextMatch();
                }
            }
        }

        public void Execute(string modulesDir, ModuleFinishedHandler callBack)
        {
            if (modulesDir == "")
                throw new Exception("Путь к хранилищу модулей не задан!");
            MConnect connect = new MConnect(Path.Combine(modulesDir, this.ModuleName));
            IModule module = connect.GetModuleInterface(this.ModuleParams);
            module.Execute(this.Commands);
            
            callBack(module);
        }        
    }
}
