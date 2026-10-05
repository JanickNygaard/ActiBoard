using ActiBoard.Model;
using System.ComponentModel;

namespace ActiBoard.ViewModel
{
    public class ActivityLocationOption : INotifyPropertyChanged
    {
        public Location Location { get; }
        private bool _isSelected;

        public ActivityLocationOption(Location location)
        {
            Location = location;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}