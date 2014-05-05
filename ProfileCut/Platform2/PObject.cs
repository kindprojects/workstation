using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Repository;

namespace Platform2
{
    internal class PObject : IPObject   
    {
        public int Id { set; get; }
		
        internal PCollection _ownerCollection;

        internal IStorage storage;

        internal Dictionary<int, IPObject> objectsIndex;

        private Dictionary<string, string> _attrs;

        private Dictionary<string,PCollection> _collections;

		private bool _deferredLoad;

		public IPCollection onwerCollection { get {return _ownerCollection;} }

        internal PObject(IStorage repository, int id, Dictionary<int, IPObject> objectsIndex, bool deferredLoad)
        {
            if (repository == null)
                throw new Exception("Хранилище данных не задано");

            Id = id;
			_ownerCollection = null;

            this.storage = repository;
            this.objectsIndex = objectsIndex;
			this._deferredLoad = deferredLoad;

			// получение значений атрибутов
			_attrs = repository.ListAttributes(Id);

			if (objectsIndex != null)
				objectsIndex.Add(id, this);

			// загрузка коллекций
			_collections = new Dictionary<string,PCollection>();
			
			List<string> collectsNames = storage.ListCollections(Id);
			foreach (string name in collectsNames)
			{
				string lowName = name.ToLower();
				_collections.Add(lowName, new PCollection(this, lowName, deferredLoad));
			}
        }

        public bool GetAttr(string name, bool findInOwners, out string val)
        {
			// ToDo: можно добавить Dictionary<string,PObject>, в котором запоминать объекты, в которых запрошенные атрибуты были найдены ранее (+) быстрее поиск (-) если появится атрибут в объектах по пути, то он будет "обойдён" этим индексом (!) нужно при появлении атрибута очищать его из всех детей, а это медленно
			string lowerName = name.ToLower();
            if (_attrs.TryGetValue(lowerName, out val))
            {
                return true;
            }
            else if (findInOwners) // рекурсивно ищем во всех владельцах
            {
				PCollection coll = this._ownerCollection;
				while (coll != null)
				{
					PObject o = coll.OwnerObject;
					if (o._attrs.TryGetValue(lowerName, out val))
						return true;
					coll = o._ownerCollection;
				}
            }
			return false;
        }

        public bool IsChildOf(int objectId)
        {
            if (objectId <= 0)
                return false;
            PCollection coll = this._ownerCollection;
            while (coll != null)
            {
                if (coll.OwnerObject.Id == objectId)
                    return true;
                coll = coll.OwnerObject._ownerCollection;
            }
            return false;
        }

        internal PCollection GetCollection(string name, bool createIfNotFound)
        {
			name = name.ToLower();
            PCollection coll;

            if (this._collections.TryGetValue(name, out coll)){
                return coll;
            }else if (createIfNotFound){
				coll = new PCollection(this, name, this._deferredLoad);
				_collections.Add(name, coll);
                return coll;
            }else{
				throw new Exception(string.Format("Коллекция {0} не найдена!", name));
			}
        }
		public IPCollection GetCollection(string name)
		{
			return this.GetCollection(name, false);
		}
		
		public void SetAttr(string name, string value)
        {
            string val = "";
			string lowerName = name.ToLower();
            if (!_attrs.TryGetValue(lowerName, out val))
            {
                _attrs.Add(lowerName, value);
            }
            else
            {
                _attrs[lowerName] = value;
            }            
        }

        public bool FindAttr(string attrName, out IPObject obj, out string val)
		{
			PObject o = this;
			obj = null;
			while (!o.GetAttr(attrName, false, out val))
			{
				PCollection coll = o._ownerCollection;
				if (coll == null)
					return false;
				o = coll.OwnerObject;
			}
			obj = o;
			return true;
            /* ДАЛЕЕ НЕПРАВИЛЬНЫЙ КОД:
			 * ПРАВИЛЬНЫЙ ВЫШЕ
			if (_attrs.TryGetValue(name.ToLower(), out val))
            {
                obj = this;
                return true;
            }
            else if (this._ownerCollection != null && findInOwners) // рекурсивно ищем во всех владельцах
            {
                obj = _ownerCollection.OwnerObject;
                return this._ownerCollection.OwnerObject.GetAttr(name, true, out val); // НЕПРАВИЛЬНО - этот метод надёт значение, но не скажет где
            }
            else
            {
                obj = null;
                return false;
            }*/
        }

        public void StorageUpdateAttr(string name)
        {
            string value = "";
            if (GetAttr(name, false, out value))
            {
				this.storage.SetAttribute(this.Id, name, value);
            }
            else
            {
                throw new Exception(String.Format("Атрибут {0} у объекта {1} не найден", name, this.Id.ToString()));
            }
        }

		public XElement ToXElement()
		{
			return ObjToXElement(this);
		}
		public static XElement ObjToXElement(PObject obj)
		{
			XElement x = new XElement("obj");
			
			XElement xAttrs = new XElement("attrs");
			foreach (string attr in obj._attrs.Keys)
			{
				xAttrs.SetElementValue(attr, obj._attrs[attr]);
			}
			x.Add(xAttrs);

			XElement xColls = new XElement("colls");
			x.Add(xColls);
			foreach (PCollection coll in obj._collections.Values)
			{
				XElement xColl = new XElement("coll");
				xColl.SetAttributeValue("name", coll.Name);
				xColls.Add(xColl);
				int cnt = coll.Count;
				for (int i = 0; i < cnt; i++)
				{
					XElement xi = ObjToXElement(coll._items[i]);
					xi.SetAttributeValue("num", i);
					xColl.Add(xi);
				}
			}
			return x;
		}
    }
}
