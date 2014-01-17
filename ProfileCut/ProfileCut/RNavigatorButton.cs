using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Model
{
    class RNavigatorButton : Button
    {
        public RModelObjectNavigatorPathLevel NavigatorLevel { set; get; }
        public NAV_DIRECTION NavDirection { set; get; }

        public RNavigatorButton(RModelObjectNavigatorPathLevel level, NAV_DIRECTION dir)
        {
            NavigatorLevel = level;
            NavDirection = dir;
        }
    }
}
