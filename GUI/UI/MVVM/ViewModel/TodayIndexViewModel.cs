using GUI.Network.API;
using GUI.Network.Models;
using GUI.Network.Shared;
using GUI.UI.Core;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GUI.UI.MVVM.ViewModel
{
    internal class TodayIndexViewModel : ObservableObject
    {
        private bool _firstInit = true;
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
                if (_firstInit)
                {
                    _selectedBase = value;
                    //OnPropertyChanged("SelectedBase");

                    _firstInit = false;
                    return;
                }
                _selectedBase = value;
                OnPropertyChanged("SelectedBase");
                AddCurrencyCommand.Execute(this);
            }
        }

        private CurrencyCode _selectedTarget;

        public CurrencyCode SelectedTarget
        {
            get { return _selectedTarget; }
            set { _selectedTarget = value; }
        }



        private ObservableCollection<double> ObservableValues { get; set; }
        public IList<ISeries> Series { get; set; }
        public List<Axis> XAxes { get; set; }
        public List<Axis> YAxes { get; set; }

        private readonly ExchangeRatesService service = new();

        public TodayIndexViewModel()
        {

            Countries = CommonCurrencyCodes.GetInstance().CurrencyCodes;

            //SelectedBase = Countries.FirstOrDefault(x => x.Currency_Code == "EUR");

            TodayRate = Task.Run(() => service.GetTodayRate("EUR")).Result;

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
                    // Now the Y axis we will display labels as currency
                    // LiveCharts provides some common formatters
                    // in this case we are using the currency formatter.
                    Labeler = Labelers.Default

                    // you could also build your own currency formatter
                    // for example:
                    // Labeler = (value) => value.ToString("C")

                    // But the one that LiveCharts provides creates shorter labels when
                    // the amount is in millions or trillions
                }
            };
        }

        public void AddCurrency(string currencyCode)
        {
            MessageBox.Show(currencyCode);

            if (XAxes[0].Labels.Contains(currencyCode)) return;

            if (ObservableValues.Count >= 10)
            {
                ObservableValues.RemoveAt(0);
                XAxes[0].Labels.RemoveAt(0);
            }

            ObservableValues.Add(TodayRate.Rates[currencyCode]);
            XAxes[0].Labels.Add(currencyCode);
        }

        public ICommand AddCurrencyCommand => new RelayCommand(o => AddCurrency(SelectedBase.Currency_Code.ToString()));
    }
}
