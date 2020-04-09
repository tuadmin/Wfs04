using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace WpfApp1.Services
{
    public static class DialogService
    {
        public static void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public static bool ShowConfirmDialog(string message)
        {
            return MessageBox.Show(message, "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public static string OpenFileDialog()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                
                //TODO: delete this
                InitialDirectory = @"E:\wfs0.4 test"
            };
            return fileDialog.ShowDialog() == true ? fileDialog.FileName : "Choose file";
        }

        public static string OpenFolderDialog()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false,
                
                //TODO: delete this
                InitialDirectory = @"E:\wfs0.4 test"
            };

            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : "Choose Folder";
        }
    }
}