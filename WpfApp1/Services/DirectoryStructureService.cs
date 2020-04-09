using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfApp1.Enums;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public static class DirectoryStructureService
    {
        public static List<DirectoryItem> GetDirectoryContent(string fullPath)
        {
            var items = new List<DirectoryItem>();

            var folders = Directory.GetDirectories(fullPath)
                .Select(directory => new DirectoryItem
                {
                    FullPath = directory, 
                    Type = DirectoryItemType.Folder
                }).ToList();
            
            items.AddRange(folders);

            var files = Directory.GetFiles(fullPath)
                .Select(file => new DirectoryItem
                {
                    FullPath = file,
                    Type = DirectoryItemType.File
                }).ToList();
            
            items.AddRange(files);

            return items;
        }

        public static string GetFileOrFolderName(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentException("Path cant be null, empty or whitespace");
            }

            var lastIndex = fullPath.LastIndexOf('\\');
            return lastIndex <= 0 ? fullPath : fullPath.Substring(lastIndex + 1);
        }
    }
}