using GUI.Network.API;
using GUI.Network.Models;
using GUI.Network.Shared;
using GUI.UI.Core;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
            set { _subRates = value; }
        }

        private Rate _todayRate;
        public Rate TodayRate
        {
            get { return _todayRate; }
            set { _todayRate = value; }
        }

        private IEnumerable<CurrencyCode2> _countries;
        public IEnumerable<CurrencyCode2> Countries
        {
            get { return _countries; }
            set { _countries = value; }
        }

        private CurrencyCode2 _selectedBase;
        public CurrencyCode2 SelectedBase
        {
            get { return _selectedBase; }
            set
            {
                _selectedBase = value;
                OnPropertyChanged("SelectedBase");
            }
        }

        private CurrencyCode2 _selectedTarget;
        public CurrencyCode2 SelectedTarget
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
            Countries = _currencyInstance.CurrencyCodes2;

            TodayRate = Task.Run(() => _service.GetTodayRate("EUR")).Result;

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

            LinkedList<SubRate> subRates = new();

            foreach (var item in TodayRate.Rates)
            {
                var country = Countries.FirstOrDefault(x => x.Code.Equals(item.Key.ToString()));

                if (country is null) continue;

                subRates
                    .AddLast(new SubRate(country.Name, item.Key.ToString(), item.Value));
            }

            SubRates = subRates.ToList();
        }
        #endregion

        #region Methods
        public void AddCurrency(string currencyCode)
        {
            if (XAxes[0].Labels.Contains(currencyCode)) return;

            // Add bars
            if (ObservableValues.Count >= 10)
            {
                ObservableValues.RemoveAt(0);
                XAxes[0].Labels.RemoveAt(0);
            }

            // Add labels
            ObservableValues.Add(TodayRate.Rates[currencyCode]);
            XAxes[0].Labels.Add(currencyCode);

        }

        public void ExchangeCurrency(double result, CurrencyCode2 targetCurrency)
        {
            AddCurrencyCommand.Execute(this);
            Result = $"{ result } :: { targetCurrency.Symbol }";
        }
        #endregion

        #region Commands
        public ICommand AddCurrencyCommand => new RelayCommand(o => AddCurrency(SelectedTarget.Code.ToString()));
        public ICommand ExchangeCurrencyCommand => new RelayCommand(o => ExchangeCurrency(TodayRate.Rates[SelectedTarget.Key.ToString()], SelectedTarget));

        #endregion
    }
}
