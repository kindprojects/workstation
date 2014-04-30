using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;

using ModuleConnect;
using System.Windows.Forms;

namespace Platform2
{
    static public class PTemplates
    {
        private static bool _queryToModule(string moduleName, string varName, out string value)
        {
            string modulesDir = Path.GetDirectoryName(Application.ExecutablePath);
            MConnect connect = new MConnect(Path.Combine(modulesDir, moduleName));
            IModule module = connect.GetModuleInterface(null);

            return module.QueryValue(varName, true, out value);
        }

        public static string FormatObject(IPObject obj, string template, IMValueGetter host, IMValueGetter overloads)
        {
            List<PTemplateAttr> attrs;
            List<PTemplateCollection> fcollects;

            _parse(template, out attrs, out fcollects);

            template = template.Replace("%id%", obj.Id.ToString());
            foreach (var attr in attrs)
            {
                string val = "";
				string moduleName = attr.Module.Trim().ToLower();
				string attrName = attr.Name;

				bool valFound = false;
                
				if (moduleName == "")
                {
					if (overloads == null || !overloads.QueryValue(attrName, true, out val))
						valFound = obj.GetAttr(attr.Name, true, out val);
                }
                else if (moduleName == "host")
                {
					if (host != null)
						valFound = host.QueryValue(attr.Name, false, out val);
				}
				else
				{
					valFound = _queryToModule(moduleName, attrName, out val);
				}
				if (!valFound)
					val = "<" + attrName + ">";

                template = template.Replace(attr.OperatorText, val);
            }

            foreach (var fcollect in fcollects)
            {
                PCollection coll = obj.GetCollection(fcollect.collectionName);
                if (coll != null)
                {
                    string val = "";
                    for (int ii = 0; ii < coll.Count(); ii++)
                    {
                        PObject cobj = coll.GetObject(ii);

                        string tmp = "";
                        if (cobj.GetAttr(fcollect.templateName, true, out tmp))
                            val += FormatObject(cobj, tmp, host, overloads);
                        else
							val += "<!" + fcollect.templateName + "!>";
						val += (fcollect.endsWithNewLine ? "\n" : "");
                    }
                    template = template.Replace(fcollect.OperatorText, val);
                }
            } // ToDo: может надо else что-нибудь вывести? Коллекция не найдена

            return template;
        }
		
        static private void _parse(string template, out List<PTemplateAttr> attrs, out List<PTemplateCollection> fcollects)
        {
            attrs = new List<PTemplateAttr>();
            fcollects = new List<PTemplateCollection>();

            foreach (Match match in Regex.Matches(template, @"(%[^%\s\[\]<>']+%)"))
            {
                attrs.Add(new PTemplateAttr(match.Groups[1].Value));
            }

            foreach (Match match in Regex.Matches(template, @"(\[[^%\s\[\]]+:[^%\s\[\]\+]+\+?\])"))
            {
                fcollects.Add(new PTemplateCollection(match.Groups[1].Value));
            }
        }
    }

    internal class PTemplateOperator
    {
        internal string OperatorText { set; get; }

        internal PTemplateOperator(string text)
        {
            OperatorText = text;
        }
    }

    internal class PTemplateAttr : PTemplateOperator
    {
        internal string Module;

        internal string Name;
        internal PTemplateAttr(string operatorText)
            : base(operatorText)
        {
            string text = Regex.Match(operatorText, @"%([^%]+)%").Groups[1].Value;
            int index = text.IndexOf(":");
            if (index > 0)
            {
                Module = text.Substring(0, index);
                Name = text.Substring(index + 1, text.Length - index - 1);
            }   
            else
            {
                Module = "";
                Name = text;
            }
        }
    }

    internal class PTemplateCollection : PTemplateOperator
    {
        internal string collectionName { set; get; }
        internal string templateName { set; get; }
        internal bool endsWithNewLine { set; get; }
        internal string navigatorLevelText { set; get; }
        internal PTemplateCollection(string operatorText)
            : base(operatorText)
        {
            Match match = Regex.Match(operatorText, @"\[(\S+):([^\[\]\+]+)(\+)?\]", RegexOptions.IgnoreCase);

            collectionName = match.Groups[1].Value;
            templateName = match.Groups[2].Value;
            endsWithNewLine = (match.Groups[3].Value == "+");
        }
    }

    internal class PTagMarks
    {
        internal string Begin = "";
        internal string End = "";
    }

    internal class PNotFoundMarks
    {
        static internal PTagMarks attrs;
        static internal PTagMarks collections;

        internal PNotFoundMarks()
        {
            attrs.Begin = "<";
            attrs.End = ">";

            collections.Begin = "<!";
            collections.End = "!>";
        }
    }
}
