using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using GenCode128;

namespace ModuleDrawingPrinter
{
    class MPrinter
    {        
        private PrintDocument _pd;
        private MPage _page;
        
        public MPrinter(string printerName, PaperSize size)
        {     
            _pd = new PrintDocument();
            _pd.PrinterSettings.PrinterName = printerName;
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

            foreach (object obj in _page.Content)
            {
                if (obj is MText)
                {
                    MText label = (MText)obj;

                    ev.Graphics.TranslateTransform(label.X, label.Y);
                    ev.Graphics.RotateTransform(label.Angle);
                    ev.Graphics.DrawString(label.Text, label.Font, label.Brush, label.CalcPositionFromSelfOrigin(ev.Graphics));
                    ev.Graphics.ResetTransform();

                    //ev.PageSettings.PrinterResolution.Kind
                }
                else if (obj is MBarcode)
                {
                    MBarcode barcode = (MBarcode)obj;
                    //Image img = Code128Rendering.MakeBarcodeImage(barcode.Text, 1, true);
                    ev.Graphics.DrawImage(barcode.Render(ev.Graphics), new PointF(barcode.X, barcode.Y));
                }
            }

            ev.HasMorePages = false;
        }
    }
}
