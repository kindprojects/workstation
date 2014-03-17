using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;

namespace Platform2
{
    internal class PObject : IPObject   
    {
        public int Id { set; get; }

        internal PTemplates Templates { set; get; }

        public IPHost Host { set; get; }

        private PCollection _ownerCollection;

        private SRepository _repository;

        private Dictionary<int, PObject> _listObjects;

        private Dictionary<string, string> _attrs;

        private List<PCollection> _collections;

        private bool _deferredLoad;

        private PNavigator _navigator;
                
        // root
        public PObject(SRepository repository, string model, bool deferredLoad, IPHost host)
        {
            if (repository == null)
                throw new Exception("Хранилище данных не задано");

            _repository = repository;

            Id = _getRootObjectId(model);                         
            Templates = new PTemplates(this);
            Host = host;

            _listObjects = new Dictionary<int,PObject>();
            _ownerCollection = null;
            _deferredLoad = deferredLoad;
            
            _attrs = _repository.Implement.ListAttributes(Id);

            _loadCollections(_deferredLoad);
        }

        // submission            
        internal PObject(SRepository repository, PCollection ownerCollection, int id, Dictionary<int, PObject> listObjects, bool deferredLoad, IPHost host)
        {
            if (repository == null)
                throw new Exception("Хранилище данных не задано");

            Id = id;
            Templates = new PTemplates(this);
            Host = host;

            _repository = repository;
            _listObjects = listObjects;
            _deferredLoad = deferredLoad;
            _ownerCollection = ownerCollection;
            
            _attrs = _repository.Implement.ListAttributes(Id);
            _collections = new List<PCollection>();
        }

        private void _loadCollections(bool defferendLoad)
        {
            _collections = new List<PCollection>();
            List<string> collectsNames = _repository.Implement.ListCollections(Id);
            foreach(string name in collectsNames)            
            {
                PCollection collection = new PCollection(this, name, _deferredLoad);
                _collections.Add(collection);
                if (!defferendLoad)
                    _fillCollection(collection);
            }
        }

        private int _getRootObjectId(string model)
        {
            int id;
            id = _repository.Implement.RootObjectId(model, -1);
            if (id == -1)
                throw new Exception("Модель с кодом \"" + model + "\" не найдена!");

            return id;
        }

        public bool GetAttr(string name, bool findInOwners, out string val)
        {
            if (_attrs.TryGetValue(name.ToLower(), out val))
            {
                return true;
            }
            else if (this._ownerCollection != null && findInOwners) // рекурсивно ищем во всех владельцах
            {
                return this._ownerCollection.OwnerObject.GetAttr(name, true, out val);
            }
            else
            {
                return false;
            }
        }

        internal bool IsChildOf(PObject obj)
        {
            if (obj == null)
                return false;
            PCollection coll = this._ownerCollection;
            while (coll != null)
            {
                if (coll.OwnerObject == obj)
                    return true;
                coll = coll.OwnerObject._ownerCollection;
            }
            return false;
        }

        internal PNavigatorPath GetPathTo(PObject toObject)
        {
            PNavigatorPath path = new PNavigatorPath();

            if (toObject._ownerCollection == null)
                return null;

            PObject obj = toObject;
            while (obj._ownerCollection.OwnerObject != null)
            {
                path.Parts.Insert(0, new PNavigatorPart(obj._ownerCollection.Name, obj._ownerCollection.IndexOf(obj)));
                if (obj._ownerCollection.OwnerObject == this)
                {
                    return path;
                }

                obj = obj._ownerCollection.OwnerObject;
            }
            
            return null;
        }

        internal PCollection GetCollection(string name, bool createIfNotFound)
        {
            PCollection coll;

            coll = this._findCollection(name);
            if (coll != null)
            {
                return coll;
            }
            else
            {
                if (createIfNotFound)
                {
                    coll = _newCollection(this, name.ToLower());
                    _collections.Add(coll);
                    return coll;
                }
                else
                {
                    return null;
                }
            }
        }

        private PCollection _findCollection(string name)
        {
            name = name.ToLower();

            foreach (PCollection c in _collections)
            {
                if (c.Name.ToLower() == name)
                    return c;
            }
            return null;
        }

        private bool _fillCollection(PCollection collection)
        {
            List<int> lst = _repository.Implement.ListCollectionObjects(collection.OwnerObject.Id, collection.Name);
            foreach (int id in lst)
            {
                this._loadNewObject(id, collection);
            }
            return true;
        }

