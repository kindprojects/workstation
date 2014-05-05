using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleConnect
{
    public interface IMValueGetter
    {
        bool QueryValue(string varName, bool caseSensitive, out string value);
    }
}
