using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SRepositoryXml : SRepository
    {
        public SRepositoryXml(string fileName) : 
            base(new SStorageXml(fileName))
        {
        }
    }
}
