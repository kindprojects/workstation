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
		
        internal PCollection ownerCollection;

        internal IStorage storage;

        internal Dictionary<int, IPObject> objectsIndex;

        private Dictionary<string, string> _attrs;

        private Dictionary<string,PCollection> _collections;

		private bool _deferredLoad;
		
        private PNavigator _navigator;
		
        internal PObject(IStorage repository, int id, Dictionary<int, IPObject> objectsIndex, bool deferredLoad)
        {
            if (repository == null)
                throw new Exception("Хранилище данных не задано");

            Id = id;
			ownerCollection = null;

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
				_collections.Add(name, new PCollection(this, name, deferredLoad));
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
				PCollection coll = this.ownerCollection;
				while (coll != null)
				{
					PObject o = coll.OwnerObject;
					if (o._attrs.TryGetValue(lowerName, out val))
						return true;
					coll = o.ownerCollection;
				}
            }
			return false;
        }

        public bool IsChildOf(int objectId)
        {
            if (objectId <= 0)
                return false;
            PCollection coll = this.ownerCollection;
            while (coll != null)
            {
                if (coll.OwnerObject.Id == objectId)
                    return true;
                coll = coll.OwnerObject.ownerCollection;
            }
            return false;
        }

		// ToDo: выпилить в навигатор (?)
        internal PNavigatorPath GetPathTo(PObject toObject)
        {
            PNavigatorPath path = new PNavigatorPath();

            if (toObject.ownerCollection == null)
                return null;

            PObject obj = toObject;
            while (obj.ownerCollection.OwnerObject != null)
            {
                path.Parts.Insert(0, new PNavigatorPart(obj.ownerCollection.Name, obj.ownerCollection.IndexOf(obj)));
                if (obj.ownerCollection.OwnerObject == this)
                {
                    return path;
                }

                obj = obj.ownerCollection.OwnerObject;
            }
            
            return null;
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
		public PCollection GetCollection(string name)
		{
			return this.GetCollection(name, false);
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

        public bool FindAttr(string attrName, out IPObject obj, out string val)
		{
			PObject o = this;
			obj = null;
			while (!o.GetAttr(attrName, false, out val))
			{
				PCollection coll = o.ownerCollection;
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
    }
}
