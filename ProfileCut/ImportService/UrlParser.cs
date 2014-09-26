using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Platform2;
using Storage;
using System.Xml;
using System.Xml.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using System.Web;
using System.Net.Http;
using ImportService.Repository;
using ImportService.Repository.Fb;
using System.Threading;
using System.IO;
using FirebirdSql.Data.FirebirdClient;


namespace ImportService
{
    enum TargetPathType
    {
        Object,
        Collection
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession
        , ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class UrlParser : IContract
    {
        public XmlElement GetObjectsAsXml(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><error>Не реализовано</error>");

            return doc.DocumentElement;
        }

        public void PostObjects(string model, string path, XmlElement xml)
        {
            //HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                TargetPath target = new TargetPath(model, path);                
                PObjectContainer obj = _parseXML<PObjectContainer>(xml);
                PObjectContainer container = _parseXML<PObjectContainer>(xml);

                using (FbConnection conn = new FbConnection(Program.Config.connectionString))
                {
                    conn.Open();
                    FbTransaction trans = conn.BeginTransaction();

                    if (target.Type == TargetPathType.Object)
                        throw new Exception("Ошибка в ImportService. Для добовления объектов не реализовано");

                    SaveToDb(conn, trans, target.PlatformCollection.ownerObject.Id, target.PlatformCollection.CollectionName, container.Objects);
                    trans.Commit();
                }
            }
            catch(Exception ex)
            {
                Program.Logger.Error(ex.Message);
                _throwNewWebExeption(model, path, String.Format("Ошибка в ImportService. {0}", ex.Message));
            }
        }

        public void PostModel(PObjectContainer container)
        {
            try
            {
                TargetPath target = new TargetPath(container.ModelCode, "");

                using (FbConnection conn = new FbConnection(Program.Config.connectionString))
                {
                    conn.Open();
                    FbTransaction trans = conn.BeginTransaction();
                    foreach (PObject obj in container.Objects)
                    {
                        foreach (PCollection collection in obj.Collections)
                        {
                            SaveToDb(conn, trans, target.PlatfromObject.Id, collection.Name, collection.Objects);
                        }
                    }

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex.Message);
                _throwNewWebExeption(container.ModelCode, "", String.Format("Ошибка в ImportService. {0}", ex.Message));
            }            

        }

        public void DeleteObjects(string model, string path)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                TargetPath target = new TargetPath(model, path);                
              
