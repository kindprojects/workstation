using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HardwareInterfaces
{
    public interface IBaseFunction
    {
        void Execute();
    }
    public interface IStickerPrinter : IBaseFunction
    {
        void SetFont(string name, float size);
        void WriteText(RectangleF bounds, int horAlign, int verAlign, string text, float angle);
        void WriteBarcode(PointF origin, float height, int horAlign, int verAlign, string text);
        void NewPage(float width, float height, int fieldLeft, int fieldTop, int fieldRight, int filedBottom);
        void Init(string printerName);
    }
}
