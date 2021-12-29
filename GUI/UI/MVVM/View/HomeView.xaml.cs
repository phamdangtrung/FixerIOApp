using Data;
using GUI.Network.Models;
using GUI.Network.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly KeyPair _accessKeys = AccessKeys.GetInstance().Fixer;
        public HomeView()
        {
            InitializeComponent();
            
        }
    }
}