        private PCollection _newCollection(PObject owner, string name)
        {
            PCollection collection = new PCollection(owner, name, _deferredLoad);
            if (!_deferredLoad)
            {
                _fillCollection(collection);
            }

            return collection;
        }

        private PObject _loadNewObject(int id, PCollection ownerCollection)
        {
            PObject obj = new PObject(this._repository, ownerCollection, id, this._listObjects, _deferredLoad, this.Host);

            if (ownerCollection != null)
                ownerCollection.InsertObject(obj);
            Dictionary<string, string> attrs = _repository.Implement.ListAttributes(id);
            foreach (KeyValuePair<string, string> p in attrs)
            {
                obj.SetAttr(p.Key, p.Value);
            }

            List<string> listColl = this._repository.Implement.ListCollections(id);

            foreach (string coll in listColl)
            {
                obj.GetCollection(coll, true);
            }

            _listObjects.Add(obj.Id, obj);
            
            return obj;
        }

        public void SetAttr(string name, string value)
        {
            string val = "";
            if (!_attrs.TryGetValue(name.ToLower(), out val))
            {
                _attrs.Add(name.ToLower(), value);
            }
            else
            {
                _attrs[name.ToLower()] = value;
            }            
        }

        public void DelAttr(string name)
        {
            _attrs.Remove(name);
        }

        public IPObject Navigate(int depth, NAV_DIRECTION direction)
        {
            if (_navigator == null)
                throw new Exception("Попытка выполнить относительную навигацию без полной (Navigate(string))");
            return _navigator.Navigate(depth, direction);
        }

        public IPObject Navigate(string path)
        {
            if (_navigator == null)
            {
                _navigator = new PNavigator(this);
            }
            return _navigator.Navigate(path);
        }
        
        public IPObject GetObjectByDepth(int level)
        {
            return this._navigator.GetObjectByDepth(level);
        }

        public IPObject GetObjectById(int id)
        {
            PObject ret = null;
            if (_listObjects.TryGetValue(id, out ret))
                return ret;
            else
                return null;
        }

        public void SetNavigatorPointer(IPObject obj)
        {
            if (_navigator == null)
                throw new Exception("Навигатор не создан");

            IPObject o = GetObjectById(obj.Id);
            _navigator.Pointer = (PObject)o;
        }

        public IPObject GetNavigatorPointer()
        {
            if (_navigator == null)
                return null;
            else
                return _navigator.Pointer;
        }     

        public string FindAndFormat(string attrName)//, Dictionary<string,string>overloads)
        {
            string ret = "";

            string nameAttrTemplate = "";
            PObject formatObj = null;
            if (this._getAttrWithObject(attrName, true, out nameAttrTemplate, out formatObj))
            {
                if (nameAttrTemplate != "")
                {
                    string template = "";
                    if (formatObj.GetAttr(nameAttrTemplate, true, out template))
                    {
                        if (template != "")
                        {
                            //ret = Templates.Format(template, formatObj, overloads);//, ref dummy, false);
                            ret = formatObj.Templates.Format(template);
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("Не найден объект с атрибутом {0} содержащим имя шаблона", nameAttrTemplate));
                    }
                }
                else
                {
                    ret = Templates.NotFoundMarks.attrs.Begin + attrName + Templates.NotFoundMarks.attrs.End;
                }
            }
            else
            {
                throw new Exception(String.Format("Не найден объект с атрибутом {0} содержащим имя шаблона", attrName));
            }

            return ret;
        }

        private bool _getAttrWithObject(string name, bool findInOwners, out string val, out PObject obj)
        {
            if (_attrs.TryGetValue(name.ToLower(), out val))
            {
                obj = this;
                return true;
            }
            else if (this._ownerCollection != null && findInOwners) // рекурсивно ищем во всех владельцах
            {
                obj = _ownerCollection.OwnerObject;
                return this._ownerCollection.OwnerObject.GetAttr(name, true, out val);
            }
            else
            {
                obj = null;
                return false;
            }
        }

        internal bool FillCollection(PCollection collection)
        {
            List<int> lst = _repository.Implement.ListCollectionObjects(collection.OwnerObject.Id, collection.Name);
            foreach (int id in lst)
            {
                _loadNewObject(id, collection);
            }
            
            return true;
        }

        public string Format(string templateName)
        {
            return this.Templates.TransformText(templateName);
        }

        public void SaveAttr(string name, string value)
        {
            _repository.Implement.SaveAttribute(this.Id, name, value);
        }
    }
}