                using (FbConnection conn = new FbConnection(Program.Config.connectionString))
                {
                    conn.Open();
                    FbTransaction trans = conn.BeginTransaction();

                    if (target.Type == TargetPathType.Object)
                        new RepObject().Delete(conn, trans, target.PlatfromObject.Id);
                    else
                        new RepCollection().Delete(conn, trans, target.PlatformCollection.ownerObject.Id, target.PlatformCollection.CollectionName);

                    trans.Commit();
                }
            }
            catch(Exception ex)
            {
                Program.Logger.Error(ex.Message);
                _throwNewWebExeption(model, path, String.Format("Ошибка в ImportService. {0}", ex.Message));                
            }            
        }

        public Stream ViewAsHtml(string model, string path)
        {
            try
            {
                TargetPath target = new TargetPath(model, path);

                string content = "";
                if (target.Type == TargetPathType.Collection)
                    content = target.PlatformCollection.GenHtml();
                else
                    content = target.PlatfromObject.GenHtml();
                
                return _getResponseStream(_getFullHtml(content));
            }
            catch (Exception ex)
            {
                return _getResponseStream(_getFullHtml(ex.Message));
            }   
        }

        public Stream ViewModelsAsHtml()
        {
            List<string> models = new SStorageFB(Program.Config.connectionString).GetModelCodes();

            string content = "<h3>View</h3><div><a href=\"../\">&larr;&nbsp;Назад</a></div><div><b>Модели:</b></div>";
            foreach(string model in models)
                content += String.Format("<div><a href=\"{0}/\">{0}<a></div>", model);

            return _getResponseStream(_getFullHtml(content));
        }

        private Stream _getResponseStream(string html)
        {
            byte[] resultBytes = Encoding.UTF8.GetBytes(html);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            return new MemoryStream(resultBytes);             
        }

        //private string _getOptimizationHtml(string optimizationName, string path)
        //{
        //    return String.Format("<a href=\"{0}/{1}\">{2}<a>", Program.Host.Point.AbsoluteUri, path, optimizationName);
        //}

        private string _getFullHtml(string content)
        {
            return String.Format("<!DOCTYPE html><html><head><meta charset=\"utf-8\"/></head><body>{0}</body></html>", content);
        }

        private static XmlElement _getXmlElement(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return doc.DocumentElement;
        }

        private void _throwNewWebExeption(string model, string path, string message)
        {
            throw new WebFaultException<string>(String.Format("Не удалось выполнить {0} запрос к модели {1} по пути {2}. {3}"
                , WebOperationContext.Current.IncomingRequest.Method, model, path, message), HttpStatusCode.BadRequest);
        }

        private void _createErrorResponse(string model, string path, string message)
        {
            OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
            response.StatusCode = HttpStatusCode.Unauthorized;
            response.StatusDescription = String.Format("Не удалось выполнить {0} запрос к модели {1} по пути {2}. {3}"
                , WebOperationContext.Current.IncomingRequest.Method, model, path, message);
        }

        private T _parseXML<T>(XmlElement xml)
        {
            T container;
            try
            {
                container = (T)new XmlSerializer(typeof(PObjectContainer)).Deserialize(new XmlNodeReader(xml));
                if (container == null)
                {
                    throw new Exception(String.Format("Не удалось распарсить XML в объект типа {0}. Пустой XML", typeof(T).Name));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось распарсить XML в объект типа {0}. {1}", typeof(T).Name, ex.Message));
            }

            return container;
        }

        private void _deleteOptimization(FbConnection conn, FbTransaction trans, int ownerObjectId, string grorderId)
        {
            RepObject rep = new RepObject();
            List<int> ids = rep.Find(conn, trans, ownerObjectId, "optimizations", "grorderid", grorderId);
            foreach (int id in ids)
                rep.Delete(conn, trans, id);
        }


        private bool SaveToDb(FbConnection conn, FbTransaction trans, int rootObjectId, string collectionName, List<PObject> objects)
        {
            int? collectionId = new RepCollection().Get(conn, trans, rootObjectId, collectionName, true);
            if (collectionId == null)
                throw new Exception(String.Format("Коллекция {0} не найдена и не создана", collectionName));

            foreach (PObject o in objects)
            {
                //int? collectionId = new RepCollection().Get(conn, trans, rootObjectId, collectionName, true);
                //if (collectionId == null)
                //    throw new Exception(String.Format("Коллекция {0} не найдена и не создана", collectionName));

                if (collectionName.ToLower().Trim() == "optimizations")
                {
                    string grorderId = o.Attributes.FirstOrDefault(f => f.Name.ToLower().Trim() == "grorderid").Value;

                    if (!String.IsNullOrWhiteSpace(grorderId))
                        _deleteOptimization(conn, trans, rootObjectId, grorderId);
                }

                int? newObjectId = new RepObject().Add(conn, trans, (int)collectionId);
                if (newObjectId == null)
                    throw new Exception(String.Format("Объект в коллекции {0} не создан", collectionName));

                foreach (PAttribute a in o.Attributes)
                    new RepAttribute().Set(conn, trans, (int)newObjectId, a.Name, a.Value);

                foreach (PCollection c in o.Collections)
                {
                    // рекурсия
                    SaveToDb(conn, trans, (int)newObjectId, c.Name, c.Objects);
                }
            }

            return true;
        }

        private IPObject _getObjectByKey(IPCollection collection, PObject o)
        {
            PAttribute attr = o.Attributes.Where(f => f.Name.ToLower() == "_key").First();
            if (attr != null)
                return collection.GetObject(attr.Value);
            else
                return null;            
        }

        public Stream Help()
        {
            string content = "<h3>ImportService Api</h3><div><b>GET:</b></div><div><a href=\"View/\">View<a><div><div><a href=\"Objects/\">Objects<a><div></br>"
                + "<div><b>POST:</b></div><div>Objects</div></br>"
                + "<div><b>DELETE:</b></div><div>Objects</div>"; 
                
            return _getResponseStream(_getFullHtml(content));
        }
    }

    internal class TargetPath
    {
        public TargetPathType Type { private set; get; }

        public IPCollection PlatformCollection { set; get; }

        public IPObject PlatfromObject { set; get; }
        
        public TargetPath(string model, string path)
        {
            string[] pathes = path.Split('/');

            //if (pathes == null || pathes.Length == 0)
            //    throw new Exception(String.Format("Неверный путь {0} для модели {1}", path, model));
            
            PModel platformModel;
            try
            {
                platformModel = new PModel(new SStorageFB(Program.Config.connectionString), model, true);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex.Message);

                throw new Exception(String.Format("Ошибка в ImportService. Не удалось загрузить модель. {0}", ex.Message));
            }
           
            this.Type = pathes.Count().IsEven() ? TargetPathType.Object : TargetPathType.Collection;
            PlatfromObject = platformModel.Root;
            for (int ii = 0; ii < pathes.Length; ii++)
            {
                // root
                if (String.IsNullOrEmpty(pathes[0]))
                {
                    this.Type = TargetPathType.Object;
                    break;
                }

                if (ii.IsEven()) // коллекция               
                {
                    if (PlatfromObject == null)
                        throw new Exception(String.Format("Не задант объект модели {0}, в котором надо искать коллекцию {1}"
                            , model, pathes[ii]));

                    PlatformCollection = PlatfromObject.GetCollection(pathes[ii], true);
                    if (PlatformCollection == null)
                        throw new Exception(String.Format("Коллекция {0} не найдена ине создана в модели {1}"
                            , pathes[ii], model));
                }
                else // объект
                {
                    if (PlatformCollection == null)
                        throw new Exception(String.Format("Не задана коллекция модели {0}, в которой надо искать объект с ключом {1}"
                            , model, path[ii]));

                    PlatfromObject = PlatformCollection.GetObject(pathes[ii]);
                    if (PlatfromObject == null)
                        throw new Exception(String.Format("Объект с ключом {0} не найден в коллекции {1} модели {2}"
                            , pathes[ii], PlatformCollection.CollectionName, model));
                }
            }
            
            // если this представляет собой коллекцию, целевого объекта нет
            if (Type == TargetPathType.Collection)
                PlatfromObject = null;
        }            
    }
}
