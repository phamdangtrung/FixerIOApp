
using Gui.Data;
using GUI.Network.API.Models;
using GUI.Network.Models;
using GUI.Network.Services;
using GUI.Network.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GUI.Network.API
{
    internal class ExchangeRatesService : IRateService
    {
        private readonly KeyPair _accessKeys = AccessKeys.GetInstance().Fixer;
        public async Task<Rate> GetTodayRate(string countryCode)
        {
            ConfigSystem.UpdateDatabaseAsync(ConfigSystem.defaultCountryCode);
            using (HttpClient client = new HttpClient())
            {
                /*string requestURI = _accessKeys.URI;

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
                }*/
                string date = DateTime.Today.ToString("yyyy-MM-dd");
                string sql_base = "select distinct code from Rate";
                DataSet tab = QueryCommand.QueryToData(sql_base);
                List<string> list = new List<string>();
                foreach (DataRow item in tab.Tables[0].Rows)
                {
                    list.Add(item.ItemArray[0].ToString().Trim());
                }
                LinkedList<SubRate> subRateListings = new LinkedList<SubRate>();
                Dictionary<string, double> rates = new Dictionary<string, double>();
                foreach (string item in list)
                {
                    string sqlst = "exec ps_convertTo '" + countryCode.ToString() + "','" + item.ToString() + "','" + date.ToString() + "',1";
                    rates.Add(item, Convert.ToDouble(QueryCommand.QueryToStored(sqlst).ToString()));
                }
                
                Rate rate = new Rate
                {
                    BaseCurrency = countryCode,
                    Date = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Rates = rates,
                };

                return rate;
            }
        }

        public async Task<Rate> GetHistoryRate(string countryCode, string date)
        {
            string sql_base = "select distinct code from Rate";
            DataSet tab= QueryCommand.QueryToData(sql_base);
            List<string> list = new List<string>();
            foreach(DataRow item in tab.Tables[0].Rows)
            {
                list.Add(item.ItemArray[0].ToString().Trim());
            }
            LinkedList<SubRate> subRateListings = new LinkedList<SubRate>();
            Dictionary<string, double> rates = new Dictionary<string, double>();
            foreach (string item in list)
            {
                string sqlst = "exec ps_convertTo '" + countryCode.ToString() + "','" + item.ToString() + "','" + date.ToString() + "',1";
                rates.Add(item, Convert.ToDouble(QueryCommand.QueryToStored(sqlst).ToString()));
            }

            Rate rate = new Rate
            {
                BaseCurrency = countryCode,
                Date = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Rates = rates,
                //{[AED, 4.151952]}

            };
            return rate;
            /*
            using (HttpClient client = new HttpClient())
            {
                string requestURI = _accessKeys.URI;

                requestURI += $"{date}?access_key={_accessKeys.AccessKey}&base={countryCode}";
                //requestURI = "https://vi.coinmill.com/EUR_calculator.html#EUR=1";
                HttpResponseMessage response = await client.GetAsync(requestURI);
                JsonSerializer serializer = new JsonSerializer();
                string stringRes = await response.Content.ReadAsStringAsync();
                var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));

                LinkedList<SubRate> subRateListings = new LinkedList<SubRate>();
                
               Rate rate = new Rate
                {
                    BaseCurrency = apiRate.Base,
                    Date = DateTime.ParseExact(apiRate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Rates = apiRate.Rates,
                    //{[AED, 4.151952]}
                    
                };
                return rate;
            }*/


            
        }

        public List<SubRate> GetSubRates(Dictionary<string, double> subRates, IEnumerable<CurrencyCode> countries)
        {
            LinkedList<SubRate> newSubRates = new();
            foreach (var item in subRates)
            {
                var country = countries.FirstOrDefault(x => x.Code.Equals(item.Key.ToString()));

                if (country is null) continue;

                newSubRates
                    .AddLast
                    (
                        new SubRate(country.Name, item.Key.ToString(), item.Value)
                    );
            }

            return newSubRates.ToList();
        }
        public double GetConvertTo(string countryCodeFrom, string countryCodeTo, string date, int amount)
        {
            string sqlst = "exec ps_convertTo '" + countryCodeFrom + "','" + countryCodeTo + "','" + date.ToString() + "'," + amount;
        
            return Convert.ToDouble(QueryCommand.QueryToStored(sqlst).ToString());
        }
        public List<double> getValueInMonth(string countryCodeFrom, string date)
        {
            string sqlst = "exec ps_rateOnMonth '"+"'"+countryCodeFrom+"','"+date+"'";
            DataSet dataSet= QueryCommand.QueryToData(sqlst);
            List<double> listValue = new List<double>();
            foreach(DataRow item in dataSet.Tables[0].Rows)
            {
                listValue.Add(Convert.ToDouble(item.ItemArray[1]));
            }
            return listValue;
        }
        public List<double> getValueInYear(string countryCodeFrom, string date)
        {
            string sqlst = "exec ps_rateOnYear '" + "'" + countryCodeFrom + "','" + date + "'";
            DataSet dataSet = QueryCommand.QueryToData(sqlst);
            List<double> listValue = new List<double>();
            foreach (DataRow item in dataSet.Tables[0].Rows)
            {
                listValue.Add(Convert.ToDouble(item.ItemArray[1]));
            }
            return listValue;
        }
        public double getpercent(string countryCodeFrom, string date)
        {
            string sqlst = "exec ratechange '" + "'"+countryCodeFrom+"','" + date + "'";
            string percent = QueryCommand.QueryToStored(sqlst);
            return Convert.ToDouble(percent);
        }
        public List<double> getListPercent(string date)
        {
            string sql_base = "select distinct code from Rate";
            DataSet tab = QueryCommand.QueryToData(sql_base);
            List<string> list = new List<string>();
            foreach (DataRow item in tab.Tables[0].Rows)
            {
                list.Add(item.ItemArray[0].ToString().Trim());
            }
            List<double> listPercent = new List<double>();
            foreach (string item in list)
            {
                double percent = getpercent(item, date);
                listPercent.Add(percent);
            }
            return listPercent;
        }
     }
}
