using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    class Object : IObject
    {
        public int Id { set; get; }
        public string CollectionCode { set; get; }
        public string CollectionPos { set; get; }
        public string MatchedValue { set; get; }
    }
}
