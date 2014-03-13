using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleConnect
{
    public delegate void ModuleFinishedHandler(IModule module);
    public class MScriptManager
    {
        public static List<MCallData> Parse(string script)
        {
            List<MCallData> ret = new List<MCallData>();

            while (true)
            {
                int index = script.IndexOf("@MODULE", 1, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    string newScript = script.Substring(0, index - 1);
                    ret.Add(new MCallData(newScript));
                    script = script.Substring(index);
                }
                else
                {
                    ret.Add(new MCallData(script));
                    break;
                }
            }
            return ret;
        }

        public static void Execute(string modulesDir, string script, ModuleFinishedHandler callBack)
        {
            List<MCallData> lst = Parse(script);
            foreach(MCallData m in lst)
            {
                m.Execute(modulesDir, callBack);
            }
        }       
    }
}
