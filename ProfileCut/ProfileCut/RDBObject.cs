using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class RDBObject : RBaseObject
    {
        public int Id { set; get; }

        public RDBObject(int id, RDBObject owner, IDataModel model)
            : base(owner, model)
        {
            this.Id = id;
        }
    }
}
