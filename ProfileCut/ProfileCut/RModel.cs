using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Platform2;
using Repository;

namespace ProfileCut
{
    public class RModel
    {
        public IPObject Root { set; get; }

        public RModel(string connectionString, string modelCode, bool defferedLoad, IPHost host)
        {
            Root = new PPlatform().GetRoot(
                new SRepositoryDb(connectionString), 
                modelCode, 
                defferedLoad, 
                host);                        
        }        
    }
}
