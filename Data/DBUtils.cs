using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Data
{
    class DBUtils
    {
        //Data Source=DESKTOP-7TPBTKL\NGOLUAT1;Initial Catalog=EXCHANGE_RATE;User ID=sa pass=123
        public static SqlConnection GetDBConnection()
        {
            string datasource = @"DESKTOP-7TPBTKL\NGOLUAT1";

            string database = "EXCHANGE_RATE";
            string username = "sa";
            string password = "123";

            return DBSQLUtils.GetDBConnection(datasource, database, username, password);
        }
        public static SqlConnection GetDBConnection(string dataSource,string database,string username,string password)
        {
            return DBSQLUtils.GetDBConnection(dataSource, database, username, password);
        }
    }
}
