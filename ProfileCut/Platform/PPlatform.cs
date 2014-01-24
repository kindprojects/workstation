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

        public List<IPBaseObject> GetCollectionObjects(string masterCollection)
        {
            List<IPBaseObject> list = new List<IPBaseObject>();

            PCollection collection = _model.Data.GetCollection(masterCollection, false);
            for (int ii = 0; ii < collection.Count(); ii ++ )
            {
                list.Add(collection.GetObject(ii));
            }

            return list;
        }        

        public string Transform(string template, IPBaseObject obj)
        {
            string path = "";
            return _model.Templates.TransformText(template, (PBaseObject)obj, ref path, false);
        }
    }
}
