using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
    public interface IStorage
    {
        int RootObjectId(string modelCode, int ifNotFound);
        
        List<string> ListCollections(int objectId);
        
        Dictionary<string, string> ListAttributes(int objectId);
        
        List<int> ListCollectionObjects(int objectId, string collName);
        		
		bool ObjectExists(int objectId);

        void SetAttribute(int objectId, string name, string value);

        void DeleteObject(int objectId);

        bool Commit();

        void Rollback();
    }
}
