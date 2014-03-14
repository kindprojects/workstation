using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public interface IPDBLink
    {
        int GetModelRootID(string modelCode, int ifNotFound);
        List<string> ListCollections(int objectId);
        Dictionary<string, string> ListAttributes(int objectId);
        List<int> ListCollectionObjects(int objectId, string collName);
    }
}
