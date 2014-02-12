using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace ModuleDrawingPrinter
{
    public class MText
    {
        public MPage OwnerPage { set; get; }
        public string Text { set; get; }
        public Font Font { set; get; }
        public Brush Brush { set; get; }
        public RectangleF Rectangle { set; get; }        
        public int HorAlign { set; get; }
        public int VerAlign { set; get; }


        public MText(MPage ownerPage, string text, Font font, RectangleF rectangle, int horAlign, int verAlign)
        {
            OwnerPage = ownerPage;

            Text = text;
            Font = font;
            HorAlign = horAlign;
            VerAlign = verAlign;

            if (ownerPage != null)
            {
                int l = (int)Math.Truncate(rectangle.Left * ownerPage.Width / 100);
                int t = (int)Math.Truncate(rectangle.Top * ownerPage.Height / 100);
                int w = (int)Math.Truncate(rectangle.Width * ownerPage.Width / 100);
                int h = (int)Math.Truncate(rectangle.Height * ownerPage.Height / 100);
                Rectangle = new RectangleF(l, t, w, h);
            }
            else
            {
                throw new Exception("Страница не задана");
            }
                        
            Brush = Brushes.Black;
        }
    }
}
