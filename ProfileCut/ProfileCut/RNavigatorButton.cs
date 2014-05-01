using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Platform2;

namespace ProfileCut
{
    public class RNavigatorButton : Button
    {
        public int Depth { set; get; }
        public NAV_DIRECTION Direction { set; get; }

        public RNavigatorButton(int depth, NAV_DIRECTION direction)
        {
            Depth = depth;
            Direction = direction;
        }
    }
}
