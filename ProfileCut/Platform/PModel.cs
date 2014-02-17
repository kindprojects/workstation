using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{    
    public class PModel : IPDataModel
    {
        public PBaseObject Data;
        public PTemplates Templates;
        private IPDBLink _db;
        private string _modelCode;

        // отложенная загрузка. Если false, то содержимое всей модели прогружается целиком, иначе частично, при обращении (для тяжёлых моделей)
        public bool DeferredLoad = true; 

        public Dictionary<int, PBaseObject> _listObjects;

        public PModel(IPDBLink db, string modelCode, bool deferredLoad)
        {
            this._db = db;
            this._modelCode = modelCode;
            this._listObjects = new Dictionary<int, PBaseObject>();
            this.Templates = new PTemplates();
            this.DeferredLoad = deferredLoad;
            this.LoadModel(); // хз?
        }

        public PBaseObject FindObjectById(int id, PBaseObject where)
        {
            PBaseObject ret = null;
            if (_listObjects.TryGetValue(id, out ret))
            {
                //return ret.IsChildOf(where) ? ret : null;
                return ret;
            }
            else
                return null;
        }

        private PBaseObject GetRootObject()
        {
            int rootID;
            rootID = _db.GetModelRootID(this._modelCode, -1);
            if (rootID == -1)
                throw new Exception("Модель с кодом \"" + this._modelCode + "\" не найдена!");
            return this.LoadNewObject(rootID, null);
        }

        private PBaseObject LoadNewObject(int id, PCollection ownerCollection)
        {
            PBaseObject obj = new PBaseObject(id, ownerCollection, this);

            if (ownerCollection != null)
                ownerCollection.InsertObject(obj);
            Dictionary<string, string> attrs = this._db.ListAttributes(id);
            foreach (KeyValuePair<string, string> p in attrs)
            {
                obj.SetAttr(p.Key, p.Value);
            }

            List<string> listColl = this._db.ListCollections(id);

            foreach (string coll in listColl)
            {
                obj.GetCollection(coll, true);
            }

            _listObjects.Add(obj.Id, obj);
            return obj;
        }

        public bool FillCollection(PCollection coll)
        {
            List<int> lst = this._db.ListCollectionObjects(coll.Owner.Id, coll.Name);
            foreach (int id in lst)
            {
                this.LoadNewObject(id, coll);
            }
            return true;
        }

        public PCollection NewCollection(PBaseObject owner, string name)
        {
            PCollection col = new PCollection(owner, name, this.DeferredLoad, this);
            if (!this.DeferredLoad)
            {
                this.FillCollection(col);
            }

            return col;
        }

        public void LoadModel()
        {
            this.Templates.Clear();
            Dictionary<string, string> tList = this._db.ListTemplates(this._modelCode);
            foreach (KeyValuePair<string, string> p in tList)
            {
                this.Templates.AddTemplate(p.Key, p.Value);
            }

            this.Data = this.GetRootObject();
        }
    }

}
