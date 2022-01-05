using Gui.Data;
using GUI.Network.API;
using GUI.Network.API.Models;
using GUI.Network.Models;
using GUI.Network.Shared;
using GUI.UI.Core;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GUI.UI.MVVM.ViewModel
{
    internal class TodayIndexViewModel : ObservableObject
    {
        #region MVVM models
        private IList<SubRate> _subRates;
        public IList<SubRate> SubRates
        {
            get { return _subRates; }
            set
            {
                _subRates = value;
                OnPropertyChanged("SubRates");
            }
        }

        private Rate _todayRate;
        public Rate TodayRate
        {
            get { return _todayRate; }
            set { _todayRate = value; }
        }

        private IEnumerable<CurrencyCode> _countries;
        public IEnumerable<CurrencyCode> Countries
        {
            get { return _countries; }
            set { _countries = value; }
        }

        private CurrencyCode _selectedBase;
        public CurrencyCode SelectedBase
        {
            get { return _selectedBase; }
            set
            {
                _selectedBase = value;
                OnPropertyChanged("SelectedBase");
            }
        }

        private CurrencyCode _selectedTarget;
        public CurrencyCode SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                _selectedTarget = value;
                OnPropertyChanged("SelectedTarget");
            }
        }

        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }

        private string _baseText;

        public string BaseText
        {
            get { return _baseText; }
            set
            {
                _baseText = value;
                OnPropertyChanged("BaseText");
            }
        }

        private double _input;
        public double Input
        {
            get { return _input; }
            set
            {
                _input = value;
            }
        }

        #endregion

        #region LiveChart properties
        private ObservableCollection<double> ObservableValues { get; set; }
        public IList<ISeries> Series { get; set; }
        public List<Axis> XAxes { get; set; }
        public List<Axis> YAxes { get; set; }
        #endregion

        #region Network instances
        private readonly ExchangeRatesService _service = new();
        private readonly CommonCurrencyCodes _currencyInstance = CommonCurrencyCodes.GetInstance();
        #endregion

        #region Initialization
        public TodayIndexViewModel()
        {
            //Task.WaitAll(Task.Run(() => ConfigSystem.UpdateDatabaseAsync("EUR")));


            using (HttpClient client = new HttpClient())
            {
                string requestURI = "https://data.fixer.io/api/latest?access_key=2c534ca232efc544b377a6d0a426d2a7&base=";

                //requestURI += "latest";
                //requestURI += "?access_key=";
                //requestURI += _accessKeys.AccessKey;
                //requestURI += "&base=";
                //requestURI += countryCode;
                requestURI = requestURI + "EUR";
                //requestURI += $"latest?access_key={_accessKeys.AccessKey}&base={countryCode}";
                HttpResponseMessage response = Task.Run(() => client.GetAsync(requestURI)).Result;
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                string stringRes = Task.Run(() => response.Content.ReadAsStringAsync()).Result;
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
                        string sql_rate = "update Rate set value='" + item.Value + "' where id_Change='" + getID + "' and code='" + item.Key + "'";
                        QueryCommand.Query(sql_rate);
                    }
                }
            }

            // Get items for Countries and TodayRate
            Countries = _currencyInstance.CurrencyCodes;
          
            TodayRate = Task.Run(() => _service.GetTodayRate("EUR")).Result;

            // Set default values for SelectedBase, SelectedTarget and BaseText
            SelectedTarget = Countries.FirstOrDefault(x => x.Code.Equals("EUR"));
            SelectedBase = Countries.FirstOrDefault(x => x.Code.Equals("EUR"));
            BaseText = "Default Base: EUR";
            Input = 1;

            #region LiveChart initialization
            ObservableValues = new ObservableCollection<double>
            {
                TodayRate.Rates["CNY"],
                TodayRate.Rates["TWD"],
                TodayRate.Rates["AUD"],
                TodayRate.Rates["JPY"],
                TodayRate.Rates["THB"],
            };

            Series = new ObservableCollection<ISeries>
            {
                new ColumnSeries<double>
                {
                    Name = "Rate Exchange",
                    Values = ObservableValues,
                    AnimationsSpeed = TimeSpan.FromSeconds(2.5)
                }
            };

            XAxes = new List<Axis>
            {
                new Axis
                {
                    // Use the labels property to define named labels.
                    Labels = new List<string> { "CNY", "TWD", "AUD", "JPY", "THB" }
                }
            };

            YAxes = new List<Axis>
            {
                new Axis
                {
                    Labeler = Labelers.Default
                }
            };
            #endregion

            SubRates = _service.GetSubRates(TodayRate.Rates, Countries);
        }
        #endregion

        #region Methods
        public void AddCurrency()
        {
            // Avoid adding identical bars
            if (XAxes[0].Labels.Contains(SelectedTarget.Code)) return;

            // Add bars
            if (ObservableValues.Count >= 10)
            {
                ObservableValues.RemoveAt(0);
                XAxes[0].Labels.RemoveAt(0);
            }

            // Add labels
            ObservableValues.Add(TodayRate.Rates[SelectedTarget.Code]);
            XAxes[0].Labels.Add(SelectedTarget.Code);
        }

        public void ExchangeCurrency()
        {
            if (SelectedTarget is null)
            {
                MessageBox.Show("Invalid Target Currency. Please select again!");
                return;
            }

            double result = TodayRate.Rates[SelectedTarget.Key.ToString()];

            // Add a new bar whenever the EXCHANGE button is pressed
            AddCurrencyCommand.Execute(this);

            // Set the Result TextBlock
            Result = $"{ Input * result } :: { SelectedTarget.Symbol }";
        }

        public void SetBaseCurrency()
        {
            if (SelectedBase is null)
            {
                MessageBox.Show("Invalid Base Currency. Please select again!");
                return;
            }

            BaseText = $"Current Base: { SelectedBase.Code }";

            TodayRate = Task.Run(() => _service.GetTodayRate(SelectedBase.Code)).Result;
            SubRates = _service.GetSubRates(TodayRate.Rates, Countries);
        }
        #endregion

        #region Commands
        public ICommand AddCurrencyCommand => new RelayCommand(o => AddCurrency());
        public ICommand ExchangeCurrencyCommand => new RelayCommand(o => ExchangeCurrency());
        public ICommand SetBaseCurrencyCommand => new RelayCommand(o => SetBaseCurrency());

        #endregion
    }
}
