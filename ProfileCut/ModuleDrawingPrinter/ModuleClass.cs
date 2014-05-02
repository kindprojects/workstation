using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using GenCode128;
using ModuleConnect;

namespace ModuleNamespace
{
    public class ModuleClass : IModule, IDisposable
    {
        List<string> _problems = new List<string>();
        Printer _printer;
        
        public ModuleClass(Dictionary<string,string>moduleParams)
        {
            string printerName;
            if (moduleParams == null || !moduleParams.TryGetValue("printername", out printerName))
                 AddProblem("PrinterName не задан");
            else
                _printer = new Printer(printerName);
        }
		public void Dispose()
		{
			this.Dispose(false);
		}
		protected virtual void Dispose(bool cleanManaged)
		{
			_printer.Dispose();
		}

        public bool Execute(string commands)
        {
            try
            {
                if (_printer != null)
                {
                    string[] aCommands = commands.Split('\n');
                    foreach (string line in aCommands)
                    {
                        string cmdLine = line.Trim();
                        if (cmdLine == string.Empty)
                            continue;
                        MCustomizedPrinterCommand cmd = new MCustomizedPrinterCommand(cmdLine);
                        if (cmd.Code != null)
                        {
                            string expr = cmd.GetParamStr("hideif", "");
                            if (expr != "")
                            {
                                string[] parts = expr.Split('=');
                                if (parts.Count() == 2)
                                {
                                    if (parts[0].Trim() == parts[1].Trim())
                                        continue;
                                }
                            }
                            switch (cmd.Code.ToUpper())
                            {
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
                                        _printer.NewPage(cmd.GetParamIntPositive("width"),
                                            cmd.GetParamIntPositive("height"),
                                            fields.left,
                                            fields.top,
                                            fields.right,
                                            fields.bottom,
                                            cmd.GetParamFloat("originy", 0.0F));
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message + "\nПараметры PAGE: width(int, мм), height(int, мм), fields(intList, мм)");
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

                    _printer.Print();

                    return true;
                }
                else
                {
                    throw new Exception("Принтер не назначен!");
                }
            }
            catch (Exception e)
            {
                AddProblem(e.Message);
                
                return false;
            }
        }

        public void AddProblem(string description)
        {
            _problems.Add(description);
        }

        public string CheckProblems()
        {
            string s = "";
            foreach(string prb in _problems)
            {
                s += prb+"\n";
            }
            
            return s.TrimEnd();
        }

        public bool QueryValue(string paramName, bool caseSensitive, out string result)
        {
			result = null;
			return false;
        }
    }
}
