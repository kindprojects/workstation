using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;

using ModuleConnect;

namespace ModuleNamespace
{
    public class Printer :IDisposable
    {
        private string _printerName;
        private List<MPage> _pages;
        PrintDocument _sessionDoc;
        private MPage _sessionPage;
        private MPage _currentPage;
        private Font _currentFont;
        //private MConfig _conf;

        public Printer(string printerName)
        {
            _printerName = printerName;
            _pages = new List<MPage>();
            _currentFont = new Font("Arial", 12);
        }
		public void Dispose()
		{
			this.Dispose(false);
		}
		protected virtual void Dispose(bool cleanManaged){
			_sessionDoc.Dispose();
			_currentFont.Dispose();
		}

        public void SetFont(string name, float size)
        {
            _assertPage();

            _currentFont = new Font(name, size);
        }

        public void WriteText(RectangleF bounds, int horAlign, int verAlign, string text, float angle)
        {
            _assertPage();

            bounds.Y += _currentPage.OriginY;
            MText txt = new MText(_currentPage, text, _currentFont, bounds, horAlign, verAlign, angle);            
            _currentPage.Content.Add(txt);
        }
        public void WriteBarcode(PointF origin, float height, int horAlign, int verAlign, string text)
        {
            _assertPage();
            origin.Y += _currentPage.OriginY;
            MBarcode barcode = new MBarcode(_currentPage, origin, height, horAlign, verAlign, text);
            _currentPage.Content.Add(barcode);
        }

        public void NewPage(float width, float height, int fieldLeft, int fieldTop, int fieldRight, int filedBottom, float originY)
        {
            MPage page = new MPage(width, height, fieldLeft, fieldTop, fieldRight, filedBottom, originY);
            _pages.Add(page);
            _currentPage = page;
        }
        //public void Init(string printerName)
        //{
        //    _printer = new MPrinter(printerName);
        //}

        private void _assertPage()
        {
            if (_currentPage == null)
            {
                throw new Exception("Страница не создана");
            }
        }

        public void Print(List<MPage> pages)
        {
            try
            {
                foreach (MPage page in pages)
                {
                    //_pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 350, 50);
                    _sessionPage = page;
                    _sessionDoc.Print();
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

            foreach (object obj in _sessionPage.Content)
            {
                if (obj is MText)
                {
                    MText label = (MText)obj;

                    ev.Graphics.TranslateTransform(label.X, label.Y);
                    ev.Graphics.RotateTransform(label.Angle);
                    ev.Graphics.DrawString(label.Text, label.Font, label.Brush, label.CalcPositionFromSelfOrigin(ev.Graphics), label.GetStringFormat());
                    ev.Graphics.ResetTransform();
                }
                else if (obj is MBarcode)
                {
                    MBarcode barcode = (MBarcode)obj;
                    ev.Graphics.DrawImage(barcode.Render(ev.Graphics), new PointF(barcode.X, barcode.Y));
                }
            }

            ev.HasMorePages = false;
        }

        public void Print()
        {
            _sessionDoc = new PrintDocument();
            _sessionDoc.PrinterSettings.PrinterName = _printerName;
            _sessionDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            _sessionDoc.PrintPage += new PrintPageEventHandler(this._printPage);

            try
            {
                foreach (MPage page in _pages)
                {
                    //_pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 350, 50);
                    _sessionPage = page;
                    _sessionDoc.Print();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка принтера. {0}", ex.Message));
            }
        }
    }
}
