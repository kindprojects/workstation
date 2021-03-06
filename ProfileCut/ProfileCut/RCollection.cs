﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Platform;

namespace Model
{
    public class RCollection
    {
        private List<PBaseObject> _items { set; get; }
        public PBaseObject Owner;
        public string Name;
        private bool _deferredLoad;
        private bool _loaded;
        private IDataModel _model;

        public RCollection(PBaseObject owner, string name, bool deferredLoad, IDataModel model)
        {
            _items = new List<PBaseObject>();
            Owner = owner;
            Name = name;
            _deferredLoad = deferredLoad;
            _loaded = false;
            _model = model;
        }

        public PBaseObject InsertObject(PBaseObject obj)
        {
            _items.Add(obj);
            return obj;
        }

        public PBaseObject GetObject(int index)
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

        public PBaseObject FindObjectByAttrValue(string name, string value)
        {
            _loadIfNotLoaded();

            foreach (PBaseObject item in _items)
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

        public PBaseObject PrevSibling(PBaseObject obj)
        {
            int i = this._items.IndexOf(obj) - 1;
            return (i < 0) ? null : this._items[i];
        }

        public PBaseObject NextSibling(PBaseObject obj)
        {
            int i = this._items.IndexOf(obj) + 1;
            return (i >= this._items.Count()) ? null : this._items[i];
        }

        public int IndexOf(PBaseObject obj)
        {
            if (obj != null)
                return _items.IndexOf(obj);
            else
                return -1;
        }
    }
}
