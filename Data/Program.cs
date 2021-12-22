using System;
using System.Data.SqlClient;

namespace Data
{
    class Program
    {
        static void Main(string[] args)
        {
            string sql="";
            SqlConnection conn = DBUtils.GetDBConnection();
            QueryCommand.QueryEmployee( conn, sql);
        }
    }
}
