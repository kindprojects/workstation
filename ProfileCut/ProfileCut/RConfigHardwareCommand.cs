using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platform
{
    public class RConfigHardwareCommand
    {
        public string Name { set; get; }
        public string ApplyTo { set; get; }
        public string List { set; get; }
        public string Step { set; get; }
        public string Send { set; get; }
        public string Module { set; get; }
        public string Func { set; get; }
        public string Text { set; get; }
    }

    public class RConfigHardwareCommands
    {
        public List<RConfigHardwareCommand> items { set; get; }
        public RConfigHardwareCommands()
        {
            items = new List<RConfigHardwareCommand>();
        }
    }
}
