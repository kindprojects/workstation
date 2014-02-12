using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace ModuleDrawingPrinter
{
    class MPrinter
    {        
        private PrintDocument _pd;
        private MPage _page;
        
        public MPrinter(string printerName, PaperSize size)
        {     
            _pd = new PrintDocument();
            _pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            _pd.PrintPage += new PrintPageEventHandler(this._printPage);
        }

        public void Print(List<MPage> pages)
        {
            try
            {
                foreach (MPage page in pages)
                {
                    //_pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 350, 50);
                    _page = page;
                    _pd.Print();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка принтера. {0}", ex.Message));
            }
        }
        private void _printPage(object sender, PrintPageEventArgs ev)
		{
			ev.Graphics.PageUnit = GraphicsUnit.Millimeter;
			int pageW = 95; //374;
			int pageH = 47; //177;
			//ev.PageSettings.PaperSize.Width = pageW;
			//ev.PageSettings.PaperSize.Height = pageH;
			//ev.PageSettings.Landscape = false;//.Width = 0;
			//ev.PageSettings.HardMarginX;
			/*ev.PageSettings.Margins.Left = 0;
			ev.PageSettings.Margins.Right = 0;
			ev.PageSettings.Margins.Top = 7;
			ev.PageSettings.Margins.Bottom = 0;*/
            SizeF size = ev.Graphics.MeasureString("WC", new Font("Arial", 12));
			float tw = size.Width;
			float th = size.Height;
			float areaW = ev.PageSettings.PrintableArea.Width;
			float areaH = ev.PageSettings.PrintableArea.Height;
			float ptsW = pageW;// / 100.0f;
			float ptsH = pageH;// / 100.0f;
			//ev.Graphics.DrawRectangle(new Pen(Brushes.Black, 1), 0, 0, ptsW, ptsH);
			//ev.Graphics.DrawRectangle(new Pen(Color.Black, 1), ptsW / 4, ptsH / 2, 0.05f, 0.05f);
			Pen pen = new Pen(Color.Black, 0.1f);
			ev.Graphics.DrawLine(pen, 0, 0, 95, 45);
			ev.Graphics.DrawEllipse(pen, -10, -10, 20, 20);
			ev.Graphics.DrawEllipse(pen, 85, 35, 20, 20);
            ev.Graphics.DrawString("WA", new Font("Arial", 12), Brushes.Black, new RectangleF(0, 0, tw, th));
            ev.Graphics.DrawString("WB", new Font("Arial", 12), Brushes.Black, new RectangleF(ptsW / 2 - tw/2, ptsH / 2 - th/2, tw, th));
            ev.Graphics.DrawString("WC", new Font("Arial", 12), Brushes.Black, new RectangleF(ptsW-tw, ptsH - th, tw, th));
			

                //foreach(object obj in _page.Content)
                //{
                //    if (obj is MText)
                //    {
                //        MText str = (MText)obj;                    
                //        ev.Graphics.DrawString(str.Text, str.Font, str.Brush, _calcGraphicsTextRectangle(ev.Graphics, str));
                //    }
                //}

                ev.HasMorePages = false;
        }

        private RectangleF _calcGraphicsTextRectangle(Graphics context, MText str)
        {
            SizeF strSize = context.MeasureString(str.Text, str.Font);

            RectangleF ret = new RectangleF();                        
            if (str.HorAlign == -1) // L
            {
                ret.X = str.Rectangle.Left;                
            }
            else if (str.HorAlign == 0) // C
            {
                ret.X = str.Rectangle.Left - strSize.Width / 2;
                
            }
            else // R
            {
                ret.X = str.Rectangle.Left - strSize.Width;                
            }
            ret.Width = strSize.Width;

            if (str.VerAlign == -1) // L
            {
                ret.Y = str.Rectangle.Top;

            }
            else if (str.VerAlign == 0) // C
            {
                ret.Y = str.Rectangle.Top - strSize.Height / 2;
            }
            else // R
            {
                ret.Y = str.Rectangle.Top - strSize.Height;
            }
            ret.Height = strSize.Height;

            return ret;
        }
    }
}
