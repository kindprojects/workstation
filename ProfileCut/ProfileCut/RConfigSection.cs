using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace ProfileCut
{
    public class RConfigPrintButton : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }
        [ConfigurationProperty("text", IsRequired = true)]
        public string Text
        {
            get
            {
                return this["text"] as string;
            }
        }
        [ConfigurationProperty("module", IsRequired = true)]
        public string Module
        {
            get
            {
                return this["module"] as string;
            }
        }
        [ConfigurationProperty("nameSpace", IsRequired = true)]
        public string NameSpace
        {
            get
            {
                return this["nameSpace"] as string;
            }
        }
        [ConfigurationProperty("class", IsRequired = true)]
        public string Class
        {
            get
            {
                return this["class"] as string;
            }
        }
        [ConfigurationProperty("printer", IsRequired = true)]
        public string Printer
        {
            get
            {
                return this["printer"] as string;
            }
        }
        [ConfigurationProperty("attrTemplate", IsRequired = true)]
        public string AttrTemplate
        {
            get
            {
                return this["attrTemplate"] as string;
            }
        }
    }
    public class RConfigButtons : ConfigurationElementCollection
    {
        public RConfigPrintButton this[int index]
        {
            get
            {
                return base.BaseGet(index) as RConfigPrintButton;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }
        public new RConfigPrintButton this[string responseString]
        {
            get { return (RConfigPrintButton)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }
        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new RConfigPrintButton();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((RConfigPrintButton)element).Name;
        }
    }
    public class RConfigRegisterButtons : ConfigurationSection
    {
        public static RConfigRegisterButtons GetConfig()
        {
            return (RConfigRegisterButtons)System.Configuration.ConfigurationManager.GetSection("printButtons") ?? new RConfigRegisterButtons();
        }

        [System.Configuration.ConfigurationProperty("buttons")]
        [ConfigurationCollection(typeof(RConfigButtons), AddItemName = "button")]
        public RConfigButtons Buttons
        {
            get
            {
                object o = this["buttons"];
                return o as RConfigButtons;
            }
        }

    }

}
