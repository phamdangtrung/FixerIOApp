using GUI.Network.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI.UI.MVVM.View
{
    /// <summary>
    /// Interaction logic for SystemConfigurationView.xaml
    /// </summary>
    public partial class SystemConfigurationView : UserControl
    {
        public SystemConfigurationView()
        {
            InitializeComponent();
        }
        private void SavePassword(object sender, RoutedEventArgs e)
        {
            ConfigSystem.Password = inputPass.Text;
            MessageBox.Show(ConfigSystem.Password);
        }
        private void SaveUsername(object sender, RoutedEventArgs e)
        {
            ConfigSystem.Username= inputUser.Text;
        }
        private void SaveDataBase(object sender, RoutedEventArgs e)
        {
            ConfigSystem.Database = inputPass.Text;
        }
        private void SaveDataSource(object sender, RoutedEventArgs e)
        {
            ConfigSystem.DataSource = inputSource.Text;
        }
        private void SaveCountryCode(object sender, RoutedEventArgs e)
        {
            ConfigSystem.defaultCountryCode = inputCountryCode.Text;
        }
    }
}
