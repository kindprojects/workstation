using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    internal interface IModel
    {
        int Id { set; get; }
        string Code { set; get; }
        int ObjectIdRoot { set; get; }
    }
}
