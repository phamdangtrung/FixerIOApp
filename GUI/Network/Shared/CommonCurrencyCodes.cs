using GUI.Network.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GUI.Network.Shared
{
    internal class CommonCurrencyCodes
    {
        private CommonCurrencyCodes() { }

        private static CommonCurrencyCodes _instance;

        private static readonly object _lock = new();

        public IEnumerable<CurrencyCode> CurrencyCodes { get; private set; }

        internal static CommonCurrencyCodes GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new();

                        _instance.CurrencyCodes = Initialize();
                    }
                }
            }

            return _instance;
        }

        private static IEnumerable<CurrencyCode> Initialize()
        {
            // Get file path
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string filePath = @"\Network\Shared\CurrencyCodes.json";
            filePath = projectDirectory + filePath;

            // Read file from path
            using StreamReader streamReader = new StreamReader(filePath);
            var obj = streamReader.ReadToEnd();
            var json = Encoding.UTF8.GetBytes(obj);

            // Deserialize and map to list of records
            var deserializedObject = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            LinkedList<CurrencyCode> currencyCodes = new();

            foreach (var item in deserializedObject)
            {
                currencyCodes
                    .AddLast
                    (
                        new CurrencyCode(item["country"], item["currency_code"])
                    );
            }

            return currencyCodes.AsEnumerable();
        }
    }
}
