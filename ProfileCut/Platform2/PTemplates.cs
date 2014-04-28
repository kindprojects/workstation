using System;
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
    internal class PTemplates
    {
        public PNotFoundMarks NotFoundMarks;
        public PObject OwnerObject { set; get; }

        public PTemplates(PObject ownerObject)
        {
            NotFoundMarks = new PNotFoundMarks();
            OwnerObject = ownerObject;            
        }
        
        private string _queryToOutside(string module, string text)
        {
            if (module.ToLower().Trim() == "host")
                return _queryToHost(text);
            else
                return _queryToModule(module, text);
        }

        private string _queryToHost(string text)
        {
            string response = this.OwnerObject.Host.QueryToHost(text);
            if (response != null)
                return response;
            else
                return NotFoundMarks.attrs.Begin + text + NotFoundMarks.attrs.End;
        }

        private string _queryToModule(string moduleName, string text)
        {
            string response = null;
            string modulesDir = Path.GetDirectoryName(Application.ExecutablePath);
            MConnect connect = new MConnect(Path.Combine(modulesDir, moduleName));
            IModule module = connect.GetModuleInterface(null);

            response = module.ModuleQuery(text);
            if (response != null)
                return response;
            else
                return NotFoundMarks.attrs.Begin + text + NotFoundMarks.attrs.End;
        }

        public string Format(string template, BackgroundWorker worker)
        {           
            List<PTemplateAttr> attrs;
            List<PTemplateCollection> fcollects;

            _parse(template, out attrs, out fcollects);

            template = template.Replace("%id%", OwnerObject.Id.ToString());
            foreach (var attr in attrs)
            {
                string val = "";
                if (attr.Module == "")
                {
                    if (!OwnerObject.GetAttr(attr.Name, true, out val))
                    {
                        val = NotFoundMarks.attrs.Begin + attr.Name + NotFoundMarks.attrs.End;
                    }
                }
                else
                {
                    val = _queryToOutside(attr.Module, attr.Name);
                }

                template = template.Replace(attr.OperatorText, val);
            }

            foreach (var fcollect in fcollects)
            {
                PCollection coll = OwnerObject.GetCollection(fcollect.collectionName, false);
                if (coll != null)
                {
                    string val = "";
                    for (int ii = 0; ii < coll.Count(); ii++)
                    {
                        PObject cobj = coll.GetObject(ii);

                        string tmp = "";
                        if (cobj.GetAttr(fcollect.templateName, true, out tmp))
                        {
                            string carret = "";

                            if (worker != null && worker.CancellationPending)
                                return "";                            

                            string format = cobj.Templates.Format(tmp, worker);
                            if (fcollect.endsWithNewLine)
                            {
                                carret = "\n";
                            }
                            else
                            {
                                // г.к. выпилить при следующем диплое
                                if (format.IndexOf("PAGE") >= 0)
                                    carret = "\n";
                            }
                            val += (format + carret);
                        }
                        else
                        {
                            val += NotFoundMarks.attrs.Begin + fcollect.templateName + NotFoundMarks.attrs.End;
                        }
                    }
                    template = template.Replace(fcollect.OperatorText, val);
                }
            }

            return template;
        }

        public string TransformText(string templateName, BackgroundWorker worker)
        {       
            string template = "";
            if (OwnerObject.GetAttr(templateName, true, out template))
            {
                if (worker != null && worker.CancellationPending)
                    return "";
                
                return Format(template, worker);
            }
            else
            {
                return NotFoundMarks.attrs.Begin + templateName + NotFoundMarks.attrs.End; 
            }   
        }

        private void _parse(string template, out List<PTemplateAttr> attrs, out List<PTemplateCollection> fcollects)
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
        internal PTagMarks attrs;
        internal PTagMarks collections;

        internal PNotFoundMarks()
        {
            attrs = new PTagMarks();
            attrs.Begin = "<";
            attrs.End = ">";

            collections = new PTagMarks();
            collections.Begin = "<!";
            collections.End = "!>";
        }
    }
}
