using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GenCode128;
using BarcodeLib;

namespace ModuleNamespace
{
    public class MBarcode : MPrintable
    {
        public float Height { set; get; }
        public int HorAlign { set; get; }
        public int VerAlign { set; get; }
        public string Text { set; get; }

        public MBarcode(MPage ownerPage, PointF origin, float height, int horAlign, int verAlign, string text)
        {
            OwnerPage = ownerPage;
            
            this.X = origin.X * ownerPage.Width / 100.0F;
            this.Y = origin.Y * ownerPage.Height / 100.0F;
            
            Height = height * ownerPage.Height / 100.0F;
            HorAlign = horAlign;
            VerAlign = verAlign;
            Text = text;
        }

        public override SizeF MeasureObject(Graphics context)
        {
            return new SizeF();
        }

        public Image Render(Graphics context)
        {
            float heightInPixel = new Bitmap(1,1).VerticalResolution * this.Height / 25.4F;
            //Image img = Code128Rendering.MakeBarcodeImage(this.Text, 1F, (int)heightInPixel, false);     

            Barcode bar = new Barcode();
            bar.IncludeLabel = false;
            bar.Alignment = AlignmentPositions.LEFT;
            bar.Height = (int)heightInPixel;
            bar.Width = this.Text.Length * 10 + 50;
            bar.RotateFlipType = RotateFlipType.RotateNoneFlipNone;
            bar.BackColor = Color.White;
            bar.ForeColor = Color.Black;       
            
            Image img = bar.Encode(TYPE.CODE128, this.Text);

            Bitmap b = new Bitmap(img.Width, img.Height);
            Graphics g = Graphics.FromImage((Image)b);
            g.DrawImage(img, 0, 0, (int)img.Width, img.Height);
            g.Dispose();

            return (Image)b;
        }
    }
}
