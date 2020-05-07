using WpfApp1.ViewModels;

namespace WpfApp1.Components.ConfirmDialog
{
    public class ConfirmDialogViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        
        public string YesButtonContent { get; set; }
        
        public string NoButtonContent { get; set; }

        public bool OnlyYes { get; set; }
    }
}