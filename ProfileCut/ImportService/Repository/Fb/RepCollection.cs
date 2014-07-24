
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FirebirdSql.Data.FirebirdClient;

namespace ImportService.Repository.Fb
{
    internal class RepCollection : Repository, IRepCollection
    {
  

        public int? Get(int objectId, string code, bool createIfNotExist)
        {
            string query = "SELECT * FROM sp_get_collection(@objectid, @collectioncode, @createifnotexist)";

            int? ret = null;

            try
            {
                using (FbConnection connection = new FbConnection(ConnectionString))
                {
                    connection.Open();
                    using (FbCommand cmd = new FbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@objectid", objectId);
                        cmd.Parameters.AddWithValue("@collectioncode", code);
                        cmd.Parameters.AddWithValue("@createifnotexist", createIfNotExist);

                        using (FbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ret = !reader.IsDBNull(0) ? reader.GetInt32(0) as int? : null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка SQL запроса. {0}", ex.Message));
            }

            return ret;                                    
        }
    }
}
