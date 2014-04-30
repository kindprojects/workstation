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

		public Dictionary<int,IPObject> objectsIndex;

        public PModel(IStorage rep, string model, bool deferredLoad)
        {
			int objectId = rep.RootObjectId(model, -1);
			if (objectId == -1)
				throw new Exception(string.Format("Модель с кодом \"{1}\" не найдена", model));
			Root = new PObject(rep, objectId, objectsIndex, deferredLoad);
            //Root = new PObject(new SRepositoryLinq(connectionString), model, deferredLoad, host);
        }
    }
}
