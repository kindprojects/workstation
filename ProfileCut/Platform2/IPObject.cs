using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;

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
        
        string FindAndFormat(string attrName);
        
        string Format(string templateName);
        
        void SaveAttr(string name);
    }
}
