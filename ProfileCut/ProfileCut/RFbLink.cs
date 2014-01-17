using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FirebirdSql.Data.FirebirdClient;

namespace Model
{
    public class RFbLink : IDBLink
    {
        private FbConnection _db;

        private bool IsOpen()
        {
            return (_db.State == System.Data.ConnectionState.Open);
        }

        public RFbLink(string ConnectionString)
        {
            this._db = new FbConnection(ConnectionString);
        }

        private void _ConnectDB()
        {
            if (!this.IsOpen())
                this._db.Open();
        }

        public void DisconnectDB()
        {
            if (this.IsOpen())
                _db.Close();
        }

        private List<Dictionary<string, string>> SqlSelect(string sqlQuery, string[] paramList)
        {
            this._ConnectDB();
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();
            using (FbCommand cmd = new FbCommand(sqlQuery, this._db))
            {
                for (int ii = 0; ii < Math.Floor(paramList.Count() / 2.0); ii++)
                {
                    cmd.Parameters.AddWithValue(paramList[ii * 2], paramList[ii * 2 + 1]);
                }

                using (FbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> row = new Dictionary<string, string>();
                        for (int jj = 0; jj < reader.FieldCount; jj++)
                        {
                            row.Add(reader.GetName(jj).ToLower(), reader.GetString(jj));
                        }
                        ret.Add(row);
                    }
                }
            }
            return ret;
        }

        private int SqlIntField(string sqlQuery, string[] paramList, int ifEmpty)
        {
            List<Dictionary<string, string>> q = this.SqlSelect(sqlQuery, paramList);
            if (q.Count() > 0)
            {
                string str;
                string key = q[0].Keys.First();
                if (q[0].TryGetValue(key, out str))
                {
                    return Convert.ToInt32(str);
                }
            }
            
            return ifEmpty;
        }

        public List<string> ListCollections(int objectId)
        {
            List<string> ret = new List<string>();
            
            string[] paramList = { "objectid", objectId.ToString() };
            List<Dictionary<string, string>> q = this.SqlSelect("select c.collectioncode "
                + " from collections c "
                + " where c.owner_objectid = @objectid", paramList);

            foreach (Dictionary<string, string> row in q)
            {
                string collCode = "";
                if (row.TryGetValue("collectioncode", out collCode))
                {
                    ret.Add(collCode);
                }
            }

            return ret;
        }

        public Dictionary<string, string> ListAttributes(int objectId)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            
            string[] paramList = { "objectid", objectId.ToString() };
            List<Dictionary<string, string>> q = this.SqlSelect("select a.attributecode, oa.val "
                + " from object_attributes oa"
                + " left join attributes a on a.attributeid = oa.attributeid"
                + " where oa.objectid = @objectid", paramList);
            foreach (Dictionary<string, string> row in q)
            {
                string attributeCode = "";
                string val = "";

                if (row.TryGetValue("attributecode", out attributeCode) && row.TryGetValue("val", out val))
                {
                    ret.Add(attributeCode, val);
                }
            }

            return ret;
        }

        public Dictionary<string, string> ListTemplates(string modelCode)
        {
            Dictionary<string,string> ret = new Dictionary<string,string>();

            string[] paramList = { "code", modelCode};
            List<Dictionary<string, string>> q = this.SqlSelect("select t.templatecode, t.templatedata"
                + " from models m"
                + " join templates t on t.modelid = m.modelid"
                + " where upper(m.modelcode) = upper(@code)", paramList);
            foreach (Dictionary<string, string> row in q)
            {
                string tplName = "";
                string tplData = "";
                if (row.TryGetValue("templatecode", out tplName)
                    && row.TryGetValue("templatedata", out tplData)
                  )
                {
                    ret.Add(tplName, tplData);
                }
            }
            
            return ret;
        }

        public int GetModelRootID(string modelCode, int ifNotFound)
        {
            string[] paramList = {"code", modelCode};
            return this.SqlIntField("select first 1 objectid_root from models where upper(modelcode) = upper(@code)", paramList, ifNotFound);
        }

        public List<int> ListCollectionObjects(int objectId, string collName)
        {
            List<int> ret = new List<int>();
            string[] paramList = { "objectid", objectId.ToString() , "collectioncode", collName};

            List<Dictionary<string, string>> q = this.SqlSelect("select o.objectid"
                + " from collections c"
                + " join objects o on o.owner_collectionid = c.collectionid"
                + " where c.owner_objectid = @objectid"
                + " and upper(c.collectioncode) = upper(@collectioncode)"
                + " order by o.collection_pos", paramList);
            
            foreach (Dictionary<string, string> row in q)
            {
                string id = "";                
                if (row.TryGetValue("objectid", out id))
                {
                    ret.Add(Convert.ToInt32(id));
                }
            }
            
            return ret;
        }
    }
}
