using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using HardwareInterfaces;

namespace ModuleDrawingPrinter
{
    public class Printer : IStickerPrinter
    {
        private MPrinter _printer;
        private List<MPage> _pages;
        private MPage _currentPage;
        private Font _currentFont;
        private MConfig _conf;

        public Printer()
        {
            _conf = new MConfig();
            _pages = new List<MPage>();
            _currentFont = new Font("Arial", 12);
        }

        public void SetFont(string name, int size)
        {
            _assertPage();

            _currentFont = new Font(name, size);
        }

        public void WriteText(RectangleF bounds, int horAlign, int verAlign, string text, float angle)
        {
            _assertPage();

            bounds.Y += (float)_conf.yOrigin;
            MText txt = new MText(_currentPage, text, _currentFont, bounds, horAlign, verAlign, angle);            
            _currentPage.Content.Add(txt);
        }
        public void WriteBarcode(PointF origin, float height, int horAlign, int verAlign, string text)
        {
            _assertPage();
            origin.Y += (float)_conf.yOrigin;
            MBarcode barcode = new MBarcode(_currentPage, origin, height, horAlign, verAlign, text);
            _currentPage.Content.Add(barcode);
        }

        public void NewPage(float width, float height, int fieldLeft, int fieldTop, int fieldRight, int filedBottom)
        {
            _assertPriner();

            MPage page = new MPage(width, height, fieldLeft, fieldTop, fieldRight, filedBottom, _conf.yOrigin);
            _pages.Add(page);
            _currentPage = page;
        }
        public void Init(string printerName)
        {
            _printer = new MPrinter(printerName, new System.Drawing.Printing.PaperSize("custom", 500, 500));
        }

        private void _assertPriner()
        {
            if (_printer == null)
            {
                throw new Exception("Принтер не задан");
            }
        }

        private void _assertPage()
        {
            if (_currentPage == null)
            {
                throw new Exception("Страница не создана");
            }
        }

        public void Execute()
        {
            _printer.Print(_pages);
        }
    }
}
