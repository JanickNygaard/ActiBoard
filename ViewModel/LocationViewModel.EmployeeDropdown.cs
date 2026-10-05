using ActiBoard.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace ActiBoard.ViewModel
{
    public partial class LocationViewModel
    {
        public static Employee EmptyEmployeeOption { get; } = new Employee { Name = "" };
        public ObservableCollection<Employee> EmployeeDropdownOptions { get; } = new();

        private void InitializeEmployeeDropdown()
        {
            EmployeeDropdownOptions.Add(EmptyEmployeeOption);
            foreach (var employee in Employees)
                EmployeeDropdownOptions.Add(employee);

            Employees.CollectionChanged += (_, _) =>
            {
                // Keep existing items in place so adding staff does not clear selections.
                foreach (var employee in EmployeeDropdownOptions
                    .Where(employee => employee != EmptyEmployeeOption && !Employees.Contains(employee))
                    .ToList())
                    EmployeeDropdownOptions.Remove(employee);

                foreach (var employee in Employees)
                    if (!EmployeeDropdownOptions.Contains(employee))
                        EmployeeDropdownOptions.Add(employee);
            };
        }
    }
}