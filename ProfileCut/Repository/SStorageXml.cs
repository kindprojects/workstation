﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
    class SStorageXml : IStorage
    {
        private string _fileName;

        public SStorageXml(string fileName)
        {
            _fileName = fileName;
        }

        #region Implementation IRepository
        public int RootObjectId(string modelCode, int ifNotFound)
        {
            throw new NotImplementedException();
        }

        public List<string> ListCollections(int objectId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ListAttributes(int objectId)
        {
            throw new NotImplementedException();
        }

        public List<int> ListCollectionObjects(int objectId, string collName)
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(int objectId, string name, string value)
        {
            throw new NotImplementedException();
        }
		public bool ObjectExists(int objectId)
		{
			throw new NotImplementedException();
		}        

        public void DeleteObject(int objectId)
        {
            throw new NotImplementedException();
        }

        public bool Commit()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Rollback()
        {
            throw new NotImplementedException();
        }


        public List<string> GetModelCodes()
        {
            throw new NotImplementedException();
        }
    }
}
