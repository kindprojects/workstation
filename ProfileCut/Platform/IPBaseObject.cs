﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public interface IPBaseObject
    {
        int Id { set; get; }
        IPBaseObject NavigatorInitialize(string path);
        IPBaseObject Navigate(int depth, NAV_DIRECTION dir);
        IPBaseObject Navigate(string path);
        IPBaseObject GetNavigatorPointer();
        bool GetAttr(string name, out string val);
        PCollection GetCollection(string name, bool createIfNotFound);
    }
}
