using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using HardwareInterfaces;

namespace ModulePrinter
{
    public class Printer: IStickerPrinter
    {
        public void SetFont(string name, float size)
        {
            
        }
        public void WriteText(string text, double x, double y, int alignHor, int alignVer, double angle, double maxWidth, double maxHeight, bool doShrink, bool doGrow)
        {
                        
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            throw new Exception("not implemented");
        }
        public void DrawLine(Point p1, Point p2)
        {
            throw new Exception("not implemented");
        }
        public void NewPage(double width, double height)
        {

        }
        public void Execute()
        {
            throw new Exception("not implemented");
        }
    }
}
