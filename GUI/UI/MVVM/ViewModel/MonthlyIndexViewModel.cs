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
using System.Windows;
using System.Windows.Input;

namespace GUI.UI.MVVM.ViewModel
{
    internal class MonthlyIndexViewModel : ObservableObject
    {
        private readonly DateTime _maxDate = DateTime.Today;
        private string currentDate;

        public string CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                OnPropertyChanged("CurrentDate");
            }
        }


        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
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

        private MonthlyRate _monthlyRate;

        public MonthlyRate Rates
        {
            get { return _monthlyRate; }
            set { _monthlyRate = value; }
        }


        private ObservableCollection<double> ObservableValues { get; set; }
        public IEnumerable<ISeries> SeriesCollection { get; set; }
        public List<Axis> XAxes { get; set; }
        public List<Axis> YAxes { get; set; }

        private readonly ExchangeRatesService _service = new();
        private readonly CommonCurrencyCodes _currencyInstance = CommonCurrencyCodes.GetInstance();

        public MonthlyIndexViewModel()
        {
            Countries = _currencyInstance.CurrencyCodes;
            SelectedBase = Countries.Where(x => x.Key == "EUR").FirstOrDefault();
            SelectedDate = DateTime.Today;

            Rates = Task.Run(() => _service.GetMonthlyRate("EUR", "VND", SelectedDate.ToString("yyyy-MM-dd"))).Result;

            ObservableValues = new ObservableCollection<double>();

            foreach (var item in Rates.SubRates)
                ObservableValues.Add(item.Value);

            SeriesCollection = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
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
                    Labels = new List<string>()
                }
            };
            foreach (var item in Rates.SubRates)
                XAxes[0].Labels.Add(item.TargetDate.ToString("dd"));

            YAxes = new List<Axis>
            {
                new Axis
                {
                    Labeler = Labelers.Default
                }
            };


        }

        public void ExchangeBaseCurrency()
        {
            if (SelectedTarget is null)
            {
                MessageBox.Show("Please select Target Currency");
                return;
            }

            Rates = Task.Run(() => _service.GetMonthlyRate(SelectedBase.Key, SelectedTarget.Key, SelectedDate.ToString("yyyy-MM-dd"))).Result;

            ObservableValues = new ObservableCollection<double>();

            foreach (var item in Rates.SubRates)
                ObservableValues.Add(item.Value);

            SeriesCollection = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
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
                    Labels = new List<string>()
                }
            };
            foreach (var item in Rates.SubRates)
                XAxes[0].Labels.Add(item.TargetDate.ToString("dd"));

            //if (SelectedDate > DateTime.Today)
            //{
            //    MessageBox.Show("Invalid date. Please input date smaller than today.");
            //    return;
            //}

            //CurrentDate = $"Current Date: { SelectedDate.ToString("dd-MMM-yyyy") }";
            //if (SelectedBase is null)
            //{
            //    MessageBox.Show("Invalid Base Currency. Please select again!");
            //    return;
            //}

            //BaseText = $"Current Base: { SelectedBase.Code }";

            //TodayRate = Task.Run(() => _service.GetHistoryRate(SelectedBase.Code, SelectedDate.ToString("yyyy-MM-dd"))).Result;
            //SubRates = _service.GetSubRates(TodayRate.Rates, Countries);
        }

        public ICommand ExchangeCurrencyCommand => new RelayCommand(o => ExchangeBaseCurrency());
    }
}
