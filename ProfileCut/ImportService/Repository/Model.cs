using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    internal class Model : IModel
    {
        public int Id {set; get; }
        public string Code { set; get; }
        public int ObjectIdRoot { set; get; }
    }
}
