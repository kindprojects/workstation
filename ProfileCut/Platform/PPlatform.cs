﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Platform
{
    public class PPlatform
    {
        public IPHost Host { set; get; }

        public PModel Model { set; get; }
        public PPlatform(IPDBLink db, string modelCode, bool defferedLoad, IPHost host)
        {
            Host = host;
            Model = new PModel(db, modelCode, defferedLoad, this);
        }

        //public List<IPBaseObject> GetCollectionObjects(string masterCollection)
        //{
        //    List<IPBaseObject> list = new List<IPBaseObject>();

        //    PCollection collection = Model.Data.GetCollection(masterCollection, false);
        //    for (int ii = 0; ii < collection.Count(); ii ++ )
        //    {
        //        list.Add(collection.GetObject(ii));
        //    }

        //    return list;
        //}        

        //public string Transform(string template, IPBaseObject obj, Dictionary<string, string>overloads)
        //{
        //    string path = "";
        //    return _model.Templates.TransformText(template, (PBaseObject)obj, overloads, ref path, false);            
        //}
    }
}
