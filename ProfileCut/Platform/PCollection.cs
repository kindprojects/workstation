using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public class PCollection
    {        
        private List<PBaseObject> _items { set; get; }
        public PBaseObject Owner;
        public string Name;
        private bool _deferredLoad;
        private bool _loaded;
        private IPDataModel _model;

        public PCollection(PBaseObject owner, string name, bool deferredLoad, IPDataModel model)
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

            if (_items.Count() > 0)
                return _items[index];
            else 
                return null;
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

        public int IndexOf(PBaseObject obj)
        {
            if (obj != null)
                return _items.IndexOf(obj);
            else
                return -1;
        }
    }
}
