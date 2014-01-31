using Com.SharpZebra;
using Com.SharpZebra.Printing;
using Com.SharpZebra.Commands;
using HardwareInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Drawing.Printing;
using System.Printing;
using System.IO;
using System.Text;

namespace ModuleZebraPrinter
{
    public class Page
    {
        public List<byte> PageBytes { set; get; }
        public double Width { set; get; }
        public double Height { set; get; }

        public Page(double width, double height)
        {
            PageBytes = new List<byte>();
            Width = width;
            Height = height;
        }
    }

    public class Printer: IStickerPrinter
    {
        private PrinterSettings _settings;
        private List<Page> _pages;
        private Page _currentPage;
        private ZebraFont _font;
        
        public void Init(string printerName)
        {
            _settings = new PrinterSettings();            
            _settings.PrinterName = printerName;
            _pages = new List<Page>();
            _font = ZebraFont.STANDARD_NORMAL;
        }

        private void _assertPriner()
        {
            if (_settings == null)
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

        public void SetFont(string name, float size)
        {
            _assertPriner();

            //_font = (ZebraFont)System.Enum.Parse(typeof(ZebraFont), name);
        }
        public void WriteText(string text, double x, double y, int alignHor, int alignVer, double angle, double maxWidth, double maxHeight, bool doShrink, bool doGrow)
        {
            _assertPage();

            _currentPage.PageBytes.AddRange(ZPLCommands.TextWrite(_floor(x), _floor(y), ElementDrawRotation.NO_ROTATION, ZebraFont.STANDARD_NORMAL, 15, 15, text));
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            throw new Exception("not implemented");
        }

        public void DrawLine(Point p1, Point p2)
        {
            throw new Exception("not implemented");
        }

        public void NewPage(double width, double height)
        {
            _assertPriner();

            _currentPage = new Page(width, height);
            _currentPage.PageBytes.AddRange(ZPLCommands.ClearPrinter(_settings));
            _pages.Add(_currentPage);            
        }

        public void Execute()
        {
            //_assertPriner();

            //foreach (Page page in _pages)
            //{
            //    page.PageBytes.AddRange(ZPLCommands.PrintBuffer(1));
            //    new SpoolPrinter(_settings).Print(page.PageBytes.ToArray());
            //}

            //PrinterSettings ps = new PrinterSettings();
            //ps.PrinterName = "ZDesigner GK420d";

            //List<byte> page = new List<byte>();
            //page.AddRange(ZPLCommands.ClearPrinter(ps));

            //page.AddRange(ZPLCommands.TextWrite(10, 150, ElementDrawRotation.NO_ROTATION, ZebraFont.STANDARD_NORMAL, 15, 15, "Hello World!"));
            //page.AddRange(ZPLCommands.LineWrite(0, 0, 10, 1000, 1000));

            //page.AddRange(ZPLCommands.PrintBuffer(1));
            //new SpoolPrinter(ps).Print(page.ToArray());


            //PrinterSettings ps = new PrinterSettings();
            //ps.PrinterName = "ZDesigner GK420d";
            //SpoolPrinter zm400 = new SpoolPrinter(ps);
            //zm400.Print(ZPLCommands.PrintBuffer(1));
            //zm400.Print(ZPLCommands.TextWrite(0, 0, ElementDrawRotation.NO_ROTATION, 10, "helloworld"));


            string ZPLString = "^XA^LH30,30^FO50,10^ADN,90,50^AD^A@,130,70,E:TT0003M_.FNT^FDВикипедия^FS^XZ";

            try
            {
                LocalPrintServer srv = new LocalPrintServer();
                PrintQueue defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                PrintSystemJobInfo myPrintJob = defaultPrintQueue.AddJob();
                Stream myStream = myPrintJob.JobStream;
                System.IO.StreamWriter writer = new System.IO.StreamWriter(myStream, Encoding.Unicode);
                writer.Write(ZPLString);

                writer.Flush();
                writer.Close();
                myStream.Close();
            }
            catch (Exception ex)
            {
                // Catch Exception
            }
        }

        string GetUnicodeString(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                sb.Append("\\u");
                sb.Append(String.Format("{0:x4}", (int)c));
            }
            return sb.ToString();
        }

        private int _floor(double value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }
    }
}
