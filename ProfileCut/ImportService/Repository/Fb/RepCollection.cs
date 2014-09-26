
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FirebirdSql.Data.FirebirdClient;

namespace ImportService.Repository.Fb
{
    internal class RepCollection
    {
  
        public int? Get(FbConnection conn, FbTransaction trans, int objectId, string code, bool createIfNotExist)
        {
            string query = "SELECT * FROM sp_get_collection(@objectid, @collectioncode, @createifnotexist)";

            int? ret = null;

            try
            {            
                using (FbCommand cmd = new FbCommand(query, conn, trans))
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
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка SQL запроса. {0}", ex.Message));
            }

            return ret;                                    
        }

        public void Delete(FbConnection conn, FbTransaction trans, int ownerObjectId, string code)
        {
            string query = "DELETE FROM collections WHERE owner_objectid = @owner_objectid AND collectioncode = UPPER(@collectioncode)";
            try
            {
                using (FbCommand cmd = new FbCommand(query, conn, trans))
                {
                    cmd.Parameters.AddWithValue("@owner_objectid", ownerObjectId);
                    cmd.Parameters.AddWithValue("@collectioncode", code);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка SQL запроса. {0}", ex.Message));
            }
        }
    }
}
