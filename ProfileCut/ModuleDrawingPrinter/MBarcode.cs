using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ModuleDrawingPrinter
{
    public class MBarcode
    {
        public MPage OwnerPage { set; get; }
        public PointF Origin { set; get; }
        public float Height { set; get; }
        public int HorAlign { set; get; }
        public int VerAlign { set; get; }
        public string Text { set; get; }

        public MBarcode(MPage ownerPage, PointF origin, float height, int horAlign, int verAlign, string text)
        {
            OwnerPage = ownerPage;
            
            int l = (int)Math.Truncate(origin.X * ownerPage.Width / 100);
            int t = (int)Math.Truncate(origin.Y * ownerPage.Height / 100);

            Origin = new PointF(l, t);
            
            Height = height;
            HorAlign = horAlign;
            VerAlign = verAlign;
            Text = text;
        }
    }
}
