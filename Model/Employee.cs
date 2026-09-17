using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace ActiBoard.Model
{
    public class Employee : INotifyPropertyChanged
    {
        private string _name = "";
        private string _employeeImagePath = "";

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

        public string EmployeeImagePath
        {
            get => _employeeImagePath;
            set
            {
                if (_employeeImagePath == value)
                    return;

                _employeeImagePath = value;
                OnPropertyChanged(nameof(EmployeeImagePath));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
