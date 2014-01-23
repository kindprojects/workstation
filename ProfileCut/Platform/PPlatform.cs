using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Platform
{
    public class PPlatform
    {
        private PModel _model;
        public PPlatform(IPDBLink db, string modelCode, bool defferedLoad)
        {
            _model = new PModel(db, modelCode, defferedLoad);
        }

        public List<T> GetCollectionObjects<T>(string collectionName, bool createIfNotFound)
        {
            List<PBaseObject> list = new List<PBaseObject>();

            return list;
        }    
    }
}
