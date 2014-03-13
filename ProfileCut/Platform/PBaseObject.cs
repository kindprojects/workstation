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
        public IPDataModel Model { set; get; }
        private PCollection _ownerCollection;
        private PModelObjectNavigator _navigator;
        public PTemplates Templates;

        public PBaseObject(int id, PCollection ownerCollection, IPDataModel model)
        {
            this.Id = id;
            _attrs = new Dictionary<string, string>();
            _collects = new List<PCollection>();
            _ownerCollection = ownerCollection;
            Model = model;
            Templates = new PTemplates(this);
        }

        public string FindAndFormat(string attrName)//, Dictionary<string,string>overloads)
        {
            string ret = "";

            string nameAttrTemplate = "";
            PBaseObject formatObj = null;
            if (this.GetAttrWithObject(attrName, true, out nameAttrTemplate, out formatObj))
            {
                if (nameAttrTemplate != "")
                {
                    string template = "";
                    if (formatObj.GetAttr(nameAttrTemplate, true, out template))
                    {
                        if (template != "")
                        {
                            //ret = Templates.Format(template, formatObj, overloads);//, ref dummy, false);
                            ret = Templates.Format(template, formatObj);
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("Не найден объект с атрибутом {0} содержащим имя шаблона", nameAttrTemplate));
                    }
                }
                else
                {
                    ret = Templates.NotFoundMarks.attrs.Begin + attrName + Templates.NotFoundMarks.attrs.End;
                }
            }
            else
            {                
                throw new Exception(String.Format("Не найден объект с атрибутом {0} содержащим имя шаблона", attrName));
            }

            return ret;
        }

        //public string Transform(string template, ABaseObject obj, Dictionary<string,string>overloads)
        //{
        //    string dummy = "";
        //    return this. _model.Templates.TransformText(template, obj.platformObject, overloads, ref dummy, false);
        //}

        public IPBaseObject GetNavigatorPointer()
        {
            if (_navigator == null)
                return null;
            else
                return _navigator.Pointer;
        }

        public void SetNavigatorPointer(int id)
        {
            PBaseObject obj = FindObjectById(id);
            if (_navigator == null)
                throw new Exception("Навигатор не создан");
            _navigator.Pointer = obj;
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
            if (_navigator == null)
                throw new Exception("Попытка выполнить относительную навигацию без полной (Navigate(string))");
            return _navigator.Navigate(depth, direction);
        }

        public IPBaseObject Navigate(string path)
        {
            if (_navigator == null)
            {
                _navigator = new PModelObjectNavigator(this);
            }
            return _navigator.Navigate(path);
        }
        public IPBaseObject GetObjectByDepth(int level)
        {
            return this._navigator.GetObjectByDepth(level);
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
            if (obj.GetAttr(attrTemplate, true, out attrVal) && attrVal != "")
            {
                ret = attrVal;
            }
            else
            {
                while (c != null)
                {
                    if (c.Owner.GetAttr(attrTemplate, true, out attrVal) && attrVal != "")
                    {
                        ret = attrVal;
                        break;
                    }
                    c = c.Owner._ownerCollection;
                }
            }
            return ret;
        }
        
        public bool GetAttr(string name, bool findInOwners, out string val)
        {
            if (_attrs.TryGetValue(name.ToLower(), out val))
            {
                return true;
            }
            else if (this._ownerCollection != null && findInOwners) // рекурсивно ищем во всех владельцах
            {
                return this._ownerCollection.Owner.GetAttr(name, true, out val);
            }
            else
            {
                return false;
            }
        }

        public bool GetAttrWithObject(string name, bool findInOwners, out string val, out PBaseObject obj)
        {
            if (_attrs.TryGetValue(name.ToLower(), out val))
            {
                obj = this;
                return true;
            }
            else if (this._ownerCollection != null && findInOwners) // рекурсивно ищем во всех владельцах
            {
                obj = this._ownerCollection.Owner;
                return this._ownerCollection.Owner.GetAttr(name, true, out val);
            }
            else
            {
                obj = null;
                return false;
            }
        }
        
        public PBaseObject FindObjectById(int id)
        {
            return Model.FindObjectById(id, this);
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

        //public List<int> GetPathTo(PBaseObject toObject)
        //{
        //    List<int> path = new List<int>();

        //    if (toObject._ownerCollection == null)
        //        return null;

        //    PBaseObject obj = toObject;
        //    while (obj._ownerCollection.Owner != null)
        //    {
        //        path.Insert(0, obj._ownerCollection.IndexOf(obj));

        //        if (obj._ownerCollection.Owner == this)
        //        {
        //            return path;
        //        }

        //        obj = obj._ownerCollection.Owner;
        //    }
        //    return null;
        //}
        public PNavigatorPath GetPathTo(PBaseObject toObject)
        {
            PNavigatorPath path = new PNavigatorPath();

            if (toObject._ownerCollection == null)
                return null;

            PBaseObject obj = toObject;
            while (obj._ownerCollection.Owner != null)
            {
                path.Parts.Insert(0, new PNavigatorPartPath(obj._ownerCollection.Name, obj._ownerCollection.IndexOf(obj)));
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
                    coll = Model.NewCollection(this, name.ToLower());
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
