using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Data
{
    class QueryCommand
    {
        public static void QueryEmployee(SqlConnection conn,string sql)
        {
            sql = "select  * from Base";

            conn = DBUtils.GetDBConnection();
            conn.Open();
            try
            {
                DataSet ds = new DataSet();
                // Tạo một đối tượng Command.
                SqlDataAdapter dap = new SqlDataAdapter(sql, conn);

                // Liên hợp Command với Connection.
                dap.Fill(ds);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                // Đóng kết nối.
                conn.Close();
                // Hủy đối tượng, giải phóng tài nguyên.
                conn.Dispose();
            }
            Console.Read();
        }
    }
}
