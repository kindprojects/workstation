using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Platform;

namespace ProfileCut
{
    class RNavigatorButton : Button
    {
        public PModelObjectNavigatorPathLevel NavigatorLevel { set; get; }
        public NAV_DIRECTION NavDirection { set; get; }

        public RNavigatorButton(PModelObjectNavigatorPathLevel level, NAV_DIRECTION dir)
        {
            NavigatorLevel = level;
            NavDirection = dir;
        }
    }
}
