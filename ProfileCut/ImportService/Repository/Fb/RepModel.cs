using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace ImportService.Repository.Fb
{
    internal class RepModel: Repository, IRepModel
    {
        public IModel Select(string code)
        {
            string query = "SELECT * FROM models WHERE modelcode = UPPER(@modelcode)";

            IModel ret = null;

            try
            {
                using (FbConnection connection = new FbConnection(ConnectionString))
                {
                    connection.Open();
                    using (FbCommand cmd = new FbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("modelcode", code.ToUpper());
                        using (FbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ret = new Model
                                {
                                    Id = reader.GetInt32(0),
                                    Code = reader.GetString(1),
                                    ObjectIdRoot = reader.GetInt32(2)
                                };
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
