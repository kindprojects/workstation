using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace ProfileCut
{
    
    class RConfigHardwareCommandsSection : ConfigurationSection
    {
        [ConfigurationProperty("hardwareCommands")]
        public RHardwareCommandsCollection HardwareCommandItems
        {
            get { return ((RHardwareCommandsCollection)(base["hardwareCommands"])); }
        }
    }

    [ConfigurationCollection(typeof(RHardwareCommandElement))]
    public class RHardwareCommandsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RHardwareCommandElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RHardwareCommandElement)(element)).Key;
        }

        public RHardwareCommandElement this[int idx]
        {
            get { return (RHardwareCommandElement)BaseGet(idx); }
        }       
    }

    public class RHardwareCommandElement : ConfigurationElement
    {

        [ConfigurationProperty("key", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return ((string)(base["key"])); }
            set { base["key"] = value; }
        }

        [ConfigurationProperty("applyTo", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ApplyTo
        {
            get { return ((string)(base["applyTo"])); }
            set { base["applyTo"] = value; }
        }

        [ConfigurationProperty("list", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string List
        {
            get { return ((string)(base["list"])); }
            set { base["list"] = value; }
        }

        [ConfigurationProperty("step", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Step
        {
            get { return ((string)(base["step"])); }
            set { base["step"] = value; }
        }

        [ConfigurationProperty("send", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Send
        {
            get { return ((string)(base["send"])); }
            set { base["send"] = value; }
        }

        [ConfigurationProperty("module", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Module
        {
            get { return ((string)(base["module"])); }
            set { base["module"] = value; }
        }

        [ConfigurationProperty("func", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Func
        {
            get { return ((string)(base["func"])); }
            set { base["func"] = value; }
        }

        [ConfigurationProperty("text", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Text
        {
            get { return ((string)(base["text"])); }
            set { base["text"] = value; }
        }
    }
}
