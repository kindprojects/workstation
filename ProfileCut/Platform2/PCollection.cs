using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform2
{
    internal class PCollection : IPCollection
    {
        internal string Name { set; get; }

        internal PObject OwnerObject;

		private bool _deferredLoad;

        internal List<PObject> _items { set;  get; }
		
        private bool _loaded;

		public IPObject ownerObject { get {return OwnerObject;} }

		public string CollectionName { get { return Name; } }

        internal PCollection(PObject owner, string name, bool deferredLoad)
        {
            _items = new List<PObject>();
            OwnerObject = owner;
            Name = name;
            _loaded = false;
			_deferredLoad = deferredLoad;

			if (!deferredLoad)
			{
				this.FillWithObjects(deferredLoad);
			}
        }

        internal PObject InsertObject(PObject obj)
        {
            _items.Add(obj);
			obj._ownerCollection = this;
            return obj;
        }

        public IPObject GetObject(int index)
        {
            _loadIfNotLoaded();

			int cnt = _items.Count();
            if (index >= 0 && index < cnt)
                return _items[index];
            else
				throw new Exception(string.Format("Запрошен объект с несуществующим индексом ({0}), коллекция {1}", index, this.Name));
        }

		public int IndexOf(IPObject obj){
			int refId = obj.Id;
			int cnt = this._items.Count();
			for(int i = 0; i < cnt; i++)
			{
				if (this._items[i].Id == refId)
					return i;
			}
			return -1;
		}

		public int Count
		{
			get
			{
				_loadIfNotLoaded();

				return _items.Count();
			}
		}

        internal void _loadIfNotLoaded()
        {
            if (!_loaded)
            {
				this.FillWithObjects(this._deferredLoad);
				_loaded = true;
            }
        }

		internal void FillWithObjects(bool deferredLoad)
		{
			if (!_loaded)
			{
				if (deferredLoad && !OwnerObject.storage.ObjectExists(OwnerObject.Id))
					throw new PObjectNotExistsException(OwnerObject.Id, "Объект с указанным ID не существует. Вероятно данные устарели и требуют обновления.");
				List<int> lst = OwnerObject.storage.ListCollectionObjects(OwnerObject.Id, this.Name);
				foreach (int id in lst)
				{
					this.InsertObject(new PObject(OwnerObject.storage, id, OwnerObject.objectsIndex, deferredLoad));
				}
				_loaded = true;
			}
		}

        internal PObject FindObjectByAttrValue(string name, string value)
        {
            _loadIfNotLoaded();

            foreach (PObject item in _items)
            {
                string val = "";
                if (item.GetAttr(name, true, out val))
                {
                    if (val == value)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

		class PObjectNotExistsException : Exception
		{
			public int ObjectID;
			public PObjectNotExistsException(int ObjectId, string Message, Exception innerException)
				: base(Message)
			{
				this.ObjectID = ObjectId;
			}
			public PObjectNotExistsException(int ObjectId, string Message)
				: this(ObjectId, Message, null)
			{

			}
		}

        public void RemoveObject(IPObject obj)
        {
            try
            {
                this._items.Remove((PObject)obj);
            }
            catch (Exception ex) 
            {
                throw new Exception(String.Format("Не удалось получить экземпляр обьекта.Невозможно удалить объект из коллекции {0}. {1}", this.Name, ex.Message));
            }
        }


        public IPObject GetObject(string key)
        {
            return this.FindObjectByAttrValue("_key", key);
        }

        public string GenHtml()
        {
            _loadIfNotLoaded();

            
            string content = "<div><a href=\"../\">...</a></div>";
            foreach (var i in this._items)
            {
                string keyValue = "";
                i.GetAttr("_key", false, out keyValue);
                content += String.Format("<div>id: {0} _key: <a href=\"{1}/\">{1}</a></div>", i.Id, keyValue);
            }

            return content;
        }
    }
}
