using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace ImportService.Repository.Fb
{
    internal class RepObject
    {        
        public int? Add(FbConnection conn, FbTransaction trans, int collectionId)
        {
            string query = "SELECT * FROM sp_add_object(@collectionId)";

            int? ret = null;

            try
            {

                using (FbCommand cmd = new FbCommand(query, conn, trans))
                {
                    cmd.Parameters.AddWithValue("collectionId", collectionId);
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

        public int Delete(FbConnection conn, FbTransaction trans, int id)
        {
            string query = "EXECUTE PROCEDURE sp_delete_object(@id)";

            int ret = -1;

            try
            {
                using (FbCommand cmd = new FbCommand(query, conn, trans))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    ret = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка SQL запроса. {0}", ex.Message));
            }

            return ret;                                       
        }

        public List<int> Find(FbConnection conn, FbTransaction trans, int ownerObjectId, string collectionName, string attrName, string attrValue)
        {
            string query = "SELECT objectid FROM sp_find_objects(@ownerObjectId, @collectionName, @attrName, '=', @attrValue)";

            List<int> ret = new List<int>();

            try
            {

                using (FbCommand cmd = new FbCommand(query, conn, trans))
                {
                    cmd.Parameters.AddWithValue("ownerObjectId", ownerObjectId);
                    cmd.Parameters.AddWithValue("collectionName", collectionName);
                    cmd.Parameters.AddWithValue("attrName", attrName);
                    cmd.Parameters.AddWithValue("attrValue", attrValue);

                    using (FbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ret.Add(reader.GetInt32(0));
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
