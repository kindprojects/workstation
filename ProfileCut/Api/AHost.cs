using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Platform;

namespace Api
{
    public class AHost : IPHost
    {
        public IAHost Host { set; get; }

        public AHost(IAHost host)
        {
            Host = host;
        }

        public string QueryToHost(string text)
        {
            return this.Host.QueryToHost(text);
        }        
    }
}
