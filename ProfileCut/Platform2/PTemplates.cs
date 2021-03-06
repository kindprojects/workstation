﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text.RegularExpressions;

using ModuleConnect;
using System.Windows.Forms;
using System.ComponentModel;

namespace Platform2
{
    static public class PTemplates
    {
        private static bool _queryToModule(string moduleName, string varName, out string value)
        {
            string modulesDir = Path.GetDirectoryName(Application.ExecutablePath);
            MConnect connect = new MConnect(Path.Combine(modulesDir, moduleName));
            IModule module = connect.GetModuleInterface(null);

            return module.QueryValue(varName, false, out value);
        }

        public static string FormatObject(IPObject obj, string template, IMHost host, IMValueGetter overloads, BackgroundWorker worker, PNavigationInfo navInfo)
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
					if (overloads != null)
						valFound = overloads.QueryValue(attrName, false, out val);
					if (!valFound)
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
					val = "<" + attr.ToString() + ">";

                template = template.Replace(attr.OperatorText, val);
            }

            foreach (var fcollect in fcollects)
            {
                IPCollection coll = obj.GetCollection(fcollect.collectionName);
                if (coll != null)
                {
                    string val = "";
					int cnt = coll.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        IPObject cobj = coll.GetObject(i);

                        string tmp = "";
                        if (cobj.GetAttr(fcollect.templateName, true, out tmp))
                        {
                            if (worker != null && worker.CancellationPending)
                                return "";

                            bool navAdded = false;
                            if (fcollect.navigatorLevelCaption != "")
                            {
                                if (navInfo != null)
                                {
                                    navInfo.captions.Add(fcollect.navigatorLevelCaption);
                                    navInfo.levels.Add(fcollect.collectionName);
                                    navAdded = true;
                                }
                            }
                            val += FormatObject(cobj, tmp, host, overloads, worker, navInfo);

							// теперь, если уровень навигации добавлялся - нужно сбросить переменную, чтоб в этом цикле не добавить соседние подходящие коллекции
							if (navAdded)
								navInfo = null;
                        }
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

	public class PNavigationInfo
	{
		public List<string> levels;
		public List<string> captions;

		public PNavigationInfo()
		{
			levels = new List<string>();
			captions = new List<string>();
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
		public override string ToString()
		{
			return (Module != "" ? Module + ":" : "") + Name;
		}
    }

    internal class PTemplateCollection : PTemplateOperator
    {
		internal string collectionName;
		internal string templateName;
		internal bool endsWithNewLine;
		internal string navigatorLevelCaption;
        internal PTemplateCollection(string operatorText)
            : base(operatorText)
        {
			Match match = Regex.Match(operatorText, @"\[(\S+):([^\[\]\+\(\)]+)(\+)?(?:\((.+)\))?\]", RegexOptions.IgnoreCase);

			collectionName = match.Groups[1].Value;
			templateName = match.Groups[2].Value;
			endsWithNewLine = (match.Groups[3].Value == "+");
			navigatorLevelCaption = match.Groups[4].Value;
        }
    }
}
