using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

// %([^%]+)%
// \[(\S+:\S+)\]

namespace Model
{
    public class RBaseObject
    {
        public int Id;
        private Dictionary<string, string> _attrs;
        private List<RCollection> _collects;
        //private RBaseObject _owner;
        private IDataModel _model;
        private RCollection _ownerCollection;
        
        public RBaseObject(int id, RCollection ownerCollection, IDataModel model)
        {
            this.Id = id;
            _attrs = new Dictionary<string, string>();
            _collects = new List<RCollection>();
            _ownerCollection = ownerCollection;
            _model = model;
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
            
            //if (!_attrs.TryGetValue(name.ToLower(), out ret))
            //{
            //    ret = notf + name + ">";
            //}
            
        }

        public RBaseObject FindObjectById(int id)
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

        public bool IsChildOf(RBaseObject obj)
        {
            if (obj == null)
                return false;
            RCollection coll = this._ownerCollection;
            while (coll != null)
            {
                if (coll.Owner == obj)
                    return true;
                coll = coll.Owner._ownerCollection;
            }
            return false;
        }

        //public void AddAtr(string name, string value)
        //{
        //    //_attrs.Add(name, value);
        //}

        //public RBaseObject GetObject(string name)
        //{
        //    RBaseObject ret = null;

        //    //_collect.TryGetValue(name.ToLower(), out ret);
            
        //    return ret;
        //}
        
        
        //public void AddCollection(string name, RCollections collects)
        //{
        //    _collect.Add(name, collects);
        //}

        private RCollection FindCollection(string name){
            name = name.ToLower();

            foreach(RCollection c in _collects)
            {
                if (c.Name.ToLower() == name)
                    return c;
            }
            return null;
        }

        public List<RCollection> GetCollections()
        {
            return this._collects;
        }

        public List<RObjectLevelPath> GetPathTo(RBaseObject toObject)
        {
            List<RObjectLevelPath> path = new List<RObjectLevelPath>();

            if (toObject._ownerCollection == null)
                return null;

            RBaseObject obj = toObject;
            while (obj._ownerCollection.Owner != null)
            {
                path.Insert(0, new RObjectLevelPath()
                {
                    CollectionName = obj._ownerCollection.Name,
                    Index = obj._ownerCollection.IndexOf(obj)
                });

                if (obj._ownerCollection.Owner == this)
                {
                    return path;
                }

                obj = obj._ownerCollection.Owner;
            }
            return null;
        }

        public RCollection GetCollection(string name, bool createIfNotFound)
        {
            RCollection coll;

            coll = this.FindCollection(name);
            if (coll != null){
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
