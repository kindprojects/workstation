using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;

namespace Platform2
{
    public class PModel
    {
        public IPObject GetRoot(SRepository repository, string model, bool deferredLoad, IPHost host)
        {
            return new PObject(repository, model, deferredLoad, host);
        }
    }
}
