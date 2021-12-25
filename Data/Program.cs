using System;
using System.Data.SqlClient;

namespace Data
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 4;
            string sql="exec te "+a;
            SqlConnection conn = DBUtils.GetDBConnection();
           Console.WriteLine( QueryCommand.QueryToStored(sql));
            Console.Read();
        }
    }
}
