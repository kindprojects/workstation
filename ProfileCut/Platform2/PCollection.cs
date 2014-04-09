using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform2
{
    internal class PCollection
    {        
        internal string Name { set; get; }

        internal PObject OwnerObject;

        private List<PObject> _items { set; get; }
        
        private bool _deferredLoad;
        private bool _loaded;

        internal PCollection(PObject owner, string name, bool deferredLoad)
        {
            _items = new List<PObject>();
            OwnerObject = owner;
            Name = name;
            _deferredLoad = deferredLoad;
            _loaded = false;
        }

        internal PObject InsertObject(PObject obj)
        {
            _items.Add(obj);
            return obj;
        }

        internal PObject GetObject(int index)
        {
            _loadIfNotLoaded();

            if (_items.Count() > 0)
                return _items[index];
            else 
                return null;
        }

        internal int Count()
        {
            _loadIfNotLoaded();

            return _items.Count();
        }

        internal void _loadIfNotLoaded()
        {
            if (_deferredLoad && !_loaded)
            {
                if (this.OwnerObject.FillCollection(this))
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
