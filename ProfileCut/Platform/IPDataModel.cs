using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public interface IPDataModel
    {
        // интерфейс модели для её объектов
        bool FillCollection(PCollection coll);
        PCollection NewCollection(PBaseObject owner, string name);
        PBaseObject FindObjectById(int id, PBaseObject where);
        //void RiseHostRequest(string textRequest);
        //void RiseHostResponse(string textResponse);

        //event EventHandler<HostQueryEventArgs> HostRersponse;

        PPlatform Platform { set; get; }
    }
}
