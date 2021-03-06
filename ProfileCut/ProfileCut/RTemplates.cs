﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.IO;

namespace Model
{
    public class RTemplates
    {
        public RNotFoundMarks NotFoundMarks;

        private Dictionary<string, string> _items;

        public RTemplates()
        {
            NotFoundMarks = new RNotFoundMarks();
            _loadFiles(Directory.GetCurrentDirectory());
        }

        private void _loadFiles(string folder)
        {
            _items = new Dictionary<string, string>();
            foreach (string file in Directory.EnumerateFiles(folder, "*.tmp"))
            {
                _items.Add(Path.GetFileNameWithoutExtension(file).ToLower(), File.ReadAllText(file));
            }
        }

        public void Clear()
        {
            _items = new Dictionary<string,string>();
        }

        public void AddTemplate(string name, string text)
        {
            _items.Add(name.ToLower(), text);
        }

        public string TransformText(string templateName, RBaseObject obj, ref string path, bool addPath)
        {
            string template = "";

            if (!_items.TryGetValue(templateName.ToLower(), out template))
            {
                template = NotFoundMarks.collections.Begin + templateName + NotFoundMarks.collections.End;
            }
            else
            {
                List<RTemplateAttr> attrs;
                List<RTemplateCollection> fcollects;

                _parse(template, out attrs, out fcollects);

                template = template.Replace("%id%", obj.Id.ToString());
                foreach (var attr in attrs)
                {
                    string val = "";
                    if (!obj.GetAttr(attr.Name, out val))
                        val = NotFoundMarks.attrs.Begin + attr.Name + NotFoundMarks.attrs.End;
                    template = template.Replace(attr.OperatorText, val);
                }

                foreach (var fcollect in fcollects)
                {
                    RCollection coll = obj.GetCollection(fcollect.collectionName, false);
                    if (coll != null)
                    {
                        string val = "";
                        for (int ii = 0; ii < coll.Count(); ii++)
                        {
                            //val += this.PreScript + TransformText(fcollect.templateName, coll.GetObject(ii)) + this.PostStript;

                            RBaseObject cobj =  coll.GetObject(ii);
                            bool addNext = (addPath && fcollect.navigatorLevelText != "");
                            if (addNext)
                            {
                                path += ((path != "")?"/":"") + fcollect.collectionName + ":" + fcollect.navigatorLevelText;
                                addPath = false;
                            }
                            val += this.TransformText(fcollect.templateName, cobj, ref path, addNext);

                        }
                        template = template.Replace(fcollect.OperatorText, val);
                    }
                }
            }
           
            return template;
        }

        private string _getTemplate(string name)
        {
            string template = "";

            if (!_items.TryGetValue(name.ToLower(), out template))
            {
                template = NotFoundMarks.collections.Begin + name + NotFoundMarks.collections.End;
            }

            return template;
        }

        private void _parse(string template, out List<RTemplateAttr> attrs, out List<RTemplateCollection> fcollects)
        {
            attrs = new List<RTemplateAttr>();
            fcollects = new List<RTemplateCollection>();


            foreach (Match match in Regex.Matches(template, @"(%[^%\s\[\]<>']+%)"))
            {
                attrs.Add(new RTemplateAttr(match.Groups[1].Value));
            }

            //foreach (Match match in Regex.Matches(template, @"(\[[^%\s\[\]]+:[^%\s\[\]]+\])"))
            foreach (Match match in Regex.Matches(template, @"(\[[^%\s\[\]\(\)]+:[^%\s\[\]\(\)]+(?:\(" + "\".+" + "\"" + @"\))?\])"))            
            {
                fcollects.Add(new RTemplateCollection(match.Groups[1].Value));
            }
        }
    }

    public class RTemplateOperator
    {
        public string OperatorText { set; get; }

        public RTemplateOperator(string text)
        {
            OperatorText = text;
        }
    }

    public class RTemplateAttr : RTemplateOperator
    {
        public string Name;
        public RTemplateAttr(string operatorText)
            : base(operatorText)
        {
            Name = Regex.Match(operatorText, @"%([^%]+)%").Groups[1].Value;
        }
    }

    public class RTemplateCollection : RTemplateOperator
    {
        public string collectionName { set; get; }
        public string templateName { set; get; }
        public string navigatorLevelText { set; get; }
        public RTemplateCollection(string operatorText)
            : base(operatorText)
        {
            //Match match = Regex.Match(operatorText, @"\[(\S+):(\S+)\]", RegexOptions.IgnoreCase);
            Match match = Regex.Match(operatorText, @"\[(\S+):([^\[\]\(\)]+)(?:\(" + "\"(.+)\"" + @"\))?\]", RegexOptions.IgnoreCase);

            collectionName = match.Groups[1].Value;
            templateName = match.Groups[2].Value;
            navigatorLevelText = match.Groups[3].Value;
        }
    }

    public class RTagMarks
    {
        public string Begin = "";
        public string End = "";
    }


    public class RNotFoundMarks
    {
        public RTagMarks attrs;
        public RTagMarks collections;

        public RNotFoundMarks()
        {
            attrs = new RTagMarks();
            attrs.Begin = "<";
            attrs.End = ">";

            collections = new RTagMarks();
            collections.Begin = "<!";
            collections.End = "!>";
        }
    }
}
