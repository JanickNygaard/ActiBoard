using ActiBoard.Commands;
using ActiBoard.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ActiBoard.ViewModel;

public partial class LocationViewModel
{
    private Location? _activityPickerLocation;
    public string ActivityPickerTitle => "Vælg aktivitet — " + _activityPickerLocation?.Name;
    public ObservableCollection<Activity> ActivityPickerOptions { get; } = new();
    public ICommand ShowActivityPickerCommand { get; private set; } = null!;
    public ICommand ChooseLocationActivityCommand { get; private set; } = null!;

    private void InitializeActivityPicker()
    {
        ShowActivityPickerCommand = new RelayCommand(parameter =>
        {
            if (!IsUnlocked || parameter is not Location location || !Locations.Contains(location)) return;
            _activityPickerLocation = location;
            ActivityPickerOptions.Clear();
            foreach (var activity in location.AvailableActivities.Where(a => !string.IsNullOrWhiteSpace(a.Name)))
                ActivityPickerOptions.Add(activity);
            OnPropertyChanged(nameof(ActivityPickerTitle));
            CurrentPopup = PopupType.ActivityPicker;
        });
        ChooseLocationActivityCommand = new RelayCommand(parameter =>
        {
            if (!IsUnlocked || CurrentPopup != PopupType.ActivityPicker || _activityPickerLocation is null || !Locations.Contains(_activityPickerLocation)) return;
            var activity = parameter as Activity;
            if (activity is not null && !_activityPickerLocation.AvailableActivities.Contains(activity)) return;
            _activityPickerLocation.CurrentActivity = activity;
            _activityPickerLocation = null;
            CurrentPopup = PopupType.None;
        });
    }
}
