using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;
using System.ComponentModel;

namespace Platform2
{
    public interface IPObject
    {
        int Id { set; get; }

        bool GetAttr(string name, bool findInOwners, out string val);

        void SetAttr(string name, string value);

        IPObject Navigate(int depth, NAV_DIRECTION direction);
        
        IPObject Navigate(string path);
        
        IPObject GetObjectByDepth(int level);
        
        IPObject GetObjectById(int id);
        
        void SetNavigatorPointer(IPObject obj);
        
        IPObject GetNavigatorPointer();
        
        string FindAndFormat(string attrName, BackgroundWorker worker);
        
        string Format(string templateName, BackgroundWorker worker);
        
        void SaveAttr(string name);
    }
}
