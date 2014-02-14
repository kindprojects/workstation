﻿using System;
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

            _printer = _module.GetClassInstance<IStickerPrinter>("ModuleDrawingPrinter", "Printer");
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
            public float GetParamFloat(string name, double? def = null)
            {
                string s = GetParamStr(name, (def == null) ? null : def.ToString());
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

		public struct Fields
		{
			public int left, right, top, bottom;
			public Fields(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.right = right;
				this.top = top;
				this.bottom = bottom;
			}
		}

		public class RCustomizedPrinterCommand : RPrinterCommand
		{
			public RCustomizedPrinterCommand(string commandString) : base(commandString) { }

			public int GetParamAlign(string paramName, int? def = null){
				return (int)Math.Truncate(this.GetParamLimited(paramName, -1, 1, def));
			}

			public Fields GetParamFields(string paramName, string def){
				List<int> fields = this.GetParamListInt(paramName, def);
				int cnt = fields.Count();
				while (fields.Count < 4)
					fields.Add(fields[0]);
				Fields f = new Fields(fields[0], fields[1], fields[2], fields[3]);
				if (cnt == 1)
				{
					f.top = f.left;
					f.right = f.left;
					f.bottom = f.left;
				}
				else if (cnt == 2)
				{
					f.right = f.left;
					f.bottom = f.top;
				}
				else if (cnt == 3)
				{
					f.bottom = f.top;
				}
				else
				{
					throw new Exception("Параметр "+paramName+" должен состоять из 1,2,3 либо 4 целых чисел, разделённых запятой");
				}
				return f;
			}

			public float GetParamPercent(string paramName, float? def = null){
				return this.GetParamLimited(paramName, 0, 100, def);
			}

            public float GetParamFloatPositive(string paramName, float? def = null)
            {
                float val = this.GetParamFloat(paramName, def);
                if (val <= 0)
                    throw new Exception("Значение параметра " + paramName + " должно быть строго положительным (больше нуля)");
                return val;
            }
            public int GetParamIntPositive(string paramName, int? def = null)
            {
                int val = this.GetParamInt(paramName, def);
                if (val <= 0)
                    throw new Exception("Значение параметра " + paramName + " должно быть строго положительным (больше нуля)");
                return val;
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
                RCustomizedPrinterCommand cmd = new RCustomizedPrinterCommand(cmdLine);
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
								try
								{
									_printer.WriteText(
										new System.Drawing.RectangleF(
											cmd.GetParamPercent("x")
											, cmd.GetParamPercent("y")
											, cmd.GetParamPercent("width", 100)
											, cmd.GetParamPercent("height", 100)
											)
										, cmd.GetParamAlign("halign", -1)
										, cmd.GetParamAlign("valign", -1)
										, lblText
										, cmd.GetParamLimited("angle", -360, 360, 0)
									);
								}
								catch (Exception ex)
								{
									throw new Exception(ex.Message + "\nПараметры LBL: x(%), y(%), width(%), height(%), halign(int<-1..1>), valign(int<-1..1>), text(string), angle(float<-360.0..360.0>)", ex);
								}
                            }
                            break;

                        case "FNT":
							try
							{
								_printer.SetFont(cmd.GetParamStr("name"), cmd.GetParamFloatPositive("size"));
							}
							catch (Exception ex)
							{
								throw new Exception(ex.Message + "\nПараметры FNT: name(string), size(float)", ex);
							}
                            break;

						case "PAGE":
							try
							{
								Fields fields = cmd.GetParamFields("fields", "0");
								_printer.NewPage(cmd.GetParamIntPositive("width"), cmd.GetParamIntPositive("height"), fields.left, fields.top, fields.right, fields.bottom);
							}
							catch (Exception ex)
							{
								throw new Exception(ex.Message+"\nПараметры PAGE: width(int, мм), height(int, мм), fields(intList, мм)");
							}

							break;

                        case "BAR":
							try
							{
								string barText = cmd.GetParamStr("text");
								if (barText != "")
								{
									_printer.WriteBarcode(
										new System.Drawing.PointF(
											cmd.GetParamPercent("x")
											, cmd.GetParamPercent("y")
										)
										, cmd.GetParamPercent("height")
										, cmd.GetParamAlign("halign", -1)
										, cmd.GetParamAlign("valign", -1)
										, barText
									);
								}
							}
							catch (Exception ex)
							{
								throw new Exception(ex.Message + "\nПараметры BAR: text, x, y, height, halign, valign");
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
