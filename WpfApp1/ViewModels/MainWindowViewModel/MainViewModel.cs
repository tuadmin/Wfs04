using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.ViewModels.MainWindowViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private Visibility _isPage1 = Visibility.Visible;
        private Visibility _isPage2 = Visibility.Collapsed;
        private Visibility _isOptionsPage = Visibility.Collapsed;

        private RelayCommand _goToPage1;
        private RelayCommand _goToPage2;
        private RelayCommand _goToOptions;
        private RelayCommand _printDebugInfo;

        private RelayCommand _runPython;

        public PageOneViewModel PageOneViewModel { get; }
        public PageTwoViewModel PageTwoViewModel { get; }

        public MainViewModel()
        {
            PageOneViewModel = new PageOneViewModel();
            PageTwoViewModel = new PageTwoViewModel();
        }
        
        public Visibility IsPage1
        {
            get => _isPage1;
            set
            {
                _isPage1 = value;
                OnPropertyChanged(nameof(IsPage1));
            }
        }

        public Visibility IsPage2
        {
            get => _isPage2;
            set
            {
                _isPage2 = value;
                OnPropertyChanged(nameof(IsPage2));
            }
        }
        
        public Visibility IsOptionsPage
        {
            get => _isOptionsPage;
            set
            {
                _isOptionsPage = value;
                OnPropertyChanged(nameof(IsOptionsPage));
            }
        }
        

        public RelayCommand GoToPage1
        {
            get 
            { 
                return _goToPage1 ??= new RelayCommand(_ =>
                {
                    IsPage2 = Visibility.Collapsed;
                    IsOptionsPage = Visibility.Collapsed;
                    IsPage1 = Visibility.Visible;
                    
                    
                }, null); 
            }
        }

        public RelayCommand GoToPage2
        {
            get
            {
                return _goToPage2 ??= new RelayCommand(_ =>
                {
                    IsPage1 = Visibility.Collapsed;
                    IsOptionsPage = Visibility.Collapsed;
                    IsPage2 = Visibility.Visible;
                }, null);
            }
        }

        public RelayCommand GoToOptions
        {
            get
            {
                return _goToOptions ??= new RelayCommand(_ =>
                {
                    IsPage1 = Visibility.Collapsed;
                    IsPage2 = Visibility.Collapsed;
                    IsOptionsPage = Visibility.Visible;
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

        private int _testProgress;
        public int TestProgress
        {
            get => _testProgress;
            set
            {
                _testProgress = value;
                OnPropertyChanged(nameof(TestProgress));
            }
        }

        public RelayCommand RunPython
        {
            get
            {
                return _runPython ??= new RelayCommand(async _ =>
                {
                   
                    // var eventHandler = new TaskCompletionSource<bool>();
                    await Task.Run(RunScript);


                }, null);
            }
        }

        private void RunScript()
        {
            var psi = new ProcessStartInfo()
            {
                FileName = @"G:\Pitoni\detectCamNumber\venv\Scripts\python.exe",
                Arguments = @"G:\Pitoni\detectCamNumber\src\test.py --i 10",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };
            var total = 100;

            using (var process = new Process {StartInfo = psi, EnableRaisingEvents = true})
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data))
                    {
                        return;
                    }

                    if (e.Data.StartsWith("total:"))
                    {
                        total = int.Parse(e.Data.Substring(e.Data.IndexOf(':') + 1));
                    }
                    else
                    {
                        Console.WriteLine(e.Data);
                        TestProgress = int.Parse(e.Data) * 100 / total ;
                    }

                };
                process.Exited += (sender, args) =>
                {
                    Console.WriteLine("End");
                };

                process.Start();
                Console.WriteLine("start");
                process.BeginOutputReadLine();
                process.WaitForExit();
            };
        }
    }
}