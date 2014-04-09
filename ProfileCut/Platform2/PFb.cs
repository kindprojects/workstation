using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;

namespace Platform2
{
    public class PFb
    {
        public IPObject Root { set; get; }

        public PFb(string connectionString, string model, bool deferredLoad, IPHost host)
        {
            Root = new PObject(new SRepositoryFb(connectionString), model, deferredLoad, host);
        }
    }
}
