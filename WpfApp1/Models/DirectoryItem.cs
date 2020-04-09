using System.Collections.Generic;
using WpfApp1.Enums;
using WpfApp1.Services;

namespace WpfApp1.Models
{
    public class DirectoryItem
    {
        public string FullPath { get; set; }
        public DirectoryItemType Type { get; set; }

        public string Name => Type.Equals(DirectoryItemType.Drive) ? FullPath : DirectoryStructureService.GetFileOrFolderName(FullPath);
    }
}