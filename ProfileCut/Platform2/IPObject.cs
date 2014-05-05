using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Repository;

namespace Platform2
{
    public interface IPObject
    {
        int Id { set; get; }
		IPCollection GetCollection(string name);
        bool GetAttr(string name, bool findInOwners, out string val);
		bool FindAttr(string attrName, out IPObject obj, out string val);
		void SetAttr(string name, string value);
        void StorageUpdateAttr(string name);
		IPCollection onwerCollection { get; }
		bool IsChildOf(int objectId);

		XElement ToXElement();
    }
}