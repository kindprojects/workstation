using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace Repository
{
    class SStorageLinq : ISRepository
    {
        private FbConnection _db;
        private DataSet _ds;

        public SStorageLinq(string connectionString)
        {
            string modified = this._genLocalDBPathIfLocalDB(connectionString);

            _db = new FbConnection(connectionString);
            _ds = _loadTables();
        }

        private bool _isOpen()
        {
            return (_db.State == System.Data.ConnectionState.Open);
        }

        private void _openConnection()
        {
            if (!this._isOpen())
                this._db.Open();
        }

        private void _closeConnection()
        {
            if (this._isOpen())
                _db.Close();
        }

        private string _genLocalDBPathIfLocalDB(string connectionString)
        {
            // строит путь к файлу БД от текущей папки, если в качестве сервера указан localhost или 127.0.0.1
            FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder builder = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder(connectionString);
            string server = builder.DataSource.ToLower();
            if (server == "localhost" || server == "127.0.0.1")
            {
                if (!builder.Database.Contains(Path.VolumeSeparatorChar))
                {
                    // только если путь не полный, а относительный
                    string dbPath = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar;
                    dbPath += builder.Database;
                    builder.Database = dbPath;
                }
            }

            return builder.ConnectionString;
        }

        private DataSet _loadTables()
        {
            DataSet ret = new DataSet();

            _openConnection();

            using (FbDataAdapter da = new FbDataAdapter("select * from attributes", _db))
            {                
                da.Fill(ret, "attributes");
            }

            using (FbDataAdapter da = new FbDataAdapter("select * from collections", _db))
            {
                da.Fill(ret, "collections");
            }

            using (FbDataAdapter da = new FbDataAdapter("select * from models", _db))
            {
                da.Fill(ret, "models");
            }
            
            using (FbDataAdapter da = new FbDataAdapter("select * from objects", _db))
            {
                da.Fill(ret, "objects");
            }

            using (FbDataAdapter da = new FbDataAdapter("select * from object_attributes", _db))
            {
                da.Fill(ret, "object_attributes");
            }

            return ret;
        }

        public int RootObjectId(string model, int ifNotFound)
        {
            var res = from objectid_root in _ds.Tables["models"].AsEnumerable()
                      where objectid_root.Field<string>("modelcode") == model.ToUpper()
                      select new
                      {
                          objectid_root = objectid_root.Field<int>("objectid_root")   
                      };


            return res.First().objectid_root;
        }

        public List<string> ListCollections(int objectId)
        {
            List<string> ret = new List<string>();

            var res = from collections in _ds.Tables["collections"].AsEnumerable()
                      where collections.Field<int>("owner_objectid") == objectId
                      select new
                      {
                          collectioncode = collections.Field<string>("collectioncode")
                      };

            foreach(var row in res)
            {
                ret.Add(row.collectioncode);
            }

            return ret;
        }

        public Dictionary<string, string> ListAttributes(int objectId)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            var res = from object_attributes in _ds.Tables["object_attributes"].AsEnumerable()
                      join attributes in _ds.Tables["attributes"].AsEnumerable()
                      on object_attributes.Field<int>("attributeid") equals attributes.Field<int>("attributeid")
                      where object_attributes.Field<int>("objectid") == objectId
                      select new
                      {
                          attributecode = attributes.Field<string>("attributecode"),
                          val = object_attributes.Field<string>("val")
                      };

            foreach (var row in res)
            {
                ret.Add(row.attributecode, row.val);                                
            }

            return ret;
        }

        public List<int> ListCollectionObjects(int objectId, string collName)
        {
            List<int> ret = new List<int>();

            var res = from collections in _ds.Tables["collections"].AsEnumerable()
                      join objects in _ds.Tables["objects"].AsEnumerable()
                      on collections.Field<int>("collectionid") equals objects.Field<int?>("owner_collectionid")
                      where collections.Field<int>("owner_objectid") == objectId &&
                          collections.Field<string>("collectioncode").ToUpper() == collName.ToUpper()
                      orderby objects.Field<int>("collection_pos")
                      select new
                      {
                          objectid = objects.Field<int>("objectid")
                      };

            foreach(var row in res)
            {
                ret.Add(row.objectid);
            }

            return ret;
        }

        public void SaveAttribute(int objectId, string name, string value)
        {
            string[] paramList = { "attributecode", name };

            List<Dictionary<string, string>> q = _sqlSelect(
                "select attributeid from attributes a where attributecode = @attributecode",
                paramList
            );

            string attrId = "";
            if (q.Count() == 0)
            {
                int? retId = _sqlInsert(
                    "insert into attributes (attributecode) values (@attributecode) returning attributeid",
                    paramList);

                if (retId != null)
                    attrId = retId.ToString();
            }
            else
                q[0].TryGetValue("attributeid", out attrId);

            if (attrId != "")
            {
                string[] insParamList = { "attributeid", attrId, "objectid", objectId.ToString(), "val", value };
                int? retObjectId = _sqlInsert(
                    "update or insert into object_attributes (attributeid, objectid, val) values (@attributeid, @objectid, @val) returning objectid",
                    insParamList);
            }
        }

        private int? _sqlInsert(string sqlQuery, string[] paramList)
        {
            int? ret;

            _openConnection();

            using (FbTransaction trans = _db.BeginTransaction())
            {
                using (FbCommand cmd = new FbCommand(sqlQuery, _db, trans))
                {
                    for (int ii = 0; ii < Math.Floor(paramList.Count() / 2.0); ii++)
                    {
                        cmd.Parameters.AddWithValue(paramList[ii * 2], paramList[ii * 2 + 1]);
                    }

                    FbParameter outparam = new FbParameter("@out", FbDbType.Integer)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outparam);

                    cmd.ExecuteNonQuery();
                    ret = outparam.Value as int?;
                }

                trans.Commit();
            }

            return ret;
        }

        private List<Dictionary<string, string>> _sqlSelect(string sqlQuery, string[] paramList)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            _openConnection();

            using (FbCommand cmd = new FbCommand(sqlQuery, _db))
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

                        Object[] values = new Object[reader.FieldCount];
                        reader.GetValues(values);

                        for (int jj = 0; jj < reader.FieldCount; jj++)
                        {
                            row.Add(reader.GetName(jj).ToLower(), values[jj].ToString());
                        }
                        ret.Add(row);
                    }
                }
            }

            return ret;
        }
    }
}
