using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;

namespace ModuleNamespace
{
    public class MPrinterCommand
    {
        public string Code { private set; get; }
        private Dictionary<string, string> _params;
        public MPrinterCommand(string commandString)
        {
            _params = new Dictionary<string, string>();

            // получение кода
            Regex r = new Regex(@"(\S+)\s+(.*)");
            Match m = r.Match(commandString);
            if (m.Groups.Count > 1)
                Code = m.Groups[1].Value.ToString();
            if (m.Groups.Count > 2)
            {
                string sParams = m.Groups[2].Value.ToString().Trim();
                // список параметров
                r = new Regex(@"(\S+)\s*:\s*" + '"' + "([^\"]*)\"");

                m = r.Match(sParams);
                while (m.Groups.Count == 3)
                {
                    _params.Add(m.Groups[1].Value.ToString().ToLower(), m.Groups[2].Value.ToString());
                    m = m.NextMatch();
                }
            }
        }

        public string GetParamStr(string name, string def = null)
        {
            if (_params.ContainsKey(name.ToLower()))
            {
                return _params[name];
            }
            else if (def == null)
            {
                throw new Exception("В комманде " + Code + " параметр " + name + " не задан");
            }
            else
            {
                return def;
            }
        }
        public int GetParamInt(string name, int? def = null)
        {
            string s = GetParamStr(name, (def == null) ? null : def.ToString());
            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception)
            {
                throw new Exception("Параметр " + name + " должен быть задан в виде целого числа");
            }
        }
        public float GetParamFloat(string name, double? def = null)
        {
            string s = GetParamStr(name, (def == null) ? null : def.ToString());
            s = s.Replace(".", ",");
            try
            {
                return (float)Convert.ToDouble(s);
            }
            catch (Exception)
            {
                throw new Exception("Параметр " + name + " должен быть задан в виде числа с плавающей точкой");
            }
        }
        public bool GetParamBool(string name, bool? def = null)
        {
            string s = GetParamStr(name, (def == null) ? null : ((bool)def ? "1" : "0")).ToLower();
            try
            {
                return (s == "1" || s == "true");
            }
            catch (Exception)
            {
                throw new Exception("Параметр " + name + " должен быть задан в виде 1|0 или TRUE|FALSE");
            }
        }

        public List<int> GetParamListInt(string name, string def = null)
        {
            List<int> ret = new List<int>();
            string s = GetParamStr(name, (def == null) ? null : def);

            try
            {
                string[] strs = s.Split(',');
                foreach (string str in strs)
                {
                    ret.Add(Convert.ToInt32(str));
                }

                return ret;
            }
            catch (Exception)
            {
                throw new Exception("Параметр " + name + " должен быть задан в виде чисел, разделённых запятой");
            }
        }
        public float GetParamLimited(string paramName, float min, float max, float? def)
        {
            float val = this.GetParamFloat(paramName, def);
            if (val < min || val > max)
                throw new Exception(string.Format("Параметр {0} должен принимать значение в интервале от {1} до {2}", paramName, min, max));
            return val;
        }
    }
}
