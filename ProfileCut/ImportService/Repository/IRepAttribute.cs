using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    internal interface IRepAttribute
    {
        void Set(int objectId, string code, string val);
    }
}
