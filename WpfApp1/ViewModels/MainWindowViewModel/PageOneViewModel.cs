using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModels.MainWindowViewModel
{
    public class PageOneViewModel : BaseViewModel
    {
        //page1
        private string _pathToVideos;
        
        private PhysicalDiskItem _selectedDevice = new PhysicalDiskItem(@"\\.\PhysicalDrive3", 0);
        private string _pathToFileWithVideos;
        private bool _isWorkWithDevice = true;
        private bool _isWorkWithFile = false;
        
        private bool _isInProgress;
        private bool _isNewProject;
        
        private DateTime _startDate = new DateTime(2020, 01, 30, 4, 30, 0);
        private int _startTimeHours;
        private int _startTimeMinutes;
        private DateTime _endDate = new DateTime(2020, 01, 30, 4, 35, 0);
        private int _endTimeHours;
        private int _endTimeMinutes;
        
        private bool _isNeedTimeCheck = true;
        private string _logText;
        
        private string _projectName = "new_project";
        private List<string> _projectsList = new List<string>();
        private List<VideoItem> _videoList = new List<VideoItem>();
        private ScanInfo _scanInfo;
        
        private RelayCommand _selectFile;
        private RelayCommand _selectFolder;
        private RelayCommand _testStart;
        private RelayCommand _testScan;

        private RelayCommand _setWorkWithFile;
        private RelayCommand _setWorkWithDevice;

        private RelayCommand _selectedProjectChanged;
        private RelayCommand _createNewProject;
        private RelayCommand _loadProject;
        public List<PhysicalDiskItem> DiskList { get; }


        public PageOneViewModel()
        {
            var query = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            var driveInfos = query.Get().Cast<ManagementObject>()
                .Select(item => new PhysicalDiskItem(item["DeviceID"].ToString(), (ulong) item["Size"]))
                .ToList();
            DiskList = driveInfos;
            _logText = "";
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pathToDb = Path.Combine(directory, "Wfs04Test", "projects.sqlite");
            var isFileExists = File.Exists(pathToDb);
            if (!isFileExists)
            {
                Directory.CreateDirectory(Path.Combine(directory, "Wfs04Test"));
                File.Create(pathToDb);
            }
            else
            {
                ProjectsList = DatabaseService.GetExistingProjects();
            }
        }

        
        public string PathToVideos
        {
            get => _pathToVideos;
            set
            {
                _pathToVideos = value;
                OnPropertyChanged(nameof(PathToVideos));
            }
        }
        
        public string PathToFileWithVideos
        {
            get => _pathToFileWithVideos;
            set
            {
                _pathToFileWithVideos = value;
                OnPropertyChanged(nameof(PathToFileWithVideos));
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                var temp = new DateTime(value.Year, value.Month, value.Day, _startTimeHours, _startTimeMinutes, 0);
                if (temp <= _endDate)
                {
                    _startDate = temp;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        public string StartTimeHours
        {
            get => $"{_startDate.Hour}";
            set
            {
                int.TryParse(value, out var temp);
                _startTimeHours = temp > 23 ? _startTimeHours : temp;
                _startDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, _startTimeHours, _startTimeMinutes, 0);
                OnPropertyChanged(nameof(StartTimeHours));
            }
        }

        public string StartTimeMinutes
        {
            get => $"{_startDate.Minute}";
            set
            {
                int.TryParse(value, out var temp);
                _startTimeMinutes = temp > 59 ? _startTimeMinutes : temp;
                _startDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, _startTimeHours, _startTimeMinutes, 0);
                OnPropertyChanged(nameof(StartTimeMinutes));
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                var temp = new DateTime(value.Year, value.Month, value.Day, _endTimeHours, _endTimeMinutes, 0);
                if (temp >= _startDate)
                {
                    _endDate = temp;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        public string EndTimeHours
        {
            get => $"{_endDate.Hour}";
            set
            {
                int.TryParse(value, out var temp);
                _endTimeHours = temp > 23 ? _endTimeHours : temp;
                _endDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, _endTimeHours, _endTimeMinutes, 0);
                OnPropertyChanged(nameof(EndTimeHours));
            }
        }

        public string EndTimeMinutes
        {
            get => $"{_endDate.Minute}";
            set
            {
                int.TryParse(value, out var temp);
                _endTimeMinutes = temp > 59 ? _endTimeMinutes : temp;
                _endDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, _endTimeHours, _endTimeMinutes, 0);
                OnPropertyChanged(nameof(EndTimeMinutes));
            }
        }

        public PhysicalDiskItem SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged(nameof(SelectedDevice));
            }
        }

        public bool IsNeedTimeCheck
        {
            get => _isNeedTimeCheck;
            set
            {
                _isNeedTimeCheck = value;
                OnPropertyChanged(nameof(IsNeedTimeCheck));
            }
        }

        public bool IsWorkWithDevice
        {
            get => _isWorkWithDevice;
            set
            {
                _isWorkWithDevice = value;
                OnPropertyChanged(nameof(IsWorkWithDevice));
            }
        }

        public bool IsWorkWithFile
        {
            get => _isWorkWithFile;
            set
            {
                _isWorkWithFile = value;
                OnPropertyChanged(nameof(IsWorkWithFile));
            }
        }

        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        public List<string> ProjectsList
        {
            get => _projectsList;
            set
            {
                _projectsList = value;
                OnPropertyChanged(nameof(ProjectsList));
            }
        }

        public List<VideoItem> VideoList
        {
            get => _videoList;
            set
            {
                _videoList = value;
                OnPropertyChanged(nameof(VideoList));
            }
        }

        public ScanInfo ScanInfo
        {
            get => _scanInfo;
            set
            {
                _scanInfo = value;
                OnPropertyChanged(nameof(ScanInfo));
            }
        }

        public RelayCommand SelectedProjectChanged
        {
            get
            {
                return _selectedProjectChanged ??= new RelayCommand(item => { ProjectName = (string)item; }, null);
            }
        }

        public RelayCommand CreateNewProject
        {
            get
            {
                return _createNewProject ??= new RelayCommand(_ =>
                {
                    if (DatabaseService.IsTableExist(ProjectName))
                    {
                        if (DialogService.ShowConfirmDialog("Проект с таким именем уже существует. Создание нового проекта с таким же именем приведет к удалению старого проета. Вы уверены что хотите продолжить?"))
                        {
                            DatabaseService.DeleteProjectTable(ProjectName);
                        }
                        else
                        {
                            return;
                        }
                    }

                    _isNewProject = true;
                    DatabaseService.CreateNewProjectTable(ProjectName);
                    VideoList = new List<VideoItem>();
                    ProjectsList = DatabaseService.GetExistingProjects();
                }, _ => !string.IsNullOrWhiteSpace(ProjectName));
            }
        }

        public RelayCommand LoadProject
        {
            get
            {
                return _loadProject ??= new RelayCommand(_ =>
                {
                    LogText += "LoadStart\n";
                    if (!DatabaseService.IsTableExist(ProjectName))
                    {
                        DialogService.ShowMessage("Проекта с таким именем не существует");
                        return;
                    }

                    _isNewProject = false;
                    ScanInfo = DatabaseService.GetScanInfo(ProjectName);
                    VideoList = DatabaseService.GetDataFromProjectTable(ProjectName);
                    LogText += "LoadEnd\n";
                }, _ => !string.IsNullOrWhiteSpace(ProjectName));
            }
        }

        public RelayCommand SelectFile
        {
            get
            {
                return _selectFile ??= new RelayCommand(_ =>
                {
                    var filePath = DialogService.OpenFileDialog();
                    PathToFileWithVideos = filePath;
                    LogText += $"Selected filePath: '{filePath}'\n";
                }, null );
            }
        }

        public RelayCommand SelectFolder
        {
            get
            {
                return _selectFolder ??= new RelayCommand(obj =>
                {
                    var folderPath = DialogService.OpenFolderDialog();
                    PathToVideos = folderPath;
                    LogText += $"Selected folderPath: '{folderPath}'\n";
                }, obj => true);
            }
        }
        
        public RelayCommand TestStart
        {
            get
            {
                return _testStart ??= new RelayCommand(_ =>
                {
                    if (!DatabaseService.IsTableExist(ProjectName))
                    {
                        DialogService.ShowMessage("Проект с таким именем не найден");
                        return;
                    }
                    LogText += ToString();
                    LogText += "\nStart\n\n";

                    if (IsNeedTimeCheck)
                    {
                        if (IsWorkWithDevice)
                        {
                            Wfs04Service.ExtractVideos(ProjectName, SelectedDevice.DeviceId, PathToVideos, StartDate, EndDate);
                        }

                        if (IsWorkWithFile)
                        {
                            Wfs04Service.ExtractVideos(ProjectName, PathToFileWithVideos, PathToVideos, StartDate, EndDate);
                        }
                    }
                    else
                    {
                        if (IsWorkWithDevice)
                        {
                            Wfs04Service.ExtractVideos(ProjectName, SelectedDevice.DeviceId, PathToVideos);
                        }

                        if (IsWorkWithFile)
                        {
                            Wfs04Service.ExtractVideos(ProjectName, PathToFileWithVideos, PathToVideos);
                        }
                    }
                    LogText += "\n END!";

                }, _ => !string.IsNullOrWhiteSpace(PathToVideos) && !string.IsNullOrWhiteSpace(ProjectName) && (SelectedDevice.Size > 0 || !string.IsNullOrWhiteSpace(PathToFileWithVideos)));
            }
        }

        public RelayCommand TestScan
        {
            get
            {
                return _testScan ??= new RelayCommand(_ =>
                {
                    if (!DatabaseService.IsTableExist(ProjectName))
                    {
                        DialogService.ShowMessage("Проект с таким именем не найден");
                        return;
                    }
                    
                    LogText += "\n Start Scan\n";

                    if (IsWorkWithDevice)
                    {
                        Wfs04Service.StartScanDevice(SelectedDevice, ProjectName);
                    }
                    
                    if (IsWorkWithFile)
                    {
                        Wfs04Service.StartScanFile(PathToFileWithVideos, ProjectName);
                    }
                    ScanInfo = DatabaseService.GetScanInfo(ProjectName);
                    VideoList = DatabaseService.GetDataFromProjectTable(ProjectName);

                    LogText += "\n End Scan!";
                }, _ => SelectedDevice.Size > 0 || !string.IsNullOrEmpty(PathToFileWithVideos));
            }
        }

        public RelayCommand SetWorkWithFile
        {
            get
            {
                return _setWorkWithFile ??= new RelayCommand(_ =>
                {
                    IsWorkWithDevice = false;
                    IsWorkWithFile = true;
                }, null);
            }
        }

        public RelayCommand SetWorkWithDevice
        {
            get { return _setWorkWithDevice ??= new RelayCommand(_ =>
            {
                IsWorkWithFile = false;
                IsWorkWithDevice = true;
            }, null); }
        }


        public override string ToString()
        {
            return "\nPageOneViewModel:\n"
                   + $"  Path to Disk:   {_selectedDevice.DeviceId}\n"
                   + $"  Disk Size:   {_selectedDevice.Size} (bytes)\n"
                   + $"  Disk Size (gb):   {_selectedDevice.Size / 1073741824} (gb)\n"
                   + $"  Is Need TimeCheck:   {_isNeedTimeCheck}\n"
                   + $"  Is Work with device:   {_isWorkWithDevice}\n"
                   + $"  Path to File:   {_pathToFileWithVideos}\n"
                   + $"  Start Date:     {_startDate}\n"
                   + $"  End Date:       {_endDate}\n";
        }
    }
}