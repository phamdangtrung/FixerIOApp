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
    internal class ConverterViewModel: ObservableObject
    {
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

        private CurrencyCode _selectedFrom;
        public CurrencyCode SelectedFrom
        {
            get { return _selectedFrom; }
            set
            {
                _selectedFrom = value;
                OnPropertyChanged("SelectedFrom");
            }
        }
        private CurrencyCode _selectedTo;
        public CurrencyCode SelectedTo
        {
            get { return _selectedTo; }
            set
            {
                _selectedTo = value;
                OnPropertyChanged("SelectedTo");
            }
        }
        private int _input;
        public int Input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged("Input");
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
        private readonly ExchangeRatesService _service = new();
        private readonly CommonCurrencyCodes _currencyInstance = CommonCurrencyCodes.GetInstance();
        public ConverterViewModel()
        {
            Countries = _currencyInstance.CurrencyCodes;
            SelectedDate = DateTime.Today;
            Input = 1;
            SelectedFrom = Countries.FirstOrDefault(x => x.Code.Equals("EUR"));
            SelectedTo = Countries.FirstOrDefault(x => x.Code.Equals("VND"));

        }
        private void OnClick1(object sender, RoutedEventArgs e)
        {
            if (SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Invalid date. Please input date smaller than today.");
                return;
            }
            string date = $"Current Date: { SelectedDate.ToString("dd-MMM-yyyy") }";
            if (SelectedFrom is null)
            {
                MessageBox.Show("Invalid Base Currency. Please select again!");
                return;
            }
            if (SelectedTo is null)
            {
                MessageBox.Show("Invalid Base Currency. Please select again!");
                return;
            }
            double a = Task.Run(() => _service.GetConvertTo(SelectedFrom.Code, SelectedTo.Code, SelectedDate.ToString("yyyy-MM-dd"), Input)).Result;
            Result = $"{ a } ";
        }
        #region Commands
  
       

        #endregion
    }
}
