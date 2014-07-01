using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FirebirdSql.Data.FirebirdClient;
using System.Data;

namespace Repository
{
    public abstract class SCommand
    {        
        protected int _objectId { set; get; }

        public SCommand(int objectId)
        {
            _objectId = objectId;
        }

        public abstract void Execute(FbConnection conn, FbTransaction trans);

        protected List<Dictionary<string, string>> _executeReader(FbConnection conn, FbTransaction trans, string query, Dictionary<string, object> prms)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            using (FbCommand cmd = new FbCommand(query, conn, trans))
            {
                foreach (var prm in prms)
                    cmd.Parameters.AddWithValue(prm.Key, prm.Value);

                using (FbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> row = new Dictionary<string, string>();

                        Object[] values = new Object[reader.FieldCount];
                        reader.GetValues(values);

                        for (int jj = 0; jj < reader.FieldCount; jj++)
                            row.Add(reader.GetName(jj).ToLower(), values[jj].ToString());

                        ret.Add(row);
                    }
                }
            }

            return ret;
        }

        protected string _executeNonQuery(FbConnection conn, FbTransaction trans, string query, Dictionary<string, object> prms, bool isReturning)
        {
            string ret = "";

            using (FbCommand cmd = new FbCommand(query, conn, trans))
            {
                foreach (var prm in prms)
                    cmd.Parameters.AddWithValue(prm.Key, prm.Value);

                if (isReturning)
                {
                    FbParameter outparam = new FbParameter("@out", FbDbType.VarChar)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outparam);

                    cmd.ExecuteNonQuery();
                    ret = outparam.Value as string;
                }
                else
                    cmd.ExecuteNonQuery();
            }

            return ret;
        }
    }

    public class SDeleteCommand : SCommand
    {
        public SDeleteCommand(int objectId)
            : base(objectId)
        {

        }

        public override void Execute(FbConnection conn, FbTransaction trans)
        {
            Dictionary<string, object> prms = new Dictionary<string, object>();
            prms.Add("objectid", _objectId);
            _executeNonQuery(conn, trans, "DELETE FROM objects WHERE objectid = @objectid", prms, false);
        }
    }

    public class SSetAttrCommand: SCommand
    {
        protected string _name { set; get; }
        protected string _value { set; get; }

        public SSetAttrCommand(int objectId, string name, string value)
            : base (objectId)
        {
            _name = name;
            _value = value;
        }

        public override void Execute(FbConnection conn, FbTransaction trans)
        {
            List<FbCommand> ret = new List<FbCommand>();

            Dictionary<string, object> prms = new Dictionary<string, object>();
            prms.Add("attributecode", _name);

            List<Dictionary<string, string>> q = _executeReader(conn, trans, 
                "SELECT attributeid FROM attributes a WHERE UPPER(attributecode) = UPPER(@attributecode)", 
                prms);

            string attrId = "";
            if (q.Count() == 0)
            {
                string id = _executeNonQuery(conn, trans,
                    "INSERT INTO attributes (attributecode) values (upper(@attributecode)) returning attributeid",
                    prms, true);

                if (!String.IsNullOrEmpty(id))
                    attrId = id;
            }
            else
                q[0].TryGetValue("attributeid", out attrId);

            if (attrId != "")
            {
                prms = new Dictionary<string, object>();
                prms.Add("attributeid", attrId);
                prms.Add("objectid", _objectId.ToString());
                prms.Add("val", _value);
                
                _executeNonQuery(conn, trans, 
                    "UPDATE OR INSERT INTO object_attributes (attributeid, objectid, val) VALUES (@attributeid, @objectid, @val)", 
                    prms, false);
            }
        }
    }
}
