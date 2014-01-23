using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Platform;

namespace ProfileCut
{
    public class RProfileCut
    {
        private PPlatform _platform;
        private RFbLink _fbDb;
        private RConfig _conf;

        public RProfileCut()
        {
            _conf = new RConfig();
            _fbDb = new RFbLink(_conf.ConnectionString);
            _platform = new PPlatform(_fbDb, _conf.ModelCode, true)
        }     

        public string[] GetMasterList()
        {
            _platform.                                                            
        }
    }
}
