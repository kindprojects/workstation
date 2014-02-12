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
        private List<object> _content;

        public MPrinter()
        {
            _pd = new PrintDocument();
            _pd.PrintPage += new PrintPageEventHandler
                   (this._printPage);

        }

        public void Print(string printerName, List<object> content)
        {
            _content = content;

            try
            {
                _pd.Print();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка принтера. {0}", ex.Message));
            }
                    
        }
        private void _printPage(object sender, PrintPageEventArgs ev)
        {
            foreach(object obj in _content)
            {
                // print
            }
        }
    }
}
