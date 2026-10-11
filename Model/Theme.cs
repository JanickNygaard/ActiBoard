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
        private string _backgroundColor = "";
        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value) return;
                _backgroundColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitleForeground));
            }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        public string TitleForeground
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BackgroundColor))
                    return Id is "halloween" or "juleklippedag" ? "#FFF8EF" : "#172536";
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(BackgroundColor);
                return (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) < 145 ? "#FFF8EF" : "#172536";
            }
        }

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
