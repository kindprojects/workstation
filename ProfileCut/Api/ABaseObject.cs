using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    class ABaseObject
    {
        int Id { set; get; }

        public List<ABaseObject> GetObjects()
        {
            List<ABaseObject> list = new List<ABaseObject>();

            return list;
        }

        public ABaseObject GetObjectByIndex(int index)
        {

        }
    }
}
