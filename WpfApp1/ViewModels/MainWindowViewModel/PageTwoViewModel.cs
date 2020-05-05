using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Services;
using WpfApp1.ViewModels.DirectoryViewModel;

namespace WpfApp1.ViewModels.MainWindowViewModel
{
    public class PageTwoViewModel : BaseViewModel
    {
        private string _rootPath;
        private ObservableCollection<DirectoryItemViewModel> _items;
        private DirectoryItemViewModel _selectedItem;

        private bool _isInProgress;
        private long _mergeProgress;

        private RelayCommand _selectFolder;
        private RelayCommand _startMergingVideos;
        private RelayCommand _onSelectedItemChange;
        
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

        public bool IsNotInProgress => !_isInProgress;
        public bool IsInProgress
        {
            get => _isInProgress;
            set
            {
                _isInProgress = value;
                OnPropertyChanged(nameof(IsInProgress));
            }
        }

        public long MergeProgress
        {
            get => _mergeProgress;
            set
            {
                _mergeProgress = value;
                OnPropertyChanged(nameof(MergeProgress));
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

        public RelayCommand StartMergingVideos
        {
            get
            {
                return _startMergingVideos ??= new RelayCommand(async _ =>
                {
                    IsInProgress = true;
                    await Task.Run(MergingVideos);
                    MergeProgress = 100;
                    DialogService.ShowMessage("Video merging successfully ended");
                    IsInProgress = false;
                    MergeProgress = 0;
                    Items = new ObservableCollection<DirectoryItemViewModel>(
                        DirectoryStructureService.GetDirectoryContent(RootPath)
                            .Select(item => new DirectoryItemViewModel(item.FullPath, item.Type))
                    );
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

        public override string ToString()
        {
            return "PageTwoViewModel: \n" 
                   + $"  Path:   {_rootPath}\n"
                   + $"  SelectedItem: {_selectedItem}\n";
        }

        public void MergingVideos()
        {
            var directoryInfo = new DirectoryInfo(RootPath);
            var directories = directoryInfo.GetDirectories();
            var totalCount = directories.Select(item => item.GetFiles().Count(item => item.Name.EndsWith("h264"))).Sum();

            var i = 1;
            foreach (var dir in directories)
            {
                var files = dir.GetFiles().Where(item => item.Name.EndsWith("h264")).ToList();
                if (files.Count > 1)
                {
                    using var fs = new FileStream($@"{dir.FullName}\mainFile.h264", FileMode.Create);
                    foreach (var fileInfo in files)
                    {
                        if (!fileInfo.Name.EndsWith("h264"))
                        {
                            continue;
                        }
                        using var fileRead = fileInfo.OpenRead();
                        var buffer = new byte[fileRead.Length];
                                
                        fileRead.Read(buffer);
                        fs.Write(buffer);
                        fileRead.Close();
                        // fileInfo.Delete();

                        MergeProgress = i * 100 / totalCount;
                        i++;
                    }
                    fs.Close();
                }
            }
        }
    }
}