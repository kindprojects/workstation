using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace ModuleDrawingPrinter
{
    class MConfig
    {
        public double yOrigin;

        public MConfig()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            MConfigFontsSection section = (MConfigFontsSection)config.GetSection("printerSettings");          
            yOrigin = _getDouble(section.SettingsItems[0].Value);
        }

        private double _getDouble(string value)
        {
            var currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
            var numberFormat = (System.Globalization.NumberFormatInfo)currentCulture.NumberFormat.Clone();
            numberFormat.NumberDecimalSeparator = ".";

            double number;
            bool succeeded = double.TryParse(value, System.Globalization.NumberStyles.Any, numberFormat, out number);
            if (succeeded)
            {
                return number;
            }
            else
            {
                throw new Exception(string.Format("Значение {0} имеет неверный формат. Невозможно преобразовать в double", value));
            }
        }
    }
}
