using GUI.UI.Core;

namespace GUI.UI.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand TodayIndexViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public TodayIndexViewModel TodayIndexVM { get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeVM = new();
            TodayIndexVM = new();

            CurrentView = HomeVM;

            HomeViewCommand = new(obj =>
            {
                CurrentView = HomeVM;
            });

            TodayIndexViewCommand = new(obj =>
            {
                CurrentView = TodayIndexVM;
            });
        }
    }
}
