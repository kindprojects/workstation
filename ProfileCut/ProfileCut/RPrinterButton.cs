using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Reflection;

using System.ComponentModel.Design.Serialization;

namespace ProfileCut
{
    public class RTemplateOverloads
    {
        public string PrinterName { set; get; }

        public Dictionary<string, string> GetTemplateOverloadsDictonary()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            Type type = this.GetType();
            var infoArray = type.GetProperties();
            foreach (MemberInfo info in infoArray)
            {
                ret.Add(info.Name.ToLower(), PrinterName);
            }

            return ret;
        }
    }
    
    public class RPrinterButton : Button
    {
        public string AttrTemplate { set; get; }

        public RTemplateOverloads TemplateOverloads { set; get; }

        public RPrinterButton(RAppButton config)
        {
            Text = config.Text;
            AttrTemplate = config.AttrTemplate;

            TemplateOverloads = new ProfileCut.RTemplateOverloads();
            TemplateOverloads.PrinterName = config.TemplateOverloads.PrinterName;
        }
    }
}
