using GUI.Network.API.Models;
using GUI.Network.Models;
using GUI.Network.Services;
using GUI.Network.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public async Task<Rate> GetHistoryRate(string countryCode, string date)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestURI = _accessKeys.URI;

                requestURI += $"{date}?access_key={_accessKeys.AccessKey}&base={countryCode}";

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
                };

                return rate;
            }
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
    }
}
