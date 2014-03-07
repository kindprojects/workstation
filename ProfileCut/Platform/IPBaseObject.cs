using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public interface IPBaseObject
    {
        int Id { set; get; }
        IPBaseObject Navigate(int depth, NAV_DIRECTION dir);
        IPBaseObject Navigate(string path);
        IPBaseObject GetNavigatorPointer();
        void SetNavigatorPointer(int id);
        List<string> GetPathFromObject(IPBaseObject obj);
        IPBaseObject GetObjectByDepth(int level);
        bool GetAttr(string name, bool findInOwners, out string val);
        PCollection GetCollection(string name, bool createIfNotFound);
        PBaseObject FindObjectById(int id);
        string GetTemplateName(IPBaseObject obj, string attrTemplate);
        string FindAndFormat(string attrName, Dictionary<string, string> overloads);
    }
}
