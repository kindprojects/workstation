using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class SRepositoryLinq : SRepository
    {
        public SRepositoryLinq(string connectionString) :
            base(new SStorageLinq(connectionString))
        {

        }
    }
}
