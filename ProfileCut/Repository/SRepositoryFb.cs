using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SRepositoryFb : SRepository
    {
        public SRepositoryFb(string connectionString) : 
            base (new SStorageFB(connectionString))
        {

        }
    }
}
