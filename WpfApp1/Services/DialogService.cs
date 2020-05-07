using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfApp1.Components.ConfirmDialog;


namespace WpfApp1.Services
{
    public static class DialogService
    {
        private static readonly string DEFAULT_TITLE = "Предупреждение";
        private static readonly string DEFAULT_YES_CONTENT = "Применить";
        private static readonly string DEFAULT_NO_CONTENT = "Отменить";
        private static string DEFAULT_ONLY_NO_BUTTON_CONTENT = "Закрыть";
        private static readonly bool DEFAULT_ONLY_NO = false;
        
        public static async Task<bool> ShowConfirmDialog(string message)
        {
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel
                {
                    Message = message, 
                    Title = DEFAULT_TITLE,
                    OnlyNo = DEFAULT_ONLY_NO,
                    YesButtonContent = DEFAULT_YES_CONTENT,
                    NoButtonContent = DEFAULT_NO_CONTENT
                }
            };

            var result = await DialogHost.Show(view, "RootDialog");

            return (bool) result;
        }
        
        public static async Task<bool> ShowConfirmDialog(string message, string title)
        {
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel
                {
                    Message = message, 
                    Title = title,
                    OnlyNo = DEFAULT_ONLY_NO,
                    YesButtonContent = DEFAULT_YES_CONTENT,
                    NoButtonContent = DEFAULT_NO_CONTENT
                }
            };

            var result = await DialogHost.Show(view, "RootDialog");

            return (bool) result;
        }
        
        public static async Task<bool> ShowConfirmDialog(string message, string title, bool onlyNo)
        {
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel
                {
                    Message = message, 
                    Title = title,
                    OnlyNo = onlyNo,
                    YesButtonContent = DEFAULT_YES_CONTENT,
                    NoButtonContent = onlyNo ? DEFAULT_ONLY_NO_BUTTON_CONTENT : DEFAULT_NO_CONTENT
                }
            };

            var result = await DialogHost.Show(view, "RootDialog");

            return (bool) result;
        }
        public static async Task<bool> ShowConfirmDialog(string message, string title, bool onlyNo, string yesContent)
        {
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel
                {
                    Message = message, 
                    Title = title,
                    OnlyNo = onlyNo,
                    YesButtonContent = yesContent,
                    NoButtonContent = DEFAULT_NO_CONTENT
                }
            };

            var result = await DialogHost.Show(view, "RootDialog");

            return (bool) result;
        }
        
        public static async Task<bool> ShowConfirmDialog(string message, string title, bool onlyNo, string yesContent, string noContent)
        {
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel
                {
                    Message = message, 
                    Title = title,
                    OnlyNo = onlyNo,
                    YesButtonContent = yesContent,
                    NoButtonContent = noContent
                }
            };

            var result = await DialogHost.Show(view, "RootDialog");

            return (bool) result;
        }
        
        public static async Task<bool> ShowConfirmDialog(string message, string title, string yesContent, string noContent)
        {
            var view = new ConfirmDialog
            {
                DataContext = new ConfirmDialogViewModel
                {
                    Message = message, 
                    Title = title,
                    OnlyNo = DEFAULT_ONLY_NO,
                    YesButtonContent = yesContent,
                    NoButtonContent = noContent
                }
            };

            var result = await DialogHost.Show(view, "RootDialog");

            return (bool) result;
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
            return fileDialog.ShowDialog() == true ? fileDialog.FileName : null;
        }
        
        public static string OpenFileDialog(string filterString)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = filterString
            };
            return fileDialog.ShowDialog() == true ? fileDialog.FileName : null;
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

            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : null;
        }
    }
}