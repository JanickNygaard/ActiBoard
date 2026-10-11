using ActiBoard.Commands;
using ActiBoard.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ActiBoard.ViewModel;

public partial class LocationViewModel
{
    private Location? _employeePickerLocation;
    public string EmployeePickerTitle => "Vælg ansvarlig — " + _employeePickerLocation?.Name;
    public ObservableCollection<Employee> EmployeePickerOptions { get; } = new();
    public ICommand ShowEmployeePickerCommand { get; private set; } = null!;
    public ICommand ChooseLocationEmployeeCommand { get; private set; } = null!;

    private void InitializeEmployeePicker()
    {
        ShowEmployeePickerCommand = new RelayCommand(parameter =>
        {
            if (!IsUnlocked || parameter is not Location location || !Locations.Contains(location)) return;
            _employeePickerLocation = location;
            EmployeePickerOptions.Clear();
            foreach (var employee in Employees.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
                EmployeePickerOptions.Add(employee);
            OnPropertyChanged(nameof(EmployeePickerTitle));
            CurrentPopup = PopupType.EmployeePicker;
        });
        ChooseLocationEmployeeCommand = new RelayCommand(parameter =>
        {
            if (!IsUnlocked || CurrentPopup != PopupType.EmployeePicker || _employeePickerLocation is null || !Locations.Contains(_employeePickerLocation)) return;
            if (parameter is not null && parameter is not Employee) return;
            var employee = parameter as Employee;
            if (employee is not null && !Employees.Contains(employee)) return;
            _employeePickerLocation.ResponsibleEmployee = employee;
            _employeePickerLocation = null;
            CurrentPopup = PopupType.None;
        });
    }
}
