using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Windows.Forms;
using System.Data;

namespace Repository
{
    public class SStorageFB : IStorage
    {        
        private FbConnection _db;

        public SStorageFB(string connectionString)
        {
			string modified = this._genLocalDBPathIfLocalDB(connectionString);

			_db = new FbConnection(connectionString); // отказались от использования модифицированного пути, слишком много нюансов. Алиас на сервере надежнее и правильнее.
        }

        #region Service
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

                            //row.Add(reader.GetName(jj).ToLower(), reader.GetValue(jj).ToString());
                        }
                        ret.Add(row);
                    }
                }
            }

            return ret;
        }

        //private List<Dictionary<string, string>> _sqlSelect(string sqlQuery, string[] paramList)
        //{
        //    List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        //    _openConnection();

        //    using (FbCommand cmd = new FbCommand(sqlQuery, _db))
        //    {
        //        for (int ii = 0; ii < Math.Floor(paramList.Count() / 2.0); ii++)
        //        {
        //            cmd.Parameters.AddWithValue(paramList[ii * 2], paramList[ii * 2 + 1]);
        //        }

        //        DataTable table = new DataTable();
        //        using (FbDataReader reader = cmd.ExecuteReader())
        //        {
        //            table.Load(reader);
        //            foreach(DataRow row in table.Rows)
        //            {
        //                Dictionary<string, string> dict = new Dictionary<string, string>();
        //                foreach(DataColumn column in table.Columns)
        //                {
        //                    dict.Add(column.ColumnName.ToLower(), row[column.ColumnName].ToString());
        //                }

        //                ret.Add(dict);
        //            }
        //        }
        //    }

        //    return ret;
        //}

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
        #endregion

        #region Implementation IRepository
        private int _sqlIntField(string sqlQuery, string[] paramList, int ifEmpty)
        {
            List<Dictionary<string, string>> q = _sqlSelect(sqlQuery, paramList);
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


        public int RootObjectId(string model, int ifNotFound)
        {
            string[] paramList = { "code", model };
            return _sqlIntField(
                "select first 1 objectid_root from models where upper(modelcode) = upper(@code)", 
                paramList, ifNotFound);
        }

        public List<string> ListCollections(int objectId)
        {
            List<string> ret = new List<string>();

            string[] paramList = { "objectid", objectId.ToString() };
            List<Dictionary<string, string>> q = _sqlSelect(
                "select c.collectioncode "
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

            //string[] paramList = { "objectid", objectId.ToString() };
            //List<Dictionary<string, string>> q = _sqlSelect(
            //    "select a.attributecode, coalesce(oa.blobval, oa.val) as val "
            //    + " from object_attributes oa"
            //    + " left join attributes a on a.attributeid = oa.attributeid"
            //    + " where oa.objectid = @objectid"
            //    , paramList);

            string[] paramList = { "objectid", objectId.ToString() };
            List<Dictionary<string, string>> q = _sqlSelect(
                "select a.attributecode, oa.val as val "
                + " from object_attributes oa"
                + " left join attributes a on a.attributeid = oa.attributeid"
                + " where oa.objectid = @objectid"
                , paramList);

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

        public List<int> ListCollectionObjects(int objectId, string collName)
        {
            List<int> ret = new List<int>();
            string[] paramList = { "objectid", objectId.ToString(), "collectioncode", collName };

            List<Dictionary<string, string>> q = _sqlSelect(
                "select o.objectid"
                + " from collections c"
                + " join objects o on o.owner_collectionid = c.collectionid"
                + " where c.owner_objectid = @objectid"
                + " and upper(c.collectioncode) = upper(@collectioncode)"
                + " order by o.collection_pos"
                , paramList);

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

        public void SetAttribute(int objectId, string name, string value)
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

        #endregion
    }
}
