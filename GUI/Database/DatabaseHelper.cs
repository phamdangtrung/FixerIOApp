using GUI.Network.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GUI.Database
{
    internal class DatabaseHelper
    {
        private string connectionString = "Data Source=localhost;Initial Catalog=Rate;User ID=sa;Password=S@tjsf4ction;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        internal void UpdateRecord(string baseCurrency, string targetDate, string jsonString)
        {
            using var connection = new SqlConnection(connectionString);
            var command = new SqlCommand("Update_Record", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@BaseCurrency", baseCurrency));
            command.Parameters.Add(new SqlParameter("@TargetDate", targetDate));
            command.Parameters.Add(new SqlParameter("@JsonString", jsonString));

            connection.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            connection.Close();
        }

        internal string GetRecords(string baseCurrency, string targetDate)
        {
            using var connection = new SqlConnection(connectionString);
            var command = new SqlCommand("Get_Record", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@BaseCurrency", baseCurrency));
            command.Parameters.Add(new SqlParameter("@TargetDate", targetDate));

            connection.Open();
            using var reader = command.ExecuteReader();

            string result = "";
            if (reader.HasRows)
            {
                reader.Read();
                result = reader["Result"]?.ToString() ?? "";
            }
            command.Dispose();
            connection.Close();
            return result;
        }

        internal void UpdateSubRates(string baseCurrency, string date, Dictionary<string, double> rates)
        {
            string baseSql = $"INSERT INTO SubRates(BaseCurrency, TargetCurrency, TargetDate, Value) VALUES ";
            int count = 1;
            int max = rates.Count();
            foreach (var item in rates)
            {
                if (count == max)
                {
                    baseSql += $"('{ baseCurrency }', '{ item.Key }', CONVERT(date, '{ date }'), '{ item.Value }');";
                    break;
                }

                baseSql += $"('{ baseCurrency }', '{ item.Key }', CONVERT(date, '{ date }'), '{ item.Value }'), ";
                count++;
            }

            using var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(baseSql, connection);
            command.CommandType = CommandType.Text;

            connection.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            connection.Close();
        }

        internal bool DoesSubRatesExist(string baseCurrency, string targetDate)
        {
            using var connection = new SqlConnection(connectionString);
            var command = new SqlCommand("SubRatesExists", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@BaseCurrency", baseCurrency));
            command.Parameters.Add(new SqlParameter("@Date", targetDate));
            var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;

            connection.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            connection.Close();

            if ((int)returnParameter.Value == 0)
            {
                return true;
            }

            return false;
        }

        internal MonthlyRate GetMonthlyRate(string baseCurrency, string targetCurrency, string targetDate)
        {
            using var connection = new SqlConnection(connectionString);
            var command = new SqlCommand("GetMonthlyIndex", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@BaseCurrency", baseCurrency));
            command.Parameters.Add(new SqlParameter("@TargetCurrency", targetCurrency));
            command.Parameters.Add(new SqlParameter("@StartDate", targetDate));

            connection.Open();
            using var reader = command.ExecuteReader();

            MonthlyRate monthlyRate = new();
            LinkedList<SubMonthlyRate> subRates = new LinkedList<SubMonthlyRate>();

            if (!reader.HasRows)
            {
                return default;
            }

            while (reader.Read())
            {
                subRates.AddLast(new SubMonthlyRate
                (
                    DateTime.Parse(reader["TargetDate"].ToString()),
                    Convert.ToDouble(reader["Value"].ToString())
                ));
            }

            monthlyRate.BaseCurrency = baseCurrency;
            monthlyRate.TargetCurrency = targetCurrency;
            monthlyRate.SubRates = subRates.ToList();

            return monthlyRate;
        }
    }
}
