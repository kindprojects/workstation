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

        public MText(MPage ownerPage, string text, Font font, RectangleF rectangle)
        {
            OwnerPage = ownerPage;
            Text = text;
            Font = font;

            if (ownerPage != null)
            {
                RectangleF rect = new RectangleF(
                    ownerPage.Width * rectangle.X / 100 * ownerPage.Dpm,
                    ownerPage.Height * rectangle.Y / 100 * ownerPage.Dpm,
                    ownerPage.Width * (rectangle.X + rectangle.Width) / 100 * ownerPage.Dpm,
                    ownerPage.Height * (rectangle.Y + rectangle.Height) / 100 * ownerPage.Dpm);
            }
            
            Brush = Brushes.Black;
        }
    }
}
