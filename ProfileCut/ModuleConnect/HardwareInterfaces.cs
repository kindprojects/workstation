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
        void SetFont(string name);

        void WriteText(string text, double x, double y, int angle, string align, double width);

        //void WriteText(string text, double x, double y, int alignHor, int alignVer, double angle, double maxWidth, double maxHeight, bool doShrink, bool doGrow);
        //void DrawLine(float x1, float y1, float x2, float y2);
        //void DrawLine(Point p1, Point p2);
        void WriteBarcode(string text, double x, double y, double height, double width);
        void NewPage(double width, double height);
        void Init();
    }
}
