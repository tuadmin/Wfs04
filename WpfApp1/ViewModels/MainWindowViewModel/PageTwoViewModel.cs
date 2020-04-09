using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WpfApp1.Services;
using WpfApp1.ViewModels.DirectoryViewModel;

namespace WpfApp1.ViewModels.MainWindowViewModel
{
    public class PageTwoViewModel : BaseViewModel
    {
        private string _rootPath;
        private ObservableCollection<DirectoryItemViewModel> _items;
        private DirectoryItemViewModel _selectedItem;

        private RelayCommand _selectFolder;
        private RelayCommand _compileVideos;
        private RelayCommand _onSelectedItemChange;

        private RelayCommand _testPython;

        public string TEST = "E:\\wfs0.4 test\\videosTest2\\signature1.h264";
        
        public string RootPath
        {
            get => _rootPath;
            set
            {
                _rootPath = value;
                OnPropertyChanged(nameof(RootPath));
            }
        }

        public ObservableCollection<DirectoryItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public DirectoryItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public RelayCommand SelectFolder
        {
            get
            {
                return _selectFolder ??= new RelayCommand(_ =>
                {
                    RootPath = DialogService.OpenFolderDialog();
                    Items = new ObservableCollection<DirectoryItemViewModel>(
                        DirectoryStructureService.GetDirectoryContent(RootPath)
                            .Select(item => new DirectoryItemViewModel(item.FullPath, item.Type))
                        );
                }, null);
            }
        }


        public RelayCommand OnSelectedItemChange
        {
            get { 
                return _onSelectedItemChange ??= new RelayCommand(item =>
                {
                    SelectedItem = item as DirectoryItemViewModel;
                }, null); 
            }
        }

        public RelayCommand CompileVideos
        {
            get
            {
                return _compileVideos ??= new RelayCommand(_ =>
                {
                    var directoryInfo = new DirectoryInfo(RootPath);
                    var directories = directoryInfo.GetDirectories();
                    foreach (var dir in directories)
                    {
                        var files = dir.GetFiles();
                        if (files.Length > 1)
                        {
                            var fs = new FileStream($@"{dir.FullName}\mainFile.h264", FileMode.Create);
                            foreach (var fileInfo in files)
                            {
                                var fileRead = fileInfo.OpenRead();
                                var buffer = new byte[fileRead.Length];
                                
                                fileRead.Read(buffer);
                                fs.Write(buffer);
                                fileRead.Close();
                                // fileInfo.Delete();
                            }
                            fs.Close();
                        }
                    }
                    Console.WriteLine("Video Compile End");
                }, _ =>
                {
                    if (RootPath == null)
                    {
                        return false;
                    }
                    var directoryInfo = new DirectoryInfo(RootPath);
                    return directoryInfo.Exists && directoryInfo.GetDirectories().Length > 0;
                });
            }
        }

        public RelayCommand TestPython
        {
            get
            {
                return _testPython ??= new RelayCommand(_ =>
                {
                    var scriptPath = @"G:\Pitoni\detectCamNumber\src\main.py";
                    var psi = new ProcessStartInfo
                    {
                        FileName = @"G:\Pitoni\detectCamNumber\venv\Scripts\python.exe",
                        Arguments = $"{scriptPath} --i \"{RootPath}\" --o G:\\Pitoni\\detectCamNumber\\output\\result.txt",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };

                    using var process = Process.Start(psi);
                    using var reader = process.StandardOutput;
                    
                    var errors = process.StandardError.ReadToEnd();
                    
                    var result = reader.ReadToEnd();
                    
                    Console.WriteLine($"errors: {errors}");
                    Console.WriteLine($"result: {result}");
                }, _ => {
                    if (RootPath == null)
                    {
                        return false;
                    }
                    var directoryInfo = new DirectoryInfo(RootPath);
                    return directoryInfo.Exists && directoryInfo.GetDirectories().Length > 0;
                });
            }
        }

        public override string ToString()
        {
            return "PageTwoViewModel: \n" 
                   + $"  Path:   {_rootPath}\n"
                   + $"  SelectedItem: {_selectedItem}\n";
        }
    }
}