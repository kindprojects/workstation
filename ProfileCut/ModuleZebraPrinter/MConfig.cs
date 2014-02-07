using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace ModuleZebraPrinter
{
    class MConfig
    {
        public double Dpm;
        //public int XHomePos;
        //public int YHomePos;

        private Dictionary<string, MFont> _fonts;

        public MConfig()
        {
            _fonts = new Dictionary<string, MFont>();

            Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            MConfigFontsSection section = (MConfigFontsSection)config.GetSection("printerSettings");
            
            Dpm = _getDouble(section.SettingsItems[0].Value);

            foreach (MFontElement item in section.FontItems)
            {
                _fonts.Add(item.Key.ToLower(), _createFont(item));
            }
        }

        private double _getDouble(string value)
        {
            var currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
            var numberFormat = (System.Globalization.NumberFormatInfo) currentCulture .NumberFormat.Clone();
            numberFormat .NumberDecimalSeparator = "."; 

            double number;
            bool succeeded = double.TryParse(value, System.Globalization.NumberStyles.Any, numberFormat , out number);
            if (succeeded) 
            {
                return number;
            }
            else 
            {
                throw new Exception(string.Format("Значение {0} имеет неверный формат. Невозможно преобразовать в double", value));
            }
        }


        public MFont GetFont(string name)
        {
            MFont font;
            _fonts.TryGetValue(name.ToLower(), out font);

            return font;
        }

        private MFont _createFont(MFontElement item)
        {
            try
            {
                MFont font = new MFont()
                {
                    Name = item.Name,
                    Height = Convert.ToInt32(item.Height),
                    Width = Convert.ToInt32(item.Width)
                };

                return font;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Не удалось получить шрифт. {0}", ex.Message));
            }
        }       
    }
}
