using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;

namespace ImportService
{
    class WebHost
    {
        private string _baseAddress = "http://localhost";
        private WebServiceHost _webHost;
        public Uri Point { set; get; }

        public WebHost()
        {
            _webHost = _createHost();
        }

        private WebServiceHost _createHost()
        {
            WebServiceHost host;
            try
            {
                Point = new Uri(String.Format("{0}:{1}", _baseAddress, Program.Config.port));
                host = new WebServiceHost(typeof(UrlParser), Point);
                WebHttpBinding binding = new WebHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IContract), binding, "");
                ServiceDebugBehavior stp = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                stp.HttpHelpPageEnabled = false;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось загрузить службу WebServiceHost. {0}", ex.Message));
            }

            return host;
        }

        internal void Start()
        {
            try
            {
                _webHost.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось открыть загруженую службу WebServiceHost. {0}", ex.Message));
            }
        }

        internal void Done()
        {
            try
            {
                _webHost.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Не удалось закрыть загруженую службу WebServiceHost. {0}", ex.Message));
            }
        }
    }
}
