using GUI.Network.API.Models;
using GUI.Network.Models;
using GUI.Network.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ConfigSystem
    {
        public static string defaultCountryCode;
        public static int sleepTime;
        public static string keyAPI;
        public static string URL;
        public static string datasource = @"DESKTOP-7TPBTKL\NGOLUAT1";
        public static string database = "EXCHANGE_RATE";
        public static string username = "sa";
        public static string password = "123";

        public void getValueFromData()
        {
            sleepTime = Int32.Parse(QueryCommand.QueryToStored("select sleepTime from SYSTEMCONFIG"));
            defaultCountryCode = QueryCommand.QueryToStored("select countryCodeDefault from SYSTEMCONFIG");
            keyAPI = QueryCommand.QueryToStored("select APIkey from SYSTEMCONFIG");
        }
        public static void setInfoConnectToData(string _datasource,string _databaseName,string _userName,string _password)
        {
            datasource = _datasource;
            database = _databaseName;
            username = _userName;
            password = _password;
        }
        private readonly KeyPair _accessKeys = AccessKeys.GetInstance().Fixer;
        public async void UpdateDatabaseAsync(string countryCode)
        {

            using (HttpClient client = new HttpClient())
            {
                string requestURI = _accessKeys.URI;

                //requestURI += "latest";
                //requestURI += "?access_key=";
                //requestURI += _accessKeys.AccessKey;
                //requestURI += "&base=";
                //requestURI += countryCode;

                requestURI += $"latest?access_key={_accessKeys.AccessKey}&base={countryCode}";
                HttpResponseMessage response = await client.GetAsync(requestURI);
                JsonSerializer serializer = new JsonSerializer();
                string stringRes = await response.Content.ReadAsStringAsync();
                var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));

                LinkedList<SubRate> subRateListings = new LinkedList<SubRate>();

                string setID;
                string sql_base = "exec ps_createBase";
                setID = QueryCommand.QueryToStored(sql_base);
                if(setID!="-1")
                foreach (var item in apiRate.Rates)
                {
                    string sql_rate = "INSERT INTO [Rate] ([code],[value],[id_Change])VALUES('" + item.Key + "'," + item.Value + "," + setID + ")";
                    QueryCommand.Query(sql_rate);
                }
            }

        }
    }
}
