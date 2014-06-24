using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Repository;

namespace Platform2
{
    public class PModel
    {
        public IPObject Root { set; get; }

		public Dictionary<int, IPObject> objectsIndex {protected set; get; }

        protected IStorage _storage;

        public PModel(IStorage rep, string model, bool deferredLoad)
        {
			this.objectsIndex = new Dictionary<int,IPObject>();
			int objectId = rep.RootObjectId(model, -1);
			if (objectId == -1)
				throw new Exception(string.Format("Модель с кодом \"{0}\" не найдена", model));
			Root = new PObject(rep, objectId, objectsIndex, deferredLoad);

            _storage = rep;
        }

        public void DeleteObject(IPObject obj)
        {
            if (Root.Id != obj.Id)
            {                
                obj.onwerCollection.RemoveObject(obj);

                _storage.DeleteObject(obj.Id);
            }
        }    
    }
}
