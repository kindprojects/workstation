using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;

namespace Platform2
{
    class PXml
    {
        public IPObject Root { set; get; }

        public PXml(string fileName, string model, bool deferredLoad, IPHost host)
        {
            Root = new PObject(new SRepositoryXml(fileName), model, deferredLoad, host);
        }
    }
}
