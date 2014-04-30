﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace Platform2
{
    internal class PCollection
    {
        internal string Name { set; get; }

        internal PObject OwnerObject;

		private bool _deferredLoad;

        private List<PObject> _items { set; get; }
		
        private bool _loaded;

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
			obj.ownerCollection = this;
            return obj;
        }

        internal PObject GetObject(int index)
        {
            _loadIfNotLoaded();

			int cnt = _items.Count();
            if (index >= 0 && index < cnt)
                return _items[index];
            else
				throw new Exception(string.Format("Запрошен объект с несуществующим индексом ({0}), коллекция {1}", index, this.Name));
        }

        internal int Count()
        {
            _loadIfNotLoaded();

            return _items.Count();
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
   
        internal int IndexOf(PObject obj)
        {
            if (obj != null)
                return _items.IndexOf(obj);
            else
                return -1;
        }
    }
}
