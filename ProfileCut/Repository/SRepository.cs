using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SRepository
    {
        public ISRepository Storage { set; get; }

        public SRepository(ISRepository storage)
        {
            Storage = storage;
        }
    }
}
