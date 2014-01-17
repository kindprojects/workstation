using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class RCollection
    {
        private List<RBaseObject> _items { set; get; }
        public RBaseObject Owner;
        public string Name;
        private bool _deferredLoad;
        private bool _loaded;
        private IDataModel _model;

        public RCollection(RBaseObject owner, string name, bool deferredLoad, IDataModel model)
        {
            _items = new List<RBaseObject>();
            Owner = owner;
            Name = name;
            _deferredLoad = deferredLoad;
            _loaded = false;
            _model = model;
        }

        public RBaseObject InsertObject(RBaseObject obj)
        {
            _items.Add(obj);
            return obj;
        }

        public RBaseObject GetObject(int index)
        {
            _loadIfNotLoaded();

            return _items[index];
        }

        public int Count()
        {
            _loadIfNotLoaded();

            return _items.Count();
        }

        private void _loadIfNotLoaded()
        {
            if (_deferredLoad && !_loaded)
            {
                if (this._model.FillCollection(this))
                    _loaded = true;
            }
        }

        public RBaseObject FindObjectByAttrValue(string name, string value)
        {
            _loadIfNotLoaded();

            foreach (RBaseObject item in _items)
            {
                string val = "";
                if (item.GetAttr(name, out val))
                {
                    if (val == value)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public RBaseObject PrevSibling(RBaseObject obj)
        {
            int i = this._items.IndexOf(obj) - 1;
            return (i < 0) ? null : this._items[i];
        }

        public RBaseObject NextSibling(RBaseObject obj)
        {
            int i = this._items.IndexOf(obj) + 1;
            return (i >= this._items.Count()) ? null : this._items[i];
        }

        public int IndexOf(RBaseObject obj)
        {
            if (obj != null)
                return _items.IndexOf(obj);
            else
                return -1;
        }
    }

}
