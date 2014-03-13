using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Platform;
using System.Windows.Forms;

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

        public string FindAndFormat(string attrName) //, Dictionary<string, string> overloads)
        {
            return platformObject.FindAndFormat(attrName);//, overloads);
        }

        public ABaseObject GetObjectById(int id)
        {
            IPBaseObject obj = platformObject.FindObjectById(id);
            return _createApiObject(obj);
        }

        public string GetAttr(string attrName, bool findInOwners)
        {
            string val = "";
            this.platformObject.GetAttr(attrName, findInOwners, out val);

            return val;
        }
        
        ABaseObject _createApiObject(IPBaseObject obj)
        {
            if (obj != null)
                return new ABaseObject()
                {
                    Id = obj.Id,
                    platformObject = obj
                };
            else
                return null;
        }

        //public string Transform(string template, ABaseObject obj, Dictionary<string, string> overloads)
        //{
        //    string dummy = "";
        //    //return _model.Templates.TransformText(template, obj.platformObject, overloads, ref dummy, false);
        //    return _model.Templates.TransformText(template, obj.platformObject, overloads, ref dummy, false);
        //    //return this.platformObject.FindAndFormat.Templates.TransformText(template, obj.platformObject, overloads, ref dummy, false);
        //}
    }
    
    public class AModel
    {        
        private IPDBLink _db;
        private PModel _model;

        public string PrinterName;

        public AModel(IAHost host, string connectionString, string modelCode, bool defferedLoad)
        {            
            _db = new AFbLink(connectionString);

            AHost ahost = new AHost(host);

            _model = new PPlatform(_db, modelCode, defferedLoad, ahost).Model;

            //_model.HostRequest += new EventHandler<HostQueryEventArgs>(ProcessHostRequest);            
        }

        public ABaseObject GetRoot()
        {
            var obj = new ABaseObject();            
            obj.Id = _model.Data.Id; 
            obj.platformObject = _model.Data;

            return obj;
        }

        public string Transform(string template, ABaseObject obj)//, Dictionary<string, string> overloads)
        {
            string dummy = "";

            if (obj != null)
                return _model.Templates.TransformText(template, obj.platformObject);//, overloads, ref dummy, false);
            else
                return "";
        }
        
        //public void ProcessHostRequest(object sender, HostQueryEventArgs e)
        //{
        //    if (e.Text.ToLower() == "printername")
        //    {
        //        (sender as PModel).RiseHostResponse(this.PrinterName);
        //    }
        //}
    }   
}
