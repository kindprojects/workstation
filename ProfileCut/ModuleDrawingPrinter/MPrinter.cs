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
                    _pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 350, 50);
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
            SizeF size = ev.Graphics.MeasureString("WC", new Font("Arial", 12));
            ev.Graphics.DrawString("WA", new Font("Arial", 12), Brushes.Black, new RectangleF(0, 0, 40, 40));
            ev.Graphics.DrawString("WB", new Font("Arial", 12), Brushes.Black, new RectangleF(_pd.DefaultPageSettings.PaperSize.Width / 2, _pd.DefaultPageSettings.PaperSize.Height / 2, 40, 40));
            ev.Graphics.DrawString("WC", new Font("Arial", 12), Brushes.Black, new RectangleF(_pd.DefaultPageSettings.PaperSize.Width - size.Width, _pd.DefaultPageSettings.PaperSize.Height - size.Height, 40, 40));



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
