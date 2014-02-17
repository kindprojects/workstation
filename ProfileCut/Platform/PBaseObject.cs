using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public class PBaseObject: IPBaseObject
    {
        public int Id { set; get; }
        private Dictionary<string, string> _attrs;
        private List<PCollection> _collects;
        //private RBaseObject _owner;
        private IPDataModel _model;
        private PCollection _ownerCollection;
        private PModelObjectNavigator _navigator;

        public PBaseObject(int id, PCollection ownerCollection, IPDataModel model)
        {
            this.Id = id;
            _attrs = new Dictionary<string, string>();
            _collects = new List<PCollection>();
            _ownerCollection = ownerCollection;
            _model = model;
        }

        public IPBaseObject GetNavigatorPointer()
        {
            return _navigator.Pointer;
        }

        public void SetNavigatorPointer(int id)
        {
            PBaseObject obj = FindObjectById(id);
            _navigator.Pointer = obj;           
        }

        public IPBaseObject NavigatorInitialize(string path)
        {
            _navigator = new PModelObjectNavigator(this);
            return _navigator.Setup(path);            
        }

        //public void NavigatorSetPointer(PBaseObject obj)
        //{
        //    _navigator.Pointer = obj;
        //}

        // int level
        //public IPBaseObject Navigate(PModelObjectNavigatorPathLevel level, NAV_DIRECTION dir)
        //{
        //    return _navigator.Navigate(level, dir);
        //}

        public IPBaseObject Navigate(int depth, NAV_DIRECTION direction)
        {
            return _navigator.Navigate(depth, direction);
        }

        public IPBaseObject Navigate(string path)
        {
            return _navigator.Navigate(path);
        }
        public IPBaseObject GetPointerAtLevel(int level)
        {
            return this._navigator.GetPointerAtLevel(level);
        }

        //public List<PModelObjectNavigatorPathLevel> GetLevels()
        //{
        //    return _navigator.Levels;
        //}

        //public IPBaseObject GetPointer()
        //{
        //    return _navigator._pointer;
        //}

        public List<string> GetPathFromObject(IPBaseObject obj)
        {
            List<string> lst = new List<string>();
            PCollection c = this._ownerCollection;
            while (c != null)
            {
                lst.Insert(0, c.Name);
                if (c.Owner == obj)
                    break;
                c = c.Owner._ownerCollection;
            }
            return lst;
        }

        public string GetTemplateName(IPBaseObject obj, string attrTemplate)
        {
            string ret = "";
            PCollection c = this._ownerCollection;
            string attrVal = "";
            if (obj.GetAttr(attrTemplate, out attrVal) && attrVal != "")
            {
                ret = attrVal;
            }
            else
            {
                while (c != null)
                {
                    if (c.Owner.GetAttr(attrTemplate, out attrVal) && attrVal != "")
                    {
                        ret = attrVal;
                        break;
                    }
                    c = c.Owner._ownerCollection;
                }
            }
            return ret;
        }
        
        public bool GetAttr(string name, out string val)
        {
            if (_attrs.TryGetValue(name.ToLower(), out val))
            {
                return true;
            }
            else if (this._ownerCollection != null)
            {
                return this._ownerCollection.Owner.GetAttr(name, out val);
            }
            else
            {
                return false;
            }
        }

        public PBaseObject FindObjectById(int id)
        {
            return _model.FindObjectById(id, this);
        }

        public void SetAttr(string name, string value)
        {
            string val = "";
            if (!_attrs.TryGetValue(name, out val))
            {
                _attrs.Add(name.ToLower(), value);
            }
            else
            {
                _attrs[name.ToLower()] = value;
            }
        }

        public bool IsChildOf(PBaseObject obj)
        {
            if (obj == null)
                return false;
            PCollection coll = this._ownerCollection;
            while (coll != null)
            {
                if (coll.Owner == obj)
                    return true;
                coll = coll.Owner._ownerCollection;
            }
            return false;
        }

        private PCollection FindCollection(string name)
        {
            name = name.ToLower();

            foreach (PCollection c in _collects)
            {
                if (c.Name.ToLower() == name)
                    return c;
            }
            return null;
        }

        public List<PCollection> GetCollections()
        {
            return this._collects;
        }

        public List<int> GetPathTo(PBaseObject toObject)
        {
            List<int> path = new List<int>();

            if (toObject._ownerCollection == null)
                return null;

            PBaseObject obj = toObject;
            while (obj._ownerCollection.Owner != null)
            {
                path.Insert(0, obj._ownerCollection.IndexOf(obj));

                if (obj._ownerCollection.Owner == this)
                {
                    return path;
                }

                obj = obj._ownerCollection.Owner;
            }
            return null;
        }

        public PCollection GetCollection(string name, bool createIfNotFound)
        {
            PCollection coll;

            coll = this.FindCollection(name);
            if (coll != null)
            {
                return coll;
            }
            else
            {
                if (createIfNotFound)
                {
                    coll = _model.NewCollection(this, name.ToLower());
                    _collects.Add(coll);
                    return coll;
                }
                else
                {
                    return null;
                }
            }
        }

        //public void SetData(Dictionary<string, string> attrs, Dictionary<string, RBaseObject> collect)
        //{
        //    _attrs = attrs;
        //    _collect = collect;
        //}
    }
}
