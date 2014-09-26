using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FirebirdSql.Data.FirebirdClient;

namespace ImportService.Repository.Fb
{
    internal class RepAttribute
    {       
        public void Set(FbConnection conn, FbTransaction trans,  int objectId, string code, string val)
        {
            string query = "EXECUTE PROCEDURE sp_set_attribute(@objectId, @attributeCode, @val)";

            try
            {           
                using (FbCommand cmd = new FbCommand(query, conn, trans))
                {
                    cmd.Parameters.AddWithValue("objectId", objectId);
                    cmd.Parameters.AddWithValue("attributeCode", code);
                    cmd.Parameters.AddWithValue("val", val);

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
