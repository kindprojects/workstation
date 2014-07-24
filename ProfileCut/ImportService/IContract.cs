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
        [WebInvoke(UriTemplate = "/data/{model}/{*path}", Method = "GET")]
        XmlElement GetObjectsAsXml(string model, string path);

        [OperationContract]
        [WebInvoke(UriTemplate = "/data/{model}/{*path}", Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void PostObjects(string model, string path, XmlElement xml);

        [OperationContract]
        [WebInvoke(UriTemplate = "/data/{model}/{*path}", Method = "DELETE")]
        void DeleteObjects(string model, string path);

        [OperationContract]
        [WebGet(UriTemplate = "/view/{model}/{*path}")]
        Stream ViewAsHtml(string model, string path);        
    }  
}
