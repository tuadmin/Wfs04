using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
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
        private bool _isNeedSecondProgressBar;
        
        private DateTime _startDate = new DateTime(2020, 01, 30, 4, 30, 0);
        private int _startTimeHours = 4;
        private int _startTimeMinutes = 30;
        private DateTime _endDate = new DateTime(2020, 01, 30, 4, 35, 0);
        private int _endTimeHours = 4;
        private int _endTimeMinutes = 35;
        
        private bool _isNeedTimeCheck = true;
        private string _logText;
        
        private string _projectName;
        private List<string> _projectsList = new List<string>();
        private List<VideoItem> _videoList = new List<VideoItem>();
        private ScanInfo _scanInfo;

        private long _extractProgress;
        private long _camSortingProgress;
        private long _scanProgress;

        private string _pathToPythonExe = @"G:\Pitoni\detectCamNumber\venv\Scripts\python.exe";
        private string _pathToPythonScript = @"G:\Pitoni\detectCamNumber\src\main.py";
        private string _pathToPythonOutput = @"G:\Pitoni\detectCamNumber\output\result.txt";

        private RelayCommand _selectFile;
        private RelayCommand _selectPythonExePath;
        private RelayCommand _selectPythonScriptPath;
        private RelayCommand _selectPythonOutputPath;
        private RelayCommand _selectFolder;
        
        private RelayCommand _testStart;
        private RelayCommand _testScan;
        private RelayCommand _testProgressBar;

        private RelayCommand _setWorkWithFile;
        private RelayCommand _setWorkWithDevice;

        private RelayCommand _selectedProjectChanged;
        private RelayCommand _createNewProject;
        private RelayCommand _loadProject;
        private RelayCommand _deleteProject;
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

        public string StartTime
        {
            get => _startDate.ToString("hh:mm");
            set
            {
                var items = value.Split(':');
                int.TryParse(items[0], out var tempHours);
                int.TryParse(items[1], out var tempMinutes);
                var tempDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, tempHours, tempMinutes, 0);
                if (tempDate <= _endDate)
                {
                    _startTimeHours = tempHours;
                    _startTimeMinutes = tempMinutes;
                    _startDate = tempDate;
                }
                
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

        public string EndTime
        {
            get => _endDate.ToString("hh:mm");
            set
            {
                var items = value.Split(':');
                int.TryParse(items[0], out var tempHours);
                int.TryParse(items[1], out var tempMinutes);
                var tempDate =  new DateTime(_endDate.Year, _endDate.Month, _endDate.Day, tempHours, tempMinutes, 0);
                if (tempDate >= _startDate)
                {
                    _endTimeHours = tempHours;
                    _endTimeMinutes = tempMinutes;
                    _endDate = tempDate;
                }
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

        public bool IsNeedSecondProgressBar
        {
            get => _isNeedSecondProgressBar;
            set
            {
                _isNeedSecondProgressBar = value;
                OnPropertyChanged(nameof(IsNeedSecondProgressBar));
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

        public long ExtractProgress
        {
            get => _extractProgress;
            set
            {
                _extractProgress = value;
                OnPropertyChanged(nameof(ExtractProgress));
            }
        }

        public long CamSortingProgress
        {
            get => _camSortingProgress;
            set
            {
                _camSortingProgress = value;
                OnPropertyChanged(nameof(CamSortingProgress));
            }
        }

        public long ScanProgress
        {
            get => _scanProgress;
            set
            {
                _scanProgress = value;
                OnPropertyChanged(nameof(ScanProgress));
            }
        }

        public string PathToPythonExe
        {
            get => _pathToPythonExe;
            set
            {
                _pathToPythonExe = value;
                OnPropertyChanged(nameof(PathToPythonExe));
            }
        }

        public string PathToPythonScript
        {
            get => _pathToPythonScript;
            set
            {
                _pathToPythonScript = value;
                OnPropertyChanged(nameof(PathToPythonScript));
            }
        }

        public string PathToPythonOutput
        {
            get => _pathToPythonOutput;
            set
            {
                _pathToPythonOutput = value;
                OnPropertyChanged(nameof(PathToPythonOutput));
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

        public RelayCommand DeleteProject
        {
            get
            {
                return _deleteProject ??= new RelayCommand(_ =>
                {
                    if (!DatabaseService.IsTableExist(ProjectName)) return;
                    if (!DialogService.ShowConfirmDialog("Вы уверены что хотите удалить проект?")) return;
                    DatabaseService.DeleteProjectTable(ProjectName);
                    ProjectsList = DatabaseService.GetExistingProjects();
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

        public RelayCommand SelectPythonExePath
        {
            get
            {
                return _selectPythonExePath ??= new RelayCommand(_ =>
                {
                    var filePath = DialogService.OpenFileDialog("Executable file (*.exe)|*.exe");
                    PathToPythonExe = filePath;
                    LogText += $"Selected filePath: '{filePath}'\n";
                }, null);
            }
        }
        
        public RelayCommand SelectPythonScriptPath
        {
            get
            {
                return _selectPythonScriptPath ??= new RelayCommand(_ =>
                {
                    var filePath = DialogService.OpenFileDialog("Python script file (*.py)|*.py");
                    PathToPythonScript = filePath;
                    LogText += $"Selected filePath: '{filePath}'\n";
                }, null);
            }
        }
        
        public RelayCommand SelectPythonOutputPath
        {
            get
            {
                return _selectPythonOutputPath ??= new RelayCommand(_ =>
                {
                    var filePath = DialogService.OpenFileDialog("Text file (*.txt)|*.txt");
                    PathToPythonOutput = filePath;
                    LogText += $"Selected filePath: '{filePath}'\n";
                }, null);
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
                return _testStart ??= new RelayCommand(async _ =>
                {
                    if (!DatabaseService.IsTableExist(ProjectName))
                    {
                        DialogService.ShowMessage("Проект с таким именем не найден");
                        return;
                    }
                    
                    IsNeedSecondProgressBar = true;
                    IsInProgress = true;
                    
                    if (IsNeedTimeCheck)
                    {
                        if (IsWorkWithDevice)
                        {
                            await Task.Run(() => Wfs04Service.ExtractVideos(ProjectName, SelectedDevice.DeviceId, PathToVideos, StartDate, EndDate, this));
                        }
                    
                        if (IsWorkWithFile)
                        {
                            await Task.Run(() => Wfs04Service.ExtractVideos(ProjectName, PathToFileWithVideos, PathToVideos, StartDate, EndDate, this));
                        }
                    }
                    else
                    {
                        if (IsWorkWithDevice)
                        {
                            await Task.Run(() => Wfs04Service.ExtractVideos(ProjectName, SelectedDevice.DeviceId, PathToVideos, this));
                        }
                    
                        if (IsWorkWithFile)
                        {
                            await Task.Run(() => Wfs04Service.ExtractVideos(ProjectName, PathToFileWithVideos, PathToVideos, this));
                        }
                    }

                    ExtractProgress = 100;

                    await Task.Run(RunScript);
                    CamSortingProgress = 100;

                    DialogService.ShowMessage("Extract ended successfully.");
                    IsInProgress = false;
                    ExtractProgress = 0;
                    CamSortingProgress = 0;
                }, _ => !string.IsNullOrWhiteSpace(PathToVideos) && !string.IsNullOrWhiteSpace(ProjectName) && (SelectedDevice.Size > 0 || !string.IsNullOrWhiteSpace(PathToFileWithVideos)));
            }
        }

        public RelayCommand TestScan
        {
            get
            {
                return _testScan ??= new RelayCommand(async _ =>
                {
                    if (!DatabaseService.IsTableExist(ProjectName))
                    {
                        DialogService.ShowMessage("Проект с таким именем не найден");
                        return;
                    }

                    IsNeedSecondProgressBar = false;
                    IsInProgress = true;
                    
                    if (IsWorkWithDevice)
                    {
                        await Task.Run(() => Wfs04Service.StartScanDevice(SelectedDevice, ProjectName, this));
                    }
                    
                    if (IsWorkWithFile)
                    {
                        await Task.Run(() => Wfs04Service.StartScanFile(PathToFileWithVideos, ProjectName, this));
                    }
                    ExtractProgress = 100;
                    ScanInfo = DatabaseService.GetScanInfo(ProjectName);
                    VideoList = DatabaseService.GetDataFromProjectTable(ProjectName);

                    DialogService.ShowMessage("Scan ended successfully.");
                    IsInProgress = false;
                    ExtractProgress = 0;
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
                    SelectedDevice = new PhysicalDiskItem();
                }, null);
            }
        }

        public RelayCommand SetWorkWithDevice
        {
            get { return _setWorkWithDevice ??= new RelayCommand(_ =>
            {
                IsWorkWithFile = false;
                IsWorkWithDevice = true;
                PathToFileWithVideos = null;
            }, null); }
        }
        

        public override string ToString()
        {
            return "\nPageOneViewModel:\n"
                   + $"  Path to Disk (DeviceID):   {_selectedDevice.DeviceId}\n"
                   + $"  Disk Size:   {_selectedDevice.Size} (bytes)\n"
                   + $"  Disk Size (gb):   {_selectedDevice.Size / 1073741824} (gb)\n"
                   + $"  Is Need TimeCheck:   {_isNeedTimeCheck}\n"
                   + $"  Is Work with device:   {_isWorkWithDevice}\n"
                   + $"  Path to File with videos:   {_pathToFileWithVideos}\n"
                   + $"  Start Date:     {_startDate}\n"
                   + $"  End Date:       {_endDate}\n"
                   + $"  Path to python.exe:       {_pathToPythonExe}\n"
                   + $"  Path to python script:       {_pathToPythonScript}\n"
                   + $"  Path to python output:       {_pathToPythonOutput}\n"
                   + $"  Project name:       {_projectName}\n";
        }
        
        private void RunScript()
        {
            var psi = new ProcessStartInfo
            {
                FileName = PathToPythonExe,
                Arguments = $"\"{PathToPythonScript}\" --i \"{PathToVideos}\" --o \"{PathToPythonOutput}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };
            var total = 100;
            using var process = new Process {StartInfo = psi, EnableRaisingEvents = true};
            process.OutputDataReceived += (sender, e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                {
                    return;
                }

                if (e.Data.StartsWith("total:"))
                {
                    Console.WriteLine(e.Data);
                    total = int.Parse(e.Data.Substring(e.Data.IndexOf(':') + 1));
                }
                else
                {
                    Console.WriteLine(e.Data);
                    CamSortingProgress = int.Parse(e.Data) * 100 / total ;
                }

            };
            process.Exited += (sender, args) =>
            {
                Console.WriteLine("End");
            };

            process.Start();
            Console.WriteLine("start");
            process.BeginOutputReadLine();
            // process.WaitForExit();
            Console.WriteLine(process.StandardError.ReadToEnd());
        }
    }
}