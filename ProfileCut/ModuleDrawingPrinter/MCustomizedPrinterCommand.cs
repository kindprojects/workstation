using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModuleNamespace
{
    public struct Fields
    {
        public int left, right, top, bottom;
        public Fields(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }

    public class MCustomizedPrinterCommand : MPrinterCommand
    {
        public MCustomizedPrinterCommand(string commandString) : base(commandString) { }

        public int GetParamAlign(string paramName, int? def = null)
        {
            return (int)Math.Truncate(this.GetParamLimited(paramName, -1, 1, def));
        }

        public Fields GetParamFields(string paramName, string def)
        {
            List<int> fields = this.GetParamListInt(paramName, def);
            int cnt = fields.Count();
            while (fields.Count < 4)
                fields.Add(fields[0]);
            Fields f = new Fields(fields[0], fields[1], fields[2], fields[3]);
            if (cnt == 1)
            {
                f.top = f.left;
                f.right = f.left;
                f.bottom = f.left;
            }
            else if (cnt == 2)
            {
                f.right = f.left;
                f.bottom = f.top;
            }
            else if (cnt == 3)
            {
                f.bottom = f.top;
            }
            else
            {
                throw new Exception("Параметр " + paramName + " должен состоять из 1,2,3 либо 4 целых чисел, разделённых запятой");
            }
            return f;
        }

        public float GetParamPercent(string paramName, float? def = null)
        {
            return this.GetParamLimited(paramName, 0, 100, def);
        }

        public float GetParamFloatPositive(string paramName, float? def = null)
        {
            float val = this.GetParamFloat(paramName, def);
            if (val <= 0)
                throw new Exception("Значение параметра " + paramName + " должно быть строго положительным (больше нуля)");
            return val;
        }
        public int GetParamIntPositive(string paramName, int? def = null)
        {
            int val = this.GetParamInt(paramName, def);
            if (val <= 0)
                throw new Exception("Значение параметра " + paramName + " должно быть строго положительным (больше нуля)");
            return val;
        }
    }
}
