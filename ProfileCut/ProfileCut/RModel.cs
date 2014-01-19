using System;
using System.Collections.Generic;


namespace Model
{
    public interface IDataModel
    {
        // интерфейс модели для её объектов
        bool FillCollection(RCollection coll);
        RCollection NewCollection(RBaseObject owner, string name);
        RBaseObject FindObjectById(int id, RBaseObject where);
    }

    public class RModel : IDataModel
    {
        public RBaseObject Data;
        public RTemplates Templates;
        private IDBLink _db;
        private string _modelCode;
        public bool DeferredLoad = true; // отложенная загрузка. Если false, то содержимое всей модели прогружается целиком, иначе частично, при обращении (для тяжёлых моделей)
        public Dictionary<int, RBaseObject> _listObjects;

        public RModel(IDBLink db, string modelCode, bool deferredLoad)
        {
            this._db = db;
            this._modelCode = modelCode;
            this._listObjects = new Dictionary<int, RBaseObject>();
            this.Templates = new RTemplates();
            this.DeferredLoad = deferredLoad;
            this.LoadModel(); // хз?
        }

        public RBaseObject FindObjectById(int id, RBaseObject where)
        {
            RBaseObject ret = null;
            if (_listObjects.TryGetValue(id, out ret))
            {
                return ret.IsChildOf(where) ? ret : null;
            }else 
                return null;
        }
        
        private RBaseObject GetRootObject()
        {
            int rootID;
            rootID = _db.GetModelRootID(this._modelCode, -1);
            if (rootID == -1)
                throw new Exception("Модель с кодом \""+this._modelCode+"\" не найдена!");
            return this.LoadNewObject(rootID, null);
        }

        private RBaseObject LoadNewObject(int id, RCollection ownerCollection)
        {
            RBaseObject obj = new RBaseObject(id, ownerCollection, this);
            if (ownerCollection != null)
                ownerCollection.InsertObject(obj);
            Dictionary<string, string> attrs = this._db.ListAttributes(id);
            foreach (KeyValuePair<string, string> p in attrs)
            {
                obj.SetAttr(p.Key, p.Value);
            }
            
            List<string> listColl = this._db.ListCollections(id);
            
            foreach(string coll in listColl) 
            {
                obj.GetCollection(coll, true);
            }

            _listObjects.Add(obj.Id, obj);
            return obj;
        }

        public bool FillCollection(RCollection coll)
        {
            List<int> lst = this._db.ListCollectionObjects(coll.Owner.Id, coll.Name);
            foreach(int id in lst){
                this.LoadNewObject(id, coll);
            }
            return true;
        }

        public RCollection NewCollection(RBaseObject owner, string name)
        {
            RCollection col = new RCollection(owner, name, this.DeferredLoad, this);
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
