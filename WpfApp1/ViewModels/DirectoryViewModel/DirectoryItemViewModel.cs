using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp1.Enums;
using WpfApp1.Services;

namespace WpfApp1.ViewModels.DirectoryViewModel
{
    public class DirectoryItemViewModel : BaseViewModel
    {
        private ObservableCollection<DirectoryItemViewModel> _childs;
        private string _fullPath;

        public string FullPath
        {
            get => _fullPath;
            set
            {
                _fullPath = value;
                OnPropertyChanged(nameof(FullPath));
            }
        }
        public DirectoryItemType Type { get; set; }

        public bool IsExpanded
        {
            get { return Childs?.Count(item => item != null) > 0; }
            set
            {
                if (value)
                {
                    Expand();
                }
                else
                {
                    ClearChilds();
                }
            }
        }

        public bool CanExpand => !DirectoryItemType.File.Equals(Type);

        public ObservableCollection<DirectoryItemViewModel> Childs
        {
            get => _childs;
            set
            {
                _childs = value;
                OnPropertyChanged(nameof(Childs));
            }
        }

        public string Name => DirectoryItemType.Drive.Equals(Type) ? FullPath : DirectoryStructureService.GetFileOrFolderName(FullPath);

        public DirectoryItemViewModel(string fullPath, DirectoryItemType type)
        {
            FullPath = fullPath;
            Type = type;
            if (!DirectoryItemType.File.Equals(Type))
            {
                ClearChilds();
            }
        }


        private void ClearChilds()
        {
            Childs = new ObservableCollection<DirectoryItemViewModel>();
            if (!DirectoryItemType.File.Equals(Type))
            {
                Childs.Add(null);
            }
        }

        private void Expand()
        {
            if (DirectoryItemType.File.Equals(Type))
            {
                return;
            }

            Childs = new ObservableCollection<DirectoryItemViewModel>(
                DirectoryStructureService.GetDirectoryContent(FullPath)
                    .Select(item => new DirectoryItemViewModel(item.FullPath, item.Type))
                );
        }
        
        
        private RelayCommand _doubleClick;
        private RelayCommand _dragEnter;
        private RelayCommand _drop;
        private RelayCommand _mouseMove;
        private RelayCommand _mouseLeftButtonDown;

        public RelayCommand DragEnter
        {
            get
            {
                return _dragEnter ??= new RelayCommand(_ =>
                {
                    Console.WriteLine("DRAG ENTER");
                }, null);
            }
        }

        public RelayCommand Drop
        {
            get
            {
                return _drop ??= new RelayCommand(_ =>
                {
                    Console.WriteLine("DROP");
                }, null);
            }
        }

        public RelayCommand DoubleClick
        {
            get
            {
                return _doubleClick ??= new RelayCommand(_ =>
                {
                    Console.WriteLine("DoubLe click!");
                }, null);
            }
        }

        public RelayCommand MouseMove
        {
            get
            {
                return _mouseMove ??= new RelayCommand(_ =>
                {
                    Console.WriteLine("Mouse MOve");
                }, null);
            }
        }

        public RelayCommand MouseLeftButtonDown
        {
            get
            {
                return _mouseLeftButtonDown ??= new RelayCommand(_ =>
                {
                    Console.WriteLine("MouseLEftButtonDown");
                }, null);
            }
        }

        public override string ToString()
        {
            return "DirectoryItemViewModel: \n"
                   + $"    FullPath: {FullPath}\n";
        }
    }
}