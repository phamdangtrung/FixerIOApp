using System.Data.SqlClient;

namespace Data
{
    class DBSQLUtils
    {
        public static SqlConnection
            GetDBConnection(string datasource, string database, string username, string password)
        {
            //
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=12345
            //
            string connString = @"Data Source=localhost;Initial Catalog=EXCHANGE_RATE;User ID=sa;Password=S@tjsf4ction;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
