using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportService
{
    public class PAttribute
    {
        public string Name { set; get; }
        public string Value { set; get; }
    }

    public class PCollection
    {
        public string Name { set; get; }
        public List<PObject> Objects { set; get; }

        public PCollection()
        {
            Objects = new List<PObject>();
        }
    }

    public class PObject
    {
        public List<PAttribute> Attributes { set; get; }

        public List<PCollection> Collections { set; get; }

        public PObject()
        {
            Attributes = new List<PAttribute>();
            Collections = new List<PCollection>();
        }
    }


    public class PObjectContainer
    {
        public string ModelCode { set; get; }
        public string CollectionPath { set; get; }
        public List<PObject> Objects { set; get; }

        public PObjectContainer()
        {
            Objects = new List<PObject>();
        }
    }
}
