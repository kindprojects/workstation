using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Platform;

namespace Api
{
    public class ABaseObject
    {        
        public int Id {set;get;}

        internal IPBaseObject platformObject;

        public ABaseObject Navigate(string path)
        {
            IPBaseObject obj = platformObject.Navigate(path);
            return _createApiObject(obj);
        }
        public ABaseObject GetObjectByDepth(int level)
        {
            return _createApiObject(this.platformObject.GetObjectByDepth(level));
        }

        public ABaseObject Navigate(int depth, int direction)
        {
            NAV_DIRECTION navDirection = direction == 0 ? NAV_DIRECTION.DOWN: NAV_DIRECTION.UP;
            IPBaseObject obj = platformObject.Navigate(depth, navDirection);
            return _createApiObject(obj);
        }

        public ABaseObject GetNavigatorPointer()
        {
            IPBaseObject obj = platformObject.GetNavigatorPointer();
            if (obj == null)
                return null;
            else
                return _createApiObject(obj);
        }

        public void SetNavigatorPointer(ABaseObject obj)
        {
            platformObject.SetNavigatorPointer(obj.Id);
        }

        //public string GetPathFromObject(ABaseObject obj, string separator)
        //{
        //    List<string> path = this.platformObject.GetPathFromObject(obj.platformObject);
        //    string ret = "";
        //    foreach (string s in path)
        //    {
        //        ret += ((ret == "") ? "" : separator) + s;
        //    }
        //    return ret;
        //}

        public ABaseObject GetObjectById(int id)
        {
            IPBaseObject obj = platformObject.FindObjectById(id);
            return _createApiObject(obj);
        }

        //public string GetTemplateName(string attrTemplateName)
        //{
        //    IPBaseObject obj = platformObject.FindObjectById(this.Id);
        //    return obj.GetTemplateName(obj, attrTemplateName);
        //}

        public string GetAttr(string attrName, bool findInOwners)
        {
            string val = "";
            this.platformObject.GetAttr(attrName, findInOwners, out val);

            return val;
        }
        
        ABaseObject _createApiObject(IPBaseObject obj)
        {
            return new ABaseObject()
            {
                Id = obj.Id,
                platformObject = obj
            };
        }
    }

    //public class ACollection
    //{
    //    List<ABaseObject> GetObjects()
    //    {
    //        List<ABaseObject> list = new List<ABaseObject>();

    //        return list;
    //    }
    //    ABaseObject GetObjectByIndex(int index) 
    //    {
    //        return null;
    //    }
    //}
    
    public class AModel
    {        
        private IPDBLink _db;
        private PModel _model;        

        public AModel(string connectionString, string modelCode, bool defferedLoad)
        {
            _db = new AFbLink(connectionString);
            _model = new PModel(_db, modelCode, defferedLoad);
        }

        public ABaseObject GetRoot()
        {
            var obj = new ABaseObject();            
            obj.Id = _model.Data.Id; 
            obj.platformObject = _model.Data;

            return obj;
        }

        public string Transform(string template, ABaseObject obj)
        {            
            string dummy = "";
            return _model.Templates.TransformText(template, obj.platformObject, ref dummy, false);
        }
    }   
}
