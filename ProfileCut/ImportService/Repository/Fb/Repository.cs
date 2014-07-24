using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

using ImportService;

namespace ImportService.Repository.Fb
{
    internal class Repository
    {
        protected string ConnectionString { set; get; }
        public Repository()
        {
            ConnectionString = Program.Config.connectionString;
        }
    }
}
