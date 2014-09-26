
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService
{
    public static class Extensions
    {
        public static bool IsEven(this int val)
        {
            return (val % 2) == 0;
            //return (val & 1) == 0;
        }
    }
}
