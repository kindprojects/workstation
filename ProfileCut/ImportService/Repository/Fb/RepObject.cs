using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace ImportService.Repository.Fb
{
    internal class RepObject : Repository, IRepObject
    {        
        public int? Add(int collectionId)
        {
            string query = "SELECT * FROM sp_add_object(@collectionId)";

            int? ret = null;

            try
            {
                using (FbConnection connection = new FbConnection(ConnectionString))
                {
                    connection.Open();
                    using (FbCommand cmd = new FbCommand(query, connection))
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
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ошибка SQL запроса. {0}", ex.Message));
            }

            return ret;                                    
        }

        public int Delete(int id)
        {
            string query = "EXECUTE PROCEDURE sp_delete_object(@id)";

            int ret = -1;

            try
            {
                using (FbConnection connection = new FbConnection(ConnectionString))
                {
                    connection.Open();
                    using (FbCommand cmd = new FbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("id", id);
                        ret = cmd.ExecuteNonQuery();
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
