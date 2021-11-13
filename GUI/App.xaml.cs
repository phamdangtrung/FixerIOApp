using GUI.Network.API;
using GUI.Network.Shared;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            ExchangeRatesService service = new ExchangeRatesService();
            //var response = await service.GetTodayRate("VND");

            var list = CommonCurrencyCodes.GetInstance().CurrencyCodes;

            base.OnStartup(e);
        }
    }
}
