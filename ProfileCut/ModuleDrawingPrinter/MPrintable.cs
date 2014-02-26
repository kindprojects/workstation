using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ModuleNamespace
{
    public abstract class MPrintable
    {
        public MPage OwnerPage { set; get; }
        public float X;
        public float Y;
        public abstract SizeF MeasureObject(Graphics context);
    }
}
