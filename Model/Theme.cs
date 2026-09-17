using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ActiBoard.Model
{
    public class Theme : INotifyPropertyChanged
    {
        private bool _isActive;

        public string Id { get; set; } =
            Guid.NewGuid().ToString();

        public string Name { get; set; } =
            string.Empty;

        private string _backgroundImagePath = string.Empty;

        public string BackgroundImagePath
        {
            get => _backgroundImagePath;
            set
            {
                if (_backgroundImagePath == value)
                    return;

                _backgroundImagePath = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                    return;

                _isActive = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}