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
        IPBaseObject NavigatorInitialize(string path);
        IPBaseObject Navigate(PModelObjectNavigatorPathLevel level, NAV_DIRECTION dir);
        IPBaseObject GetPointer();
    }
}
