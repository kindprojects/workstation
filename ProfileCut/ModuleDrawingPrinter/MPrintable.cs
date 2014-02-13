using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ModuleDrawingPrinter
{
    public abstract class MPrintable
    {
        public float X;
        public float Y;
        public abstract SizeF MeasureObject(Graphics context);
    }
}
