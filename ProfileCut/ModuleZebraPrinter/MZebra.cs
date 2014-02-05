using HardwareInterfaces;
using System;
using System.Collections.Generic;
using System.Printing;
using System.IO;
using System.Text;

using System.Configuration;

namespace ModuleZebraPrinter
{
    public class MPoint
    {
        public double X { set; get; }
        public double Y { set; get; }
    }

    public class MZebraPrinter
    {
        public void Print(string content)
        {
            try
            {
                LocalPrintServer srv = new LocalPrintServer();
                PrintQueue defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                PrintSystemJobInfo myPrintJob = defaultPrintQueue.AddJob();
                Stream myStream = myPrintJob.JobStream;
                System.IO.StreamWriter writer = new System.IO.StreamWriter(myStream, Encoding.Unicode);
                writer.Write(content);

                writer.Flush();
                writer.Close();
                myStream.Close();
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
        public int Dpi { set; get; }
        public int XHomePos { set; get; }
        public int YHomePos { set; get; }

        public MPage(double width, double height, int xHomePos, int yHomePos)
        {
            Width = width;
            Height = height;
            XHomePos = xHomePos;
            YHomePos = yHomePos;

            _pageStrings = new List<string>();
        }

        public string GetContentPage()
        {           
            string page = "^XA";
            page += _clearPrinter();
            foreach(string line in _pageStrings)
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
            return string.Format("^MMT^LH{0},{1}^PW750", XHomePos, YHomePos);
       }
    }

    public class Printer: IStickerPrinter
    {
        private MZebraPrinter _printer;
        private List<MPage> _pages;
        private MPage _currentPage;
        private MFont _font;
        private MConfig _conf;
        
        public void Init()
        {
            
            _conf = new MConfig();
            _printer = new MZebraPrinter();
            _pages = new List<MPage>();

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

            int xInPixel = _getInPixel(x, _currentPage.Width);
            int yInPixel = _getInPixel(y, _currentPage.Height);
            int widthInPixel = _getInPixel(width, _currentPage.Width);

            if (align.ToUpper().Trim() == "R")
            {
                xInPixel = xInPixel - widthInPixel;
                string blockDefZpl = string.Format("^FB{0},,,{1},", widthInPixel, align);
                _currentPage.AddLine(blockDefZpl);
            }

            string textZpl = string.Format("^FW{0}^FO{1},{2}^FD{3}^FS", _getOreintation(angle), xInPixel, yInPixel, text, align);
            _currentPage.AddLine(textZpl);        

        }

        public void WriteBarcode(string text, double x, double y, double height, double width)
        {
            _assertPage();
            MPoint coord = _getCoordInPixel(x, y);

            string zpl = string.Format("^FO{0},{1}^BY{2}^BCN,{3},N,N,N^FD>;{4}^FS", _floor(coord.X), _floor(coord.Y), _floor(width), _floor(height), text);                                             
            _currentPage.AddLine(zpl);
        }

        public void NewPage(double width, double height)
        {
            _assertPriner();

            _currentPage = new MPage(width, height, _conf.XHomePos, _conf.YHomePos);
            _pages.Add(_currentPage);            
        }       

        public void Execute()
        {
            _assertPriner();
            foreach(MPage page in _pages)
            {
                _printer.Print(page.GetContentPage());
            }
        }

        private int _floor(double value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        private MPoint _getCoordInPixel(double x, double y)
        {
            return new MPoint() {
                X = _getInPixel(x, _currentPage.Width),
                Y = _getInPixel(y, _currentPage.Height)
            };
        }

        private int _getInPixel(double value, double width)
        {
            return _floor((width * value / 100) * _conf.Dpm);
        }

        private string _getOreintation(int angle)
        {
            string oreintation;
            if (angle < 90)
            {
                oreintation = "N";
            }
            else if (angle >= 90 && angle < 180 )
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
