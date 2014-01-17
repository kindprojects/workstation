using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IDBLink
    {
        //List<Dictionary<string, string>> SqlSelect(T db, string sqlQuery, string[] paramList);
        //int SqlIntField(T db, string sqlQuery, string[] paramList, int ifEmpty);

        int GetModelRootID(string modelCode, int ifNotFound);
        List<string> ListCollections(int objectId);
        Dictionary<string, string> ListAttributes(int objectId);
        Dictionary<string, string> ListTemplates(string modelCode);
        List<int> ListCollectionObjects(int objectId, string collName);        
    }
}
