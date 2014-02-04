using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace ModuleZebraPrinter
{
    class MConfigFontsSection : ConfigurationSection
    {
        [ConfigurationProperty("fonts")]
        public MFontsCollection FontItems
        {
            get { return ((MFontsCollection)(base["fonts"])); }
        }

        [ConfigurationProperty("settings")]
        public MSettingsCollection SettingsItems
        {
            get { return ((MSettingsCollection)(base["settings"])); }
        }


    }

    [ConfigurationCollection(typeof(MFontElement))]
    public class MFontsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MFontElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MFontElement)(element)).Key;
        }

        public MFontElement this[int idx]
        {
            get { return (MFontElement)BaseGet(idx); }
        }
    }

    public class MFontElement : ConfigurationElement
    {

        [ConfigurationProperty("key", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return ((string)(base["key"])); }
            set { base["key"] = value; }
        }

        [ConfigurationProperty("name", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Name
        {
            get { return ((string)(base["name"])); }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("height", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Height
        {
            get { return ((string)(base["height"])); }
            set { base["height"] = value; }
        }

        [ConfigurationProperty("width", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Width
        {
            get { return ((string)(base["width"])); }
            set { base["width"] = value; }
        }
    }

    [ConfigurationCollection(typeof(MSettingsElement))]
    public class MSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MSettingsElement)(element)).Key;
        }

        public MSettingsElement this[int idx]
        {
            get { return (MSettingsElement)BaseGet(idx); }
        }
    }

    public class MSettingsElement : ConfigurationElement
    {

        [ConfigurationProperty("key", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return ((string)(base["key"])); }
            set { base["key"] = value; }
        }

        [ConfigurationProperty("value", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Value
        {
            get { return ((string)(base["value"])); }
            set { base["value"] = value; }
        }
    }
}
