using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleConnect;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using HardwareInterfaces;

namespace ProfileCut
{
    public class RPrinter
    {
        private HardwareModule _module;
        private IStickerPrinter _printer;

        public RPrinter(string modulePath, string printerName)
        {
            if (!Path.IsPathRooted(modulePath))
            {
                modulePath = Path.GetFullPath(modulePath);
            }
            _module = new HardwareModule(modulePath);

            _printer = _module.GetClassInstance<IStickerPrinter>("ModuleZebraPrinter", "Printer");
            if (printerName != "")
            {
                _printer.Init(printerName);
            }
        }

        public class RPrinterCommand
        {
            public string Code { private set; get; }
            private Dictionary<string, string> _params;
            public RPrinterCommand(string commandString)
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
            public double GetParamFloat(string name, double? def = null)
            {
                string s = GetParamStr(name, (def == null) ? null : def.ToString());
                try
                {
                    return Convert.ToDouble(s);
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
                    throw new Exception("Параметр " + name + " должен быть задан в виде чисел разделенных запятой");
                }
            }
        }

        public void Print(string Commands)
        {
            string[] aCommands = Commands.Split('\n');
            foreach (string line in aCommands)
            {
                string cmdLine = line.Trim();
                if (cmdLine == string.Empty)
                    continue;
                RPrinterCommand cmd = new RPrinterCommand(cmdLine);
                if (cmd.Code != null)
                {
                    switch (cmd.Code.ToUpper())
                    {
                        //case "LBL":
                        //    _printer.WriteText(
                        //        cmd.GetParamStr("text")
                        //        , cmd.GetParamFloat("x")
                        //        , cmd.GetParamFloat("y")
                        //        , cmd.GetParamInt("alignHor", -1)
                        //        , cmd.GetParamInt("alignVer", -1)
                        //        , cmd.GetParamFloat("angle", 0)
                        //        , cmd.GetParamFloat("maxWidth", 0)
                        //        , cmd.GetParamFloat("maxHeight", 0)
                        //        , cmd.GetParamBool("shrink", false)
                        //        , cmd.GetParamBool("grow", false)
                        //    );
                        //    break;

                        case "LBL":
                            string lblText = cmd.GetParamStr("text");
                            if (lblText != "")
                            {
                                _printer.WriteText(
                                    cmd.GetParamStr("text")
                                    , cmd.GetParamFloat("x")
                                    , cmd.GetParamFloat("y")
                                    , cmd.GetParamInt("angle", 0)
                                    , cmd.GetParamStr("align", "L")
                                    , cmd.GetParamFloat("width", 100)
                                );
                            }
                            break;

                        case "FNT":
                            _printer.SetFont(cmd.GetParamStr("name"));
                            break;

                        case "PAGE":
                            List<int> fields = cmd.GetParamListInt("fields", "0");
                            if (fields.Count() == 1)
                            {
                                _printer.NewPage(cmd.GetParamFloat("width"), cmd.GetParamFloat("height"), fields[0], fields[0], fields[0], fields[0]);
                            }
                            else if (fields.Count() == 4)
                            {
                                _printer.NewPage(cmd.GetParamFloat("width"), cmd.GetParamFloat("height"), fields[0], fields[1], fields[2], fields[3]);
                            }
                            else
                            {
                                throw new Exception("У тэга PAGE неверно задан параметр fields");
                            }

                            break;

                        case "BAR":
                            string barText = cmd.GetParamStr("text");
                            if (barText != "")
                            {
                                _printer.WriteBarcode(barText,
                                    cmd.GetParamFloat("x"),
                                    cmd.GetParamFloat("y"),
                                    cmd.GetParamFloat("width"),
                                    cmd.GetParamFloat("height")
                                );
                            }
                            break;

                        default:
                            throw new Exception("Неизвестный параметр " + cmd.Code);
                    }
                }
            }

            _printer.Execute();
        }
    }
}
