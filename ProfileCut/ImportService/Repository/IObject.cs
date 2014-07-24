using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    interface IObject
    {
        int Id { set; get; }
        string CollectionCode { set; get; }

        string CollectionPos { set; get; }

        string MatchedValue { set; get; }
    }
}
