using HardwareInterfaces;
using System;
using System.Collections.Generic;
using System.Printing;
using System.IO;
using System.Text;

using System.Configuration;

namespace ModuleZebraPrinter
{
    public class MZebraPrinter
    {
        public void Print(string printerName, string content)
        {
            try
            {
                using (PrintServer ps = new PrintServer())
                {
                    using (PrintQueue pq = new PrintQueue(ps, printerName, PrintSystemDesiredAccess.AdministratePrinter))
                    {
                        using (PrintQueueStream pqs = new PrintQueueStream(pq, Guid.NewGuid().ToString()))
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(pqs, Encoding.Unicode))
                            {
                                writer.Write(content);

                                writer.Flush();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка печати. " + ex.Message);
            }
        }
    }

    public class MPage
    {
        public List<string> _pageStrings { set; get; }
        public double Width { set; get; }
        public double Height { set; get; }
        public double Dpm { set; get; }
        public int FieldLeft { set; get; }
        public int FieldTop { set; get; }
        public int FieldRight { set; get; }
        public int FieldBottom { set; get; }

        public MPage(double dpm, double width, double height, int fieldLeft, int fieldTop, int fieldRight, int fieldBottom)
        {
            Dpm = dpm;

            Width = (width - fieldLeft - fieldRight) * dpm;
            Height = (height - fieldTop - fieldBottom) * dpm;

            FieldLeft = Convert.ToInt32(Math.Floor(fieldLeft * dpm));
            FieldTop = Convert.ToInt32(Math.Floor(fieldTop * dpm));
            FieldRight = Convert.ToInt32(Math.Floor(fieldRight * dpm));
            FieldBottom = Convert.ToInt32(Math.Floor(fieldBottom * dpm));

            _pageStrings = new List<string>();
        }

        public string GetContentPage()
        {
            string page = "^XA";
            page += _clearPrinter();
            foreach (string line in _pageStrings)
            {
                page += line;
            }

            page += "^XZ";
            return page;
        }

        public void AddLine(string line)
        {
            _pageStrings.Add(line);
        }

        private string _clearPrinter()
        {
            return string.Format("^MMT^LH{0},{1}^PW750", FieldLeft, FieldTop + 18);
        }
    }

    public class Printer : IStickerPrinter
    {
        private MZebraPrinter _printer;
        private List<MPage> _pages;
        private MPage _currentPage;
        private MFont _font;
        private MConfig _conf;
        private string _printerName;

        public void Init(string printerName)
        {

            _conf = new MConfig();
            _printer = new MZebraPrinter();
            _pages = new List<MPage>();
            _printerName = printerName;

            // шрифт по умолчаню
            _font = new MFont()
            {
                Height = 20,
                Width = 20,
                Name = "E:TT0003M_.FNT"
            };
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

        public void SetFont(string name)
        {
            _assertPage();

            MFont font = _conf.GetFont(name);
            if (font != null)
            {
                _font = font;
            }
        }

        public void WriteText(string text, double x, double y, int angle, string align, double width)
        {
            _assertPage();

            string fntZpl = string.Format("^A@,{0},{1},{2}", _font.Height, _font.Width, _font.Name);
            _currentPage.AddLine(fntZpl);

            int widthInPixel = _getInPixel(width, _currentPage.Width);
            string blockDefZpl = string.Format("^FB{0},,,{1},", widthInPixel, align.ToUpper().Trim());
            _currentPage.AddLine(blockDefZpl);

            int xInPixel = _getAlignX(_getInPixel(x, _currentPage.Width), widthInPixel, align);
            int yInPixel = _getInPixel(y, _currentPage.Height);

            string textZpl = string.Format("^FW{0}^FO{1},{2}^FD{3}^FS", _getOreintation(angle), xInPixel, yInPixel, text, align);
            _currentPage.AddLine(textZpl);

        }

        private int _getAlignX(int x, int width, string align)
        {
            if (align.ToUpper().Trim() == "R")
            {
                return x - width;
            }
            else if (align.ToUpper().Trim() == "C")
            {
                return x - width / 2;
            }
            else
            {
                return x;
            }
        }

        public void WriteBarcode(string text, double x, double y, double width, double height)
        {
            _assertPage();

            string zpl = string.Format("^FO{0},{1}^BY{2}^BCN,{3},N,N,N^FD>;{4}^FS",
                _floor(_getInPixel(x, _currentPage.Width)),
                _floor(_getInPixel(y, _currentPage.Height)),
                width,
                _floor(_getInPixel(height, _currentPage.Height)),
                text
            );

            _currentPage.AddLine(zpl);
        }

        public void NewPage(double width, double height, int fieldLeft, int fieldTop, int fieldRight, int fieldBottom)
        {
            _assertPriner();

            _currentPage = new MPage(_conf.Dpm, width, height, fieldLeft, fieldTop, fieldRight, fieldBottom);
            _pages.Add(_currentPage);
        }

        public void Execute()
        {
            _assertPriner();
            foreach (MPage page in _pages)
            {
                _printer.Print(_printerName, page.GetContentPage());
            }
        }

        private int _floor(double value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        private int _getInPixel(double value, double width)
        {
            return _floor((width * value / 100));
        }

        private string _getOreintation(int angle)
        {
            string oreintation;
            if (angle < 90)
            {
                oreintation = "N";
            }
            else if (angle >= 90 && angle < 180)
            {
                oreintation = "R";
            }
            else if (angle >= 180 && angle < 270)
            {
                oreintation = "B";
            }
            else
            {
                oreintation = "N";
            }

            return oreintation;
        }
    }
}
