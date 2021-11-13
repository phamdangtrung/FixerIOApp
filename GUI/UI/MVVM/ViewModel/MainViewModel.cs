using GUI.UI.Core;

namespace GUI.UI.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand TodayIndexViewCommand { get; set; }
        public RelayCommand HistoryIndexViewCommand { get; set; }
        public RelayCommand ConverterViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public TodayIndexViewModel TodayIndexVM { get; set; }
        public HistoryIndexViewModel HistoryIndexVM { get; set; }
        public ConverterViewModel ConverterVM { get; set; }

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
            HistoryIndexVM = new();
            ConverterVM = new();

            CurrentView = HomeVM;

            HomeViewCommand = new(obj =>
            {
                CurrentView = HomeVM;
            });

            TodayIndexViewCommand = new(obj =>
            {
                CurrentView = TodayIndexVM;
            });

            HistoryIndexViewCommand = new(obj =>
            {
                CurrentView = HistoryIndexVM;
            });
            ConverterViewCommand = new(obj =>
            {
                CurrentView = ConverterVM;
            });
        }
    }
}
