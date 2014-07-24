using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService.Repository
{
    interface IRepModel
    {
        IModel Select(string code);
    }
}
