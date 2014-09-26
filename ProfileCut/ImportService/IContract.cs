using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;


namespace ImportService
{    
    [ServiceContract]
    public interface IContract
    {
        [OperationContract]
        //[WebInvoke(UriTemplate = "/objects/{model}/{*path}", Method = "GET")]
        [WebInvoke(UriTemplate = "/objects/{*path}", Method = "GET")]
        XmlElement GetObjectsAsXml(string path);

        [OperationContract]
        [WebInvoke(UriTemplate = "/objects/{model}/{*path}", Method = "POST", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Xml)]        
        void PostObjects(string model, string path, XmlElement xml);
        //HttpResponseMessage PostObjects(string model, string path, XmlElement xml);

        [OperationContract]
        [WebInvoke(UriTemplate = "/objects/{model}/{*path}", Method = "DELETE")]
        void DeleteObjects(string model, string path);

        [OperationContract]
        [WebInvoke(UriTemplate = "/models", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void PostModel(PObjectContainer container);

        [OperationContract]
        [WebGet(UriTemplate = "/view/{model}/{*path}")]
        Stream ViewAsHtml(string model, string path);

        [OperationContract]
        [WebGet(UriTemplate = "/view/")]
        Stream ViewModelsAsHtml(); 
       
        [OperationContract]
        [WebInvoke(UriTemplate = "", Method = "GET")]
        Stream Help();
    }  
}
