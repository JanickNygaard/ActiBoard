using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace ActiBoard.Model
{
    public class Activity : INotifyPropertyChanged
    {
        private string _name = "";
        private string _activityImagePath = "";

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string ActivityImagePath
        {
            get => _activityImagePath;
            set
            {
                if (_activityImagePath == value)
                    return;

                _activityImagePath = value;
                OnPropertyChanged(nameof(ActivityImagePath));
            }
        }

        public List<string> LocationNames { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
