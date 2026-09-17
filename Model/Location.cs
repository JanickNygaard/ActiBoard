using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ActiBoard.Model
{
    public class Location : INotifyPropertyChanged
    {
        private string _name = "";
        private string _locationImagePath = "";
        private bool _isOpen;
        private Employee? _responsibleEmployee;
        private Activity? _currentActivity;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public string LocationImagePath
        {
            get => _locationImagePath;
            set
            {
                if (_locationImagePath == value)
                    return;

                _locationImagePath = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public Employee? ResponsibleEmployee
        {
            get => _responsibleEmployee;
            set
            {
                if (_responsibleEmployee == value)
                    return;

                _responsibleEmployee = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public ObservableCollection<Activity> AvailableActivities
        {
            get;
            set;
        } = new ObservableCollection<Activity>();

        [JsonIgnore]
        public Activity? CurrentActivity
        {
            get => _currentActivity;
            set
            {
                if (_currentActivity == value)
                    return;

                _currentActivity = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}