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

        internal static CommonCurrencyCodes GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new();

                        _instance.CurrencyCodes = Initialize2();
                    }
                }
            }

            return _instance;
        }

        public IEnumerable<CurrencyCode> CurrencyCodes { get; private set; }

        private static IEnumerable<CurrencyCode> Initialize2()
        {
            // Get file path
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            string filePath = @"\Network\Shared\CurrencyCodes_2.json";
            filePath = projectDirectory + filePath;

            // Read file from path
            using StreamReader streamReader = new StreamReader(filePath);
            var obj = streamReader.ReadToEnd();
            var json = Encoding.UTF8.GetBytes(obj);

            // Deserialize and map to list of records
            var deserializedObject = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, dynamic>>>(json);
            LinkedList<CurrencyCode> currencyCodes = new();

            foreach (var item in deserializedObject)
            {
                currencyCodes
                    .AddLast
                    (
                        new CurrencyCode(item.Key.ToString(), item.Value["symbol"].ToString(), item.Value["name"].ToString(), item.Value["code"].ToString())
                    );
            }

            return currencyCodes.AsEnumerable();
        }
    }
}
