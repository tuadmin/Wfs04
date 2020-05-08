using System;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace WpfApp1.ViewModels.MainWindowViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private bool _isPage1 = true;
        private bool _isPage2;
        private bool _isOptionsPage;
        private bool _isDarkTheme = true;

        private RelayCommand _goToPage1;
        private RelayCommand _goToPage2;
        private RelayCommand _goToOptions;
        private RelayCommand _printDebugInfo;
        private RelayCommand _changeTheme;

        public PageOneViewModel PageOneViewModel { get; }
        public PageTwoViewModel PageTwoViewModel { get; }

        public MainViewModel()
        {
            PageOneViewModel = new PageOneViewModel();
            PageTwoViewModel = new PageTwoViewModel();
        }
        
        public bool IsPage1
        {
            get => _isPage1;
            set
            {
                _isPage1 = value;
                OnPropertyChanged(nameof(IsPage1));
            }
        }

        public bool IsPage2
        {
            get => _isPage2;
            set
            {
                _isPage2 = value;
                OnPropertyChanged(nameof(IsPage2));
            }
        }
        
        public bool IsOptionsPage
        {
            get => _isOptionsPage;
            set
            {
                _isOptionsPage = value;
                OnPropertyChanged(nameof(IsOptionsPage));
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }

        public RelayCommand GoToPage1
        {
            get 
            { 
                return _goToPage1 ??= new RelayCommand(_ =>
                {
                    IsPage2 = false;
                    IsOptionsPage = false;
                    IsPage1 = true;
                    
                    
                }, null); 
            }
        }

        public RelayCommand GoToPage2
        {
            get
            {
                return _goToPage2 ??= new RelayCommand(_ =>
                {
                    IsPage1 = false;
                    IsOptionsPage = false;
                    IsPage2 = true;
                }, null);
            }
        }

        public RelayCommand GoToOptions
        {
            get
            {
                return _goToOptions ??= new RelayCommand(_ =>
                {
                    IsPage1 = false;
                    IsPage2 = false;
                    IsOptionsPage = true;
                }, null);
            }
        }

        public RelayCommand PrintDebugInfo
        {
            get
            {
                return _printDebugInfo ??=
                    new RelayCommand(_ =>
                    {
                        Console.WriteLine(PageOneViewModel.ToString());
                        Console.WriteLine(PageTwoViewModel.ToString());
                    }, null);
            }
        }

        public RelayCommand ChangeTheme
        {
            get
            {
                return _changeTheme ??= new RelayCommand(_ =>
                {
                    var paletteHelper = new PaletteHelper();
                    var theme = paletteHelper.GetTheme();

                    theme.SetBaseTheme(_isDarkTheme ? Theme.Dark : Theme.Light);
                    
                    paletteHelper.SetTheme(theme);
                }, null);
            }
        }
    }
}