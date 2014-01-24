using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using FirebirdSql.Data.FirebirdClient;


namespace Api
{
    class RFbRepository
    {
        protected string ConnectionString { get; set; }

        public RFbRepository()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["FireBirdConnection"].ConnectionString;               
        }
    }
}
