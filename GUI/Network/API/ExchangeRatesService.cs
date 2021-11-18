using GUI.Network.API.Models;
using GUI.Network.Models;
using GUI.Network.Services;
using GUI.Network.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GUI.Network.API
{
    internal class ExchangeRatesService : IRateService
    {
        private readonly KeyPair _accessKeys = AccessKeys.GetInstance().ExchangeRates;
        public async Task<Rate> GetTodayRate(string countryCode)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestURI = _accessKeys.URI;

                requestURI += "latest";
                requestURI += "?access_key=";
                requestURI += _accessKeys.AccessKey;

                HttpResponseMessage response = await client.GetAsync(requestURI);
                JsonSerializer serializer = new JsonSerializer();
                string stringRes = await response.Content.ReadAsStringAsync();
                var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));

                LinkedList<SubRate> subRateListings = new LinkedList<SubRate>();

                //foreach (var item in apiRate.Rates)
                //{
                //    subRateListings.AddLast(new SubRate { Name = item.Key, Value = item.Value });
                //}

                Rate rate = new Rate
                {
                    BaseCurrency = apiRate.Base,
                    Date = DateTime.ParseExact(apiRate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Rates = apiRate.Rates,
                };

                return rate;
            }
        }
    }
}
