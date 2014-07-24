using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    internal interface IRepCollection
    {
        int? Get(int objectId, string code, bool createIfNotExist);
    }
}
