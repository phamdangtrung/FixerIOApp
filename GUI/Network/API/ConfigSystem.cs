using Gui.Data;
using GUI.Network.API.Models;
using GUI.Network.Models;
using GUI.Network.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GUI.Network.API
{
    public class ConfigSystem
    {
        public static string defaultCountryCode;
        public static string URL;
        public static string DataSource = @"DESKTOP-7TPBTKL\NGOLUAT1";
        public static string Database = "EXCHANGE_RATE";
        public static string Username = "sa";
        public static string Password = "123";


        
        
        private readonly KeyPair _accessKeys = AccessKeys.GetInstance().Fixer;
        public static async void UpdateDatabaseAsync(string countryCode)
        {
            
                using (HttpClient client = new HttpClient())
                {
                    string requestURI = "https://data.fixer.io/api/latest?access_key=2c534ca232efc544b377a6d0a426d2a7&base=EUR";

                    //requestURI += "latest";
                    //requestURI += "?access_key=";
                    //requestURI += _accessKeys.AccessKey;
                    //requestURI += "&base=";
                    //requestURI += countryCode;
                     requestURI = requestURI + defaultCountryCode;
                    //requestURI += $"latest?access_key={_accessKeys.AccessKey}&base={countryCode}";
                    HttpResponseMessage response = await client.GetAsync(requestURI);
                    JsonSerializer serializer = new JsonSerializer();
                    string stringRes = await response.Content.ReadAsStringAsync();
                    var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));

                    LinkedList<SubRate> subRateListings = new LinkedList<SubRate>();

                    string setID;
                    string sql_base = "exec ps_createBase";
                    setID = QueryCommand.QueryToStored(sql_base);
                     if (setID != "-1")
                     {
                         foreach (var item in apiRate.Rates)
                         {
                             string sql_rate = "INSERT INTO [Rate] ([code],[value],[id_Change])VALUES('" + item.Key + "'," + item.Value + "," + setID + ")";
                             QueryCommand.Query(sql_rate);
                         }
                     }
                     else
                     {
                         sql_base = "select top(1) ID from Date order by ID deSC";
                         string getID = QueryCommand.QueryToStored(sql_base);
                         foreach (var item in apiRate.Rates)
                         {
                             string sql_rate = "update Rate set value='"+ item.Value + "' where id_Change='"+ getID + "' and code='"+ item.Key + "'";
                             QueryCommand.Query(sql_rate);
                         }
                     }    
                }
        }
        public static async void ThreadUpdateDatabaseAsync(string countryCode,int sleepTime)
        {
            while (true)
            {
               UpdateDatabaseAsync(countryCode);

                Thread.Sleep(sleepTime);
            }    
        }
    }
}
