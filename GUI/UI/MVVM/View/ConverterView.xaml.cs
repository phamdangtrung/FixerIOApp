using GUI.Network.API;
using GUI.Network.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GUI.UI.MVVM.View
{
    /// <summary>
    /// Interaction logic for ConverterView.xaml
    /// </summary>
    public partial class ConverterView : UserControl
    {
        private readonly ExchangeRatesService _service = new();
        public ConverterView()
        {
            InitializeComponent();
        }
        private void converto(object sender, RoutedEventArgs e)
        {
            if (selectedDate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Invalid date. Please input date smaller than today.");
                return;
            }
            DateTime a = (DateTime)selectedDate.SelectedDate;
            string ad=a.ToString("yyyy-MM-dd");
            CurrencyCode _selectedFrom = (CurrencyCode)selectFrom.SelectedItem;
            CurrencyCode _selectedTo = (CurrencyCode)selectTo.SelectedItem;
           
            double b=_service.GetConvertTo(_selectedFrom.Code, _selectedTo.Code, ad,Int32.Parse(input.Text));
            result.Text = b.ToString();
        }
    }
}
