using System;

namespace WpfApp1.ViewModels.VideoItemViewModel
{
    public class VideoItemViewModel : BaseViewModel
    {
        private string _name;
        private DateTime _startDateTime;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DateTime StartDateTime
        {
            get => _startDateTime;
            set
            {
                _startDateTime = value;
                OnPropertyChanged(nameof(StartDateTime));
            }
        }
    }
}