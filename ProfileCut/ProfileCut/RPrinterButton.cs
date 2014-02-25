﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace ProfileCut
{
    class RPrinterButton : Button
    {
        public string ModuleFileName { set; get; }                
        public string ModuleNameSpace { set; get; }                
        public string ModuleClass { set; get; }                
        public string PrinterName { set; get; }
        public string AttrTemplate { set; get; }                
        public RPrinterButton(string text, string moduleFileName, string moduleNameSpace, string moduleClassName, string printerName, string attrTemplate)
        {
            Text = text;
            ModuleFileName = moduleFileName;
            ModuleNameSpace = moduleNameSpace;
            ModuleClass = moduleClassName;
            PrinterName = printerName;
            AttrTemplate = attrTemplate;
        }
    }
}