using GUI.Database;
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
        private readonly DatabaseHelper databaseHelper = new();
        private readonly JsonSerializer serializer = new JsonSerializer();
        private readonly DateTime _maxDate = DateTime.Today;
        public async Task<Rate> GetTodayRate(string countryCode)
        {
            var result = databaseHelper.GetRecords(countryCode, DateTime.Today.ToString("yyyy-MM-dd"));

            if (string.IsNullOrEmpty(result))
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
                    string stringRes = await response.Content.ReadAsStringAsync();

                    databaseHelper.UpdateRecord(countryCode, DateTime.Today.ToString("yyyy-MM-dd"), stringRes);

                    var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));

                    Rate rate = new Rate
                    {
                        BaseCurrency = apiRate.Base,
                        Date = DateTime.ParseExact(apiRate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Rates = apiRate.Rates,
                    };

                    if (!databaseHelper.DoesSubRatesExist(countryCode, rate.Date.ToString("yyyy-MM-dd")))
                    {
                        databaseHelper.UpdateSubRates(countryCode, rate.Date.ToString("yyyy-MM-dd"), rate.Rates);
                    }

                    return rate;
                }
            }
            else
            {
                var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(result)));

                Rate rate = new Rate
                {
                    BaseCurrency = apiRate.Base,
                    Date = DateTime.ParseExact(apiRate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Rates = apiRate.Rates,
                };

                if (!databaseHelper.DoesSubRatesExist(countryCode, rate.Date.ToString("yyyy-MM-dd")))
                {
                    databaseHelper.UpdateSubRates(countryCode, rate.Date.ToString("yyyy-MM-dd"), rate.Rates);
                }

                return rate;
            }


        }

        public async Task<Rate> GetHistoryRate(string countryCode, string date)
        {
            var result = databaseHelper.GetRecords(countryCode, date);

            if (string.IsNullOrEmpty(result))
            {
                using (HttpClient client = new HttpClient())
                {
                    string requestURI = _accessKeys.URI;

                    requestURI += $"{date}?access_key={_accessKeys.AccessKey}&base={countryCode}";

                    HttpResponseMessage response = await client.GetAsync(requestURI);
                    string stringRes = await response.Content.ReadAsStringAsync();

                    databaseHelper.UpdateRecord(countryCode, date, stringRes);

                    var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));

                    Rate rate = new Rate
                    {
                        BaseCurrency = apiRate.Base,
                        Date = DateTime.ParseExact(apiRate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Rates = apiRate.Rates,
                    };

                    if (!databaseHelper.DoesSubRatesExist(countryCode, rate.Date.ToString("yyyy-MM-dd")))
                    {
                        databaseHelper.UpdateSubRates(countryCode, rate.Date.ToString("yyyy-MM-dd"), rate.Rates);
                    }

                    return rate;
                }
            }
            else
            {
                var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(result)));

                Rate rate = new Rate
                {
                    BaseCurrency = apiRate.Base,
                    Date = DateTime.ParseExact(apiRate.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Rates = apiRate.Rates,
                };

                if (!databaseHelper.DoesSubRatesExist(countryCode, rate.Date.ToString("yyyy-MM-dd")))
                {
                    databaseHelper.UpdateSubRates(countryCode, rate.Date.ToString("yyyy-MM-dd"), rate.Rates);
                }

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

        public async Task<MonthlyRate> GetMonthlyRate(string countryCode, string targetCode, string date)
        {
            var convertedDate = Convert.ToDateTime(date);
            if (convertedDate > _maxDate)
            {
                return default;
            }

            if (convertedDate <= _maxDate)
            {
                await EnsureMonthlyRate(countryCode, date);

                MonthlyRate rate;
                rate = databaseHelper.GetMonthlyRate(countryCode, targetCode, date);
                return rate;
            }

            return default;
        }

        public async Task EnsureMonthlyRate(string countryCode, string date)
        {
            var convertedDate = Convert.ToDateTime(date);
            if (convertedDate > _maxDate)
            {
                return;
            }
            if (convertedDate.Month == _maxDate.Month && convertedDate.Year == _maxDate.Year)
            {
                for (int i = 1; i <= _maxDate.Day; i++)
                {
                    string tempDate = $"{_maxDate.Year}-{_maxDate.Month}-{i}";
                    if (!databaseHelper.DoesSubRatesExist(countryCode, tempDate))
                    {
                        var result = databaseHelper.GetRecords(countryCode, tempDate);

                        if (string.IsNullOrEmpty(result))
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                string requestURI = _accessKeys.URI;

                                requestURI += $"{tempDate}?access_key={_accessKeys.AccessKey}&base={countryCode}";

                                HttpResponseMessage response = await client.GetAsync(requestURI);
                                string stringRes = await response.Content.ReadAsStringAsync();

                                databaseHelper.UpdateRecord(countryCode, tempDate, stringRes);
                                var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));
                                databaseHelper.UpdateSubRates(countryCode, tempDate, apiRate.Rates);
                            }
                        }
                        else
                        {
                            databaseHelper.UpdateRecord(countryCode, tempDate, result);
                            var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(result)));
                            databaseHelper.UpdateSubRates(countryCode, tempDate, apiRate.Rates);
                        }
                    }
                }
                return;
            }


            for (int i = 1; i <= convertedDate.Day; i++)
            {
                string tempDate = "";
                if (i < 10)
                {
                    tempDate = $"{convertedDate.Year}-{convertedDate.Month}-0{i}";
                }
                else
                {
                    tempDate = $"{convertedDate.Year}-{convertedDate.Month}-{i}";
                }
                if (!databaseHelper.DoesSubRatesExist(countryCode, tempDate))
                {
                    var result = databaseHelper.GetRecords(countryCode, tempDate);

                    if (string.IsNullOrEmpty(result))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string requestURI = _accessKeys.URI;

                            requestURI += $"{tempDate}?access_key={_accessKeys.AccessKey}&base={countryCode}";

                            HttpResponseMessage response = await client.GetAsync(requestURI);
                            string stringRes = await response.Content.ReadAsStringAsync();

                            databaseHelper.UpdateRecord(countryCode, tempDate, stringRes);
                            var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(stringRes)));
                            databaseHelper.UpdateSubRates(countryCode, tempDate, apiRate.Rates);
                        }
                    }
                    else
                    {
                        //databaseHelper.UpdateRecord(countryCode, tempDate, result);
                        var apiRate = serializer.Deserialize<APIRate>(new JsonTextReader(new StringReader(result)));
                        databaseHelper.UpdateSubRates(countryCode, tempDate, apiRate.Rates);
                    }
                }
            }

        }
    }
}
