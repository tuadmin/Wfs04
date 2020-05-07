using WpfApp1.ViewModels;

namespace WpfApp1.Components.ConfirmDialog
{
    public class ConfirmDialogViewModel : BaseViewModel
    {
        private bool _onlyNo;
        public string Title { get; set; }
        public string Message { get; set; }
        
        public string YesButtonContent { get; set; }
        
        public string NoButtonContent { get; set; }

        public bool OnlyNo
        {
            get => !_onlyNo;
            set
            {
                _onlyNo = value;
                OnPropertyChanged(nameof(OnlyNo));
            }
        }
    }
}