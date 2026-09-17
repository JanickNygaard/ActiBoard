using ActiBoard.Commands;
using ActiBoard.Model;
using ActiBoard.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Input;
using System.IO;
using System;
using System.Linq;


namespace ActiBoard.ViewModel
{
    public class LocationViewModel : INotifyPropertyChanged
    {
        private ActivityRepository _activityRepository;
        private readonly EmployeeRepository _employeeRepository;
        private LocationRepository _locationRepository;
        private readonly ThemeRepository _themeRepository;

        public ObservableCollection<Location> Locations { get; }
        public ObservableCollection<Employee> Employees { get; }
        public ObservableCollection<Activity> Activities { get; }
        public ObservableCollection<Theme> Themes { get; }

        private bool _isUnlocked;

        public event PropertyChangedEventHandler? PropertyChanged;

        private Activity? _selectedActivity;
        private string _activityEditorName = "";
        private string _activityEditorImagePath = "";

        public Activity? SelectedActivity
        {
            get => _selectedActivity;
            set
            {
                if (_selectedActivity == value)
                    return;

                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));

                PrepareActivityEditor();
            }
        }

        public string ActivityEditorName
        {
            get => _activityEditorName;
            set
            {
                if (_activityEditorName == value)
                    return;

                _activityEditorName = value;
                OnPropertyChanged(nameof(ActivityEditorName));
            }
        }

        public string ActivityEditorImagePath
        {
            get => _activityEditorImagePath;
            set
            {
                if (_activityEditorImagePath == value)
                    return;

                _activityEditorImagePath = value;
                OnPropertyChanged(nameof(ActivityEditorImagePath));
            }
        }

        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                _isUnlocked = value;
                OnPropertyChanged(nameof(IsUnlocked));
            }
        }

        private bool _isUnlockPopupVisible;
        public bool IsUnlockPopupVisible
        {
            get => _isUnlockPopupVisible;
            set
            {
                if (_isUnlockPopupVisible != value)
                {
                    _isUnlockPopupVisible = value;
                    OnPropertyChanged(nameof(IsUnlockPopupVisible));
                }
            }
        }

        private bool _editorGym1;
        private bool _editorGym2;
        private bool _editorKrea;
        private bool _editorByggeren;

        public bool EditorGym1
        {
            get => _editorGym1;
            set
            {
                _editorGym1 = value;
                OnPropertyChanged(nameof(EditorGym1));
            }
        }

        public bool EditorGym2
        {
            get => _editorGym2;
            set
            {
                _editorGym2 = value;
                OnPropertyChanged(nameof(EditorGym2));
            }
        }

        public bool EditorKrea
        {
            get => _editorKrea;
            set
            {
                _editorKrea = value;
                OnPropertyChanged(nameof(EditorKrea));
            }
        }

        public bool EditorByggeren
        {
            get => _editorByggeren;
            set
            {
                _editorByggeren = value;
                OnPropertyChanged(nameof(EditorByggeren));
            }
        }

        private PopupType _currentPopup = PopupType.None;

        public PopupType CurrentPopup
        {
            get => _currentPopup;
            set
            {
                if (_currentPopup == value)
                    return;

                _currentPopup = value;
                OnPropertyChanged(nameof(CurrentPopup));
                OnPropertyChanged(nameof(IsPopupOpen));
            }
        }

        private Activity? _activityPendingDeletion;

        public Activity? ActivityPendingDeletion
        {
            get => _activityPendingDeletion;
            set
            {
                if (_activityPendingDeletion == value)
                    return;

                _activityPendingDeletion = value;
                OnPropertyChanged(nameof(ConfirmationTitle));
                OnPropertyChanged(nameof(ConfirmationMessage));
            }
        }

        private Employee? _selectedEmployee;

        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (_selectedEmployee == value)
                    return;

                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        private string _employeeEditorName = "";
        private string _employeeEditorImagePath = "";

        public string EmployeeEditorName
        {
            get => _employeeEditorName;
            set
            {
                if (_employeeEditorName == value)
                    return;

                _employeeEditorName = value;
                OnPropertyChanged(nameof(EmployeeEditorName));
            }
        }

        public string EmployeeEditorImagePath
        {
            get => _employeeEditorImagePath;
            set
            {
                if (_employeeEditorImagePath == value)
                    return;

                _employeeEditorImagePath = value;
                OnPropertyChanged(nameof(EmployeeEditorImagePath));
            }
        }

        private Employee? _employeePendingDeletion;

        public Employee? EmployeePendingDeletion
        {
            get => _employeePendingDeletion;
            set
            {
                if (_employeePendingDeletion == value)
                    return;

                _employeePendingDeletion = value;
                OnPropertyChanged(nameof(EmployeePendingDeletion));
                OnPropertyChanged(nameof(ConfirmationTitle));
                OnPropertyChanged(nameof(ConfirmationMessage));
            }
        }

        private Theme? _activeTheme;

        public Theme? ActiveTheme
        {
            get => _activeTheme;
            set
            {
                if (_activeTheme == value)
                    return;

                _activeTheme = value;
                OnPropertyChanged(nameof(ActiveTheme));
            }
        }

        private Location? _locationPendingDeletion;

        public Location? LocationPendingDeletion
        {
            get => _locationPendingDeletion;
            set
            {
                if (_locationPendingDeletion == value)
                    return;

                _locationPendingDeletion = value;

                OnPropertyChanged(nameof(LocationPendingDeletion));
                OnPropertyChanged(nameof(ConfirmationTitle));
                OnPropertyChanged(nameof(ConfirmationMessage));
            }
        }

        private Theme? _themePendingDeletion;

        public Theme? ThemePendingDeletion
        {
            get => _themePendingDeletion;
            set
            {
                if (_themePendingDeletion == value)
                    return;

                _themePendingDeletion = value;

                OnPropertyChanged(nameof(ThemePendingDeletion));
                OnPropertyChanged(nameof(ConfirmationTitle));
                OnPropertyChanged(nameof(ConfirmationMessage));
            }
        }

        private Location? _selectedLocation;
        private string _locationEditorName = "";
        private string _locationEditorImagePath = "";

        public Location? SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (_selectedLocation == value)
                    return;

                _selectedLocation = value;
                OnPropertyChanged(nameof(SelectedLocation));

                PrepareLocationEditor();
            }
        }

        public string LocationEditorName
        {
            get => _locationEditorName;
            set
            {
                if (_locationEditorName == value)
                    return;

                _locationEditorName = value;
                OnPropertyChanged(nameof(LocationEditorName));
            }
        }

        public string LocationEditorImagePath
        {
            get => _locationEditorImagePath;
            set
            {
                if (_locationEditorImagePath == value)
                    return;

                _locationEditorImagePath = value;
                OnPropertyChanged(nameof(LocationEditorImagePath));
            }
        }

        private Theme? _selectedTheme;
        private string _themeEditorName = "";
        private string _themeEditorBackgroundImagePath = "";

        public Theme? SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme == value)
                    return;

                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));

                PrepareThemeEditor();
            }
        }

        public string ThemeEditorName
        {
            get => _themeEditorName;
            set
            {
                if (_themeEditorName == value)
                    return;

                _themeEditorName = value;
                OnPropertyChanged(nameof(ThemeEditorName));
            }
        }

        public string ThemeEditorBackgroundImagePath
        {
            get => _themeEditorBackgroundImagePath;
            set
            {
                if (_themeEditorBackgroundImagePath == value)
                    return;

                _themeEditorBackgroundImagePath = value;
                OnPropertyChanged(nameof(ThemeEditorBackgroundImagePath));
            }
        }

        public string ConfirmationTitle
        {
            get
            {
                if (ActivityPendingDeletion is not null)
                    return "Slet aktivitet?";

                if (EmployeePendingDeletion is not null)
                    return "Slet medarbejder?";

                if (LocationPendingDeletion is not null)
                    return "Slet lokation?";

                if (ThemePendingDeletion is not null)
                    return "Slet tema?";

                return "Bekræft";
            }
        }

        public string ConfirmationMessage
        {
            get
            {
                if (ActivityPendingDeletion is not null)
                {
                    return $"Er du sikker på, at du vil slette " +
                           $"\"{ActivityPendingDeletion.Name}\"?";
                }

                if (EmployeePendingDeletion is not null)
                {
                    return $"Er du sikker på, at du vil slette " +
                           $"\"{EmployeePendingDeletion.Name}\"?";
                }

                if (LocationPendingDeletion is not null)
                {
                    return $"Er du sikker på, at du vil slette " +
                           $"\"{LocationPendingDeletion.Name}\"?";
                }

                if (ThemePendingDeletion is not null)
                {
                    return $"Er du sikker på, at du vil slette " +
                           $"\"{ThemePendingDeletion.Name}\"?";
                }

                return "";
            }
        }

        public int DashboardColumns
        {
            get
            {
                if (Locations.Count <= 4)
                    return 2;

                return 3;
            }
        }


        public bool IsPopupOpen => CurrentPopup != PopupType.None;

        public ICommand ToggleLocationCommand { get; }
        public ICommand UnlockCommand { get; }
        public ICommand LockCommand { get; }
        public ICommand ShowUnlockPopupCommand { get; }
        public ICommand CancelUnlockCommand { get; }
        public ICommand ShowActivityEditorPopupCommand { get; }
        public ICommand ShowAddEmployeePopupCommand { get; }
        public ICommand ShowAddLocationPopupCommand { get; }
        public ICommand ShowSettingsPopupCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand ShowAdministrationPopupCommand { get; }
        public ICommand ShowActivityManagerPopupCommand { get; }
        public ICommand BackToActivityManagerCommand { get; }
        public ICommand BackToAdministrationCommand { get; }
        public ICommand ShowDeleteActivityConfirmationCommand { get; }
        public ICommand ConfirmDeleteCommand { get; }
        public ICommand CancelConfirmationCommand { get; }
        public ICommand ShowEmployeeManagerPopupCommand { get; }
        public ICommand ShowEmployeeEditorPopupCommand { get; }
        public ICommand BackToEmployeeManagerCommand { get; }
        public ICommand SaveEmployeeCommand { get; }
        public ICommand ShowDeleteEmployeeConfirmationCommand { get; }
        public ICommand SelectEmployeeImageCommand { get; }
        public ICommand ShowLocationManagerPopupCommand { get; }
        public ICommand ShowLocationEditorPopupCommand { get; }
        public ICommand BackToLocationManagerCommand { get; }
        public ICommand ShowDeleteLocationConfirmationCommand { get; }
        public ICommand ShowThemeManagerPopupCommand { get; }
        public ICommand ShowThemeEditorPopupCommand { get; }
        public ICommand BackToThemeManagerCommand { get; }
        public ICommand SaveThemeCommand { get; }
        public ICommand ShowDeleteThemeConfirmationCommand { get; }
        public ICommand ActivateThemeCommand { get; }
        public ICommand SelectThemeBackgroundCommand { get; }
        public ICommand RemoveThemeBackgroundCommand { get; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LocationViewModel()
        {
            // Temaerne skal indlæses først
            _themeRepository = new ThemeRepository();

            Themes = new ObservableCollection<Theme>(
                _themeRepository.GetThemes());

            // Find det aktive tema
            ActiveTheme =
                Themes.FirstOrDefault(theme => theme.IsActive)
                ?? Themes.FirstOrDefault(theme => theme.Id == "default")
                ?? Themes.FirstOrDefault();

            // Hvis intet tema tidligere var aktivt,
            // gør vi fallback-temaet aktivt
            if (ActiveTheme is not null &&
                !ActiveTheme.IsActive)
            {
                ActiveTheme.IsActive = true;
                _themeRepository.Save();
            }

            // Medarbejdere er fælles for alle temaer
            _employeeRepository = new EmployeeRepository();

            // Find ID'et på det aktive tema
            string activeThemeId =
                ActiveTheme?.Id ?? "default";

            // Aktiviteter indlæses fra det aktive temas mappe
            _activityRepository =
                new ActivityRepository(activeThemeId);

            // Lokationer indlæses fra det aktive temas mappe
            _locationRepository =
                new LocationRepository(
                    activeThemeId,
                    _activityRepository,
                    _employeeRepository);

            // Fyld collections
            Locations = new ObservableCollection<Location>(
                _locationRepository.GetLocations());

            Employees = new ObservableCollection<Employee>(
                _employeeRepository.GetEmployees());

            Activities = new ObservableCollection<Activity>(
                _activityRepository.GetActivities());

            ToggleLocationCommand = new RelayCommand(ToggleLocation);

            UnlockCommand = new RelayCommand(Unlock);
            LockCommand = new RelayCommand(Lock);

            BackToActivityManagerCommand =
                new RelayCommand(_ =>
                {
                    SelectedActivity = null;
                    CurrentPopup = PopupType.ActivityManager;
                });

            BackToAdministrationCommand =
                new RelayCommand(_ =>
                {
                    CurrentPopup = PopupType.Administration;
                });

            ShowUnlockPopupCommand =
                new RelayCommand(_ => CurrentPopup = PopupType.Unlock);

            ShowActivityEditorPopupCommand =
                new RelayCommand(OpenActivityEditor);

            ShowLocationManagerPopupCommand =
                new RelayCommand(_ =>
                {
                    SelectedLocation = null;
                    CurrentPopup = PopupType.LocationManager;
                });

            ShowLocationEditorPopupCommand =
                new RelayCommand(OpenLocationEditor);

            BackToLocationManagerCommand =
                new RelayCommand(_ =>
                {
                    ClearLocationEditor();
                    CurrentPopup = PopupType.LocationManager;
                });

            ShowSettingsPopupCommand =
                new RelayCommand(_ => CurrentPopup = PopupType.Settings);

            ClosePopupCommand =
                new RelayCommand(_ => CurrentPopup = PopupType.None);

            ShowAdministrationPopupCommand =
                new RelayCommand(_ => CurrentPopup = PopupType.Administration);

            ShowActivityManagerPopupCommand =
                new RelayCommand(_ => CurrentPopup = PopupType.ActivityManager);

            ShowDeleteActivityConfirmationCommand =
                new RelayCommand(ShowDeleteActivityConfirmation);

            CancelConfirmationCommand =
                new RelayCommand(_ => CancelConfirmation());

            ShowEmployeeManagerPopupCommand =
                new RelayCommand(_ => CurrentPopup = PopupType.EmployeeManager);

            ShowEmployeeEditorPopupCommand =
                new RelayCommand(OpenEmployeeEditor);

            BackToEmployeeManagerCommand =
                new RelayCommand(_ =>
                {
                    SelectedEmployee = null;
                    EmployeeEditorName = "";
                    EmployeeEditorImagePath = "";
                    CurrentPopup = PopupType.EmployeeManager;
                });

            SaveEmployeeCommand =
                new RelayCommand(_ => SaveEmployee());

            ShowDeleteEmployeeConfirmationCommand =
                new RelayCommand(ShowDeleteEmployeeConfirmation);

            ConfirmDeleteCommand =
                new RelayCommand(_ => ConfirmDelete());

            SelectEmployeeImageCommand =
                new RelayCommand(_ => SelectEmployeeImage());

            ShowDeleteLocationConfirmationCommand =
                new RelayCommand(ShowDeleteLocationConfirmation);

            ShowThemeManagerPopupCommand =
                new RelayCommand(_ =>
                {
                    SelectedTheme = null;
                    CurrentPopup = PopupType.ThemeManager;
                });

            ShowThemeEditorPopupCommand =
                new RelayCommand(OpenThemeEditor);

            BackToThemeManagerCommand =
                new RelayCommand(_ =>
                {
                    SelectedTheme = null;
                    ThemeEditorName = "";

                    CurrentPopup = PopupType.ThemeManager;
                });

            SaveThemeCommand =
                new RelayCommand(_ => SaveTheme());

            ShowDeleteThemeConfirmationCommand =
                new RelayCommand(ShowDeleteThemeConfirmation);

            ActivateThemeCommand =
                new RelayCommand(ActivateTheme);

            SelectThemeBackgroundCommand =
                new RelayCommand(_ => SelectThemeBackground());

            RemoveThemeBackgroundCommand =
                new RelayCommand(_ => RemoveThemeBackground());
        }

        private void Unlock(object parameter)
        {
            if (parameter is string code && code == "1828")
            {
                IsUnlocked = true;
                CurrentPopup = PopupType.None;
            }
        }

        private void Lock(object parameter)
        {
            IsUnlocked = false;
        }

        private void ToggleLocation(object parameter)
        {
            if (parameter is Location location)
            {
                location.IsOpen = !location.IsOpen;
            }
        }
        public void AddActivity(Activity activity,bool addToGym1,bool addToGym2,bool addToKrea,bool addToByggeren)
        {
            _activityRepository.Add(activity);
            Activities.Add(activity);

            foreach (Location location in Locations)
            {
                bool shouldAdd =
                    location.Name == "Gymnastiksal 1" && addToGym1 ||
                    location.Name == "Gymnastiksal 2" && addToGym2 ||
                    location.Name == "Kreaværksted" && addToKrea ||
                    location.Name == "Byggeren" && addToByggeren;

                if (shouldAdd)
                {
                    location.AvailableActivities.Add(activity);
                }
            }
        }

        private void CancelUnlock(object parameter)
        {
            IsUnlockPopupVisible = false;
        }

        private void OpenActivityEditor(object? parameter)
        {
            SelectedActivity = parameter as Activity;

            CurrentPopup = PopupType.ActivityEditor;

        }

        private void PrepareActivityEditor()
        {
            if (SelectedActivity is null)
            {
                ActivityEditorName = "";
                ActivityEditorImagePath = "";

                EditorGym1 = false;
                EditorGym2 = false;
                EditorKrea = false;
                EditorByggeren = false;

                return;
            }

            ActivityEditorName = SelectedActivity.Name;
            ActivityEditorImagePath = SelectedActivity.ActivityImagePath;

            EditorGym1 =
                SelectedActivity.LocationNames.Contains("Gymnastiksal 1");

            EditorGym2 =
                SelectedActivity.LocationNames.Contains("Gymnastiksal 2");

            EditorKrea =
                SelectedActivity.LocationNames.Contains("Kreaværksted");

            EditorByggeren =
                SelectedActivity.LocationNames.Contains("Byggeren");
        }

        public void UpdateSelectedActivity(
            string name,
            string imagePath,
            List<string> locationNames)
        {
            if (SelectedActivity is null)
                return;

            SelectedActivity.Name = name;
            SelectedActivity.ActivityImagePath = imagePath;
            SelectedActivity.LocationNames = locationNames;

            foreach (Location location in Locations)
            {
                bool shouldContainActivity =
                    locationNames.Contains(location.Name);

                bool currentlyContainsActivity =
                    location.AvailableActivities.Contains(SelectedActivity);

                if (shouldContainActivity && !currentlyContainsActivity)
                {
                    location.AvailableActivities.Add(SelectedActivity);
                }
                else if (!shouldContainActivity && currentlyContainsActivity)
                {
                    location.AvailableActivities.Remove(SelectedActivity);

                    if (location.CurrentActivity == SelectedActivity)
                    {
                        location.CurrentActivity = null;
                    }
                }
            }

            _activityRepository.Save();
        }

        private void DeleteActivity(Activity activity)
        {
            foreach (Location location in Locations)
            {
                if (location.CurrentActivity == activity)
                {
                    location.CurrentActivity = null;
                }

                location.AvailableActivities.Remove(activity);
            }

            Activities.Remove(activity);
            _activityRepository.Remove(activity);

            if (!string.IsNullOrWhiteSpace(activity.ActivityImagePath) &&
                Path.IsPathRooted(activity.ActivityImagePath) &&
                File.Exists(activity.ActivityImagePath))
            {
                try
                {
                    File.Delete(activity.ActivityImagePath);
                }
                catch
                {
                    // Aktiviteten slettes stadig, selv hvis billedfilen er låst.
                }
            }
        }

        private void ShowDeleteActivityConfirmation(object? parameter)
        {
            if (parameter is not Activity activity)
                return;

            ActivityPendingDeletion = activity;
            CurrentPopup = PopupType.Confirmation;
        }

        private void CancelConfirmation()
        {
            if (ActivityPendingDeletion is not null)
            {
                ActivityPendingDeletion = null;
                CurrentPopup = PopupType.ActivityManager;
                return;
            }

            if (EmployeePendingDeletion is not null)
            {
                EmployeePendingDeletion = null;
                CurrentPopup = PopupType.EmployeeManager;
                return;
            }

            if (LocationPendingDeletion is not null)
            {
                LocationPendingDeletion = null;
                CurrentPopup = PopupType.LocationManager;
                return;
            }

            if (ThemePendingDeletion is not null)
            {
                ThemePendingDeletion = null;
                CurrentPopup = PopupType.ThemeManager;
                return;
            }

            CurrentPopup = PopupType.Administration;
        }

        private void ConfirmDelete()
        {
            if (ActivityPendingDeletion is not null)
            {
                Activity activity = ActivityPendingDeletion;

                ActivityPendingDeletion = null;
                DeleteActivity(activity);

                CurrentPopup = PopupType.ActivityManager;
                return;
            }

            if (EmployeePendingDeletion is not null)
            {
                Employee employee = EmployeePendingDeletion;

                EmployeePendingDeletion = null;
                DeleteEmployee(employee);

                CurrentPopup = PopupType.EmployeeManager;
                return;
            }

            if (LocationPendingDeletion is not null)
            {
                Location location = LocationPendingDeletion;

                LocationPendingDeletion = null;
                DeleteLocation(location);

                CurrentPopup = PopupType.LocationManager;
            }

            if (ThemePendingDeletion is not null)
            {
                Theme theme = ThemePendingDeletion;

                ThemePendingDeletion = null;

                DeleteTheme(theme);

                CurrentPopup = PopupType.ThemeManager;
            }
        }

        private void OpenEmployeeEditor(object? parameter)
        {
            SelectedEmployee = parameter as Employee;

            if (SelectedEmployee is null)
            {
                EmployeeEditorName = "";
                EmployeeEditorImagePath = "";
            }
            else
            {
                EmployeeEditorName = SelectedEmployee.Name;
                EmployeeEditorImagePath = SelectedEmployee.EmployeeImagePath;
            }

            CurrentPopup = PopupType.EmployeeEditor;
        }

        private void OpenLocationEditor(object? parameter)
        {
            SelectedLocation = parameter as Location;

            CurrentPopup = PopupType.LocationEditor;
        }

        private void PrepareLocationEditor()
        {
            if (SelectedLocation is null)
            {
                LocationEditorName = "";
                LocationEditorImagePath = "";
                return;
            }

            LocationEditorName = SelectedLocation.Name;
            LocationEditorImagePath =
                SelectedLocation.LocationImagePath;
        }

        public void AddLocation(Location location)
        {
            bool nameAlreadyExists = Locations.Any(existingLocation =>
                existingLocation.Name.Equals(
                    location.Name,
                    StringComparison.OrdinalIgnoreCase));

            if (nameAlreadyExists)
            {
                throw new InvalidOperationException(
                    "Der findes allerede en lokation med dette navn.");
            }

            _locationRepository.Add(location);
            Locations.Add(location);

            OnPropertyChanged(nameof(DashboardColumns));
        }

        public void UpdateSelectedLocation(
            string newName,
            string newImagePath)
        {
            if (SelectedLocation is null)
                return;

            string oldName = SelectedLocation.Name;

            bool nameAlreadyExists = Locations.Any(location =>
                location != SelectedLocation &&
                location.Name.Equals(
                    newName,
                    StringComparison.OrdinalIgnoreCase));

            if (nameAlreadyExists)
            {
                throw new InvalidOperationException(
                    "Der findes allerede en lokation med dette navn.");
            }

            SelectedLocation.Name = newName;
            SelectedLocation.LocationImagePath = newImagePath;

            if (!oldName.Equals(
                    newName,
                    StringComparison.Ordinal))
            {
                UpdateActivityLocationNames(
                    oldName,
                    newName);
            }

            _locationRepository.Save();

            OnPropertyChanged(nameof(Locations));
        }

        private void UpdateActivityLocationNames(
            string oldLocationName,
            string newLocationName)
        {
            bool activitiesWereChanged = false;

            foreach (Activity activity in Activities)
            {
                if (activity.LocationNames is null)
                    continue;

                int locationIndex =
                    activity.LocationNames.FindIndex(name =>
                        name.Equals(
                            oldLocationName,
                            StringComparison.Ordinal));

                if (locationIndex < 0)
                    continue;

                activity.LocationNames[locationIndex] =
                    newLocationName;

                activitiesWereChanged = true;
            }

            if (activitiesWereChanged)
            {
                _activityRepository.Save();
            }
        }

        private void ClearLocationEditor()
        {
            SelectedLocation = null;
            LocationEditorName = "";
            LocationEditorImagePath = "";
        }

        private void SaveEmployee()
        {
            string name = EmployeeEditorName.Trim();
            string imagePath = EmployeeEditorImagePath.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return;

            if (SelectedEmployee is null)
            {
                Employee newEmployee = new Employee
                {
                    Name = name,
                    EmployeeImagePath = imagePath
                };

                Employees.Add(newEmployee);
                _employeeRepository.Add(newEmployee);
            }
            else
            {
                SelectedEmployee.Name = name;
                SelectedEmployee.EmployeeImagePath = imagePath;

                _employeeRepository.Save();
            }

            SelectedEmployee = null;
            EmployeeEditorName = "";
            EmployeeEditorImagePath = "";

            CurrentPopup = PopupType.EmployeeManager;
        }

            private void ShowDeleteEmployeeConfirmation(object? parameter)
        {
            if (parameter is not Employee employee)
                return;

            ActivityPendingDeletion = null;
            EmployeePendingDeletion = employee;

            CurrentPopup = PopupType.Confirmation;
        }

        private void DeleteEmployee(Employee employee)
        {
            foreach (Location location in Locations)
            {
                if (location.ResponsibleEmployee == employee)
                {
                    location.ResponsibleEmployee = null;
                }
            }

            Employees.Remove(employee);
            _employeeRepository.Remove(employee);

            if (!string.IsNullOrWhiteSpace(employee.EmployeeImagePath) &&
                Path.IsPathRooted(employee.EmployeeImagePath) &&
                File.Exists(employee.EmployeeImagePath))
            {
                try
                {
                    File.Delete(employee.EmployeeImagePath);
                }
                catch
                {
                    // Medarbejderen slettes stadig, hvis billedfilen er låst.
                }
            }
        }

        private void SelectEmployeeImage()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Vælg medarbejderbillede",
                Filter =
                    "Billedfiler (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            string employeeImageDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "Employees");

            Directory.CreateDirectory(employeeImageDirectory);

            string extension = Path.GetExtension(dialog.FileName);

            string safeName = string.IsNullOrWhiteSpace(EmployeeEditorName)
                ? "employee"
                : MakeSafeFileName(EmployeeEditorName);

            string fileName =
                $"{safeName}_{Guid.NewGuid():N}{extension}";

            string destinationPath = Path.Combine(
                employeeImageDirectory,
                fileName);

            File.Copy(
                dialog.FileName,
                destinationPath,
                overwrite: false);

            EmployeeEditorImagePath = destinationPath;
        }

        private static string MakeSafeFileName(string value)
        {
            char[] invalidCharacters = Path.GetInvalidFileNameChars();

            foreach (char invalidCharacter in invalidCharacters)
            {
                value = value.Replace(invalidCharacter, '_');
            }

            return value.Trim();
        }

        private void ShowDeleteLocationConfirmation(object? parameter)
        {
            if (parameter is not Location location)
                return;

            ActivityPendingDeletion = null;
            EmployeePendingDeletion = null;
            LocationPendingDeletion = location;

            CurrentPopup = PopupType.Confirmation;
        }

        private void DeleteLocation(Location location)
        {
            string locationName = location.Name;

            bool activitiesWereChanged = false;

            foreach (Activity activity in Activities)
            {
                if (activity.LocationNames is null)
                    continue;

                bool removed =
                    activity.LocationNames.RemoveAll(name =>
                        name.Equals(
                            locationName,
                            StringComparison.Ordinal)) > 0;

                if (removed)
                {
                    activitiesWereChanged = true;
                }
            }

            if (activitiesWereChanged)
            {
                _activityRepository.Save();
            }

            Locations.Remove(location);
            _locationRepository.Remove(location);

            OnPropertyChanged(nameof(DashboardColumns));

            if (!string.IsNullOrWhiteSpace(location.LocationImagePath) &&
                Path.IsPathRooted(location.LocationImagePath) &&
                File.Exists(location.LocationImagePath))
            {
                try
                {
                    File.Delete(location.LocationImagePath);
                }
                catch
                {
                    // Lokationen slettes stadig, hvis billedfilen er låst.
                }
            }
        }

        private void OpenThemeEditor(object? parameter)
        {
            SelectedTheme = parameter as Theme;

            CurrentPopup = PopupType.ThemeEditor;
        }

        private void PrepareThemeEditor()
        {
            if (SelectedTheme is null)
            {
                ThemeEditorName = "";
                ThemeEditorBackgroundImagePath = "";
                return;
            }

            ThemeEditorName = SelectedTheme.Name;
            ThemeEditorBackgroundImagePath = 
                SelectedTheme.BackgroundImagePath;
        }

        private void SaveTheme()
        {
            string name =
                ThemeEditorName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Du skal indtaste et navn.");
                return;
            }

            bool duplicateName = Themes.Any(theme =>
                theme != SelectedTheme &&
                theme.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicateName)
            {
                MessageBox.Show(
                    "Der findes allerede et tema med dette navn.");

                return;
            }

            if (SelectedTheme is null)
            {
                Theme newTheme = new Theme
                {
                    Name = name,
                    BackgroundImagePath = ThemeEditorBackgroundImagePath
                };

                _themeRepository.Add(newTheme);
                Themes.Add(newTheme);
            }
            else
            {
                SelectedTheme.Name = name;
                SelectedTheme.BackgroundImagePath =
                    ThemeEditorBackgroundImagePath;

                _themeRepository.Save();

                // Theme implementerer ikke INotifyPropertyChanged endnu,
                // så vi opdaterer collectionen for at få UI'et frisk.
                int index = Themes.IndexOf(SelectedTheme);

                if (index >= 0)
                {
                    Themes[index] = SelectedTheme;
                }
            }

            SelectedTheme = null;
            ThemeEditorName = "";
            ThemeEditorBackgroundImagePath = "";

            CurrentPopup = PopupType.ThemeManager;
        }

        private void ShowDeleteThemeConfirmation(object? parameter)
        {
            if (parameter is not Theme theme)
                return;

            ActivityPendingDeletion = null;
            EmployeePendingDeletion = null;
            LocationPendingDeletion = null;

            ThemePendingDeletion = theme;

            CurrentPopup = PopupType.Confirmation;
        }

        private void DeleteTheme(Theme theme)
        {
            if (theme == ActiveTheme)
            {
                MessageBox.Show(
                    "Det aktive tema kan ikke slettes.");

                return;
            }

            Themes.Remove(theme);
            _themeRepository.Remove(theme);
        }

        private void ActivateTheme(object? parameter)
        {
            if (parameter is not Theme theme)
                return;

            foreach (Theme existingTheme in Themes)
            {
                existingTheme.IsActive = false;
            }

            theme.IsActive = true;
            ActiveTheme = theme;

            _themeRepository.Save();

            LoadThemeData(theme.Id);
        }

        private void LoadThemeData(string themeId)
        {
            _activityRepository =
                new ActivityRepository(themeId);

            _locationRepository =
                new LocationRepository(
                    themeId,
                    _activityRepository,
                    _employeeRepository);


            Activities.Clear();

            foreach (Activity activity in _activityRepository.GetActivities())
            {
                Activities.Add(activity);
            }

            Locations.Clear();

            foreach (Location location in _locationRepository.GetLocations())
            {
                Locations.Add(location);
            }

            OnPropertyChanged(nameof(DashboardColumns));
        }

        private void SelectThemeBackground()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Vælg baggrundsbillede",
                Filter =
                    "Billedfiler (*.png;*.jpg;*.jpeg;*.webp)|*.png;*.jpg;*.jpeg;*.webp",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return;

            string themeId =
                SelectedTheme?.Id
                ?? Guid.NewGuid().ToString();

            string themeImageDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "Themes",
                themeId);

            Directory.CreateDirectory(themeImageDirectory);

            string extension = Path.GetExtension(dialog.FileName);

            string destinationPath = Path.Combine(
                themeImageDirectory,
                $"background_{Guid.NewGuid():N}{extension}");

            File.Copy(
                dialog.FileName,
                destinationPath,
                overwrite: true);

            ThemeEditorBackgroundImagePath =
                destinationPath;
        }

        private void RemoveThemeBackground()
        {
            ThemeEditorBackgroundImagePath = "";
        }

    }
    
}