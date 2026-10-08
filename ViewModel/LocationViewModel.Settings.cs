using ActiBoard.Commands;
using ActiBoard.Model;
using ActiBoard.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ActiBoard.ViewModel;

public partial class LocationViewModel
{
    private AppSettingsStore _settingsStore = null!;
    private BackupService _backupService = null!;
    private string _settingsDirectory = "";
    private DateTime _lastInteraction = DateTime.UtcNow;
    public AppSettings Settings => _settingsStore.Value;
    public ObservableCollection<BackupEntry> Backups { get; } = new();
    public BackupEntry? SelectedBackup { get; set; }
    private string _settingsMessage = "";
    public string SettingsMessage
    {
        get => _settingsMessage;
        private set { _settingsMessage = value; OnPropertyChanged(nameof(SettingsMessage)); }
    }
    public ICommand SaveSettingsCommand { get; private set; } = null!;
    public ICommand CreateBackupCommand { get; private set; } = null!;
    public ICommand RestoreBackupCommand { get; private set; } = null!;
    public ICommand CloseAllLocationsCommand { get; private set; } = null!;

    private void InitializeSettings(string? directory)
    {
        _settingsDirectory = directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ActiBoard");
        _settingsStore = new AppSettingsStore(_settingsDirectory);
        _backupService = new BackupService(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(_settingsDirectory, "Backups"));
        SaveSettingsCommand = new RelayCommand(_ => SaveSettings());
        CreateBackupCommand = new RelayCommand(_ => CreateBackup());
        RestoreBackupCommand = new RelayCommand(_ => RestoreBackup());
        CloseAllLocationsCommand = new RelayCommand(_ => CloseAllLocations());
    }

    private void OpenSettings()
    {
        if (!IsUnlocked) return;
        SettingsMessage = "";
        RefreshBackups();
        CurrentPopup = PopupType.Settings;
    }

    private void RefreshBackups()
    {
        Backups.Clear();
        foreach (var backup in _backupService.List()) Backups.Add(backup);
        SelectedBackup = Backups.FirstOrDefault();
        OnPropertyChanged(nameof(SelectedBackup));
    }

    private void SaveSettings()
    {
        if (!IsUnlocked) return;
        if (Settings.AutoLockMinutes < 1 || Settings.AutoLockMinutes > 120)
        { SettingsMessage = "Låsetiden skal være mellem 1 og 120 minutter."; return; }
        try { _settingsStore.Save(); RegisterInteraction(); SettingsMessage = "Indstillingerne er gemt."; }
        catch (Exception e) { SettingsMessage = "Kunne ikke gemme: " + e.Message; }
    }

    public bool ChangeAdministrationPin(string current, string newPin, string confirmation)
    {
        if (!IsUnlocked) return false;
        if (!_settingsStore.VerifyPin(current)) { SettingsMessage = "Den nuværende PIN-kode er forkert."; return false; }
        if (newPin.Length != 4 || newPin.Any(c => c < '0' || c > '9')) { SettingsMessage = "Den nye PIN-kode skal bestå af 4 cifre."; return false; }
        if (newPin != confirmation) { SettingsMessage = "De to nye PIN-koder er ikke ens."; return false; }
        try { _settingsStore.ChangePin(newPin); SettingsMessage = "PIN-koden er ændret."; return true; }
        catch (Exception e) { SettingsMessage = "Kunne ikke ændre PIN-koden: " + e.Message; return false; }
    }

    private void CreateBackup()
    {
        if (!IsUnlocked) return;
        try { _backupService.Create("Manuel backup"); RefreshBackups(); SettingsMessage = "Backup af alle temaer, aktiviteter, lokationer, medarbejdere og billeder er gemt."; }
        catch (Exception e) { SettingsMessage = "Kunne ikke oprette backup: " + e.Message; }
    }

    private void RestoreBackup()
    {
        if (!IsUnlocked || SelectedBackup is null) { SettingsMessage = "Vælg en backup først."; return; }
        if (MessageBox.Show("Gendan den valgte backup? Alle temaer, aktiviteter, lokationer og medarbejdere erstattes. Der gemmes først en backup af den nuværende udgave. PIN-kode og kontaktoplysninger bevares.",
            "Gendan backup", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        try
        {
            _backupService.Restore(SelectedBackup.Path);
            var replacement = new LocationViewModel(_settingsDirectory);
            if (Application.Current.MainWindow is Window window) window.DataContext = replacement;
        }
        catch (Exception e) { SettingsMessage = "Kunne ikke gendanne: " + e.Message; }
    }

    public void CloseAllLocations()
    {
        if (!IsUnlocked) return;
        foreach (var location in Locations) location.IsOpen = false;
        SettingsMessage = "Alle lokationer i det viste tema er lukket.";
    }

    private bool BackupBeforeDeletion()
    {
        if (!IsUnlocked) return false;
        try { _backupService.Create("Før sletning"); return true; }
        catch (Exception e)
        {
            MessageBox.Show("Sletningen blev ikke gennemført, fordi backup ikke kunne gemmes: " + e.Message, "Backup", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    public void RegisterInteraction() => _lastInteraction = DateTime.UtcNow;

    public void CheckAutoLock(DateTime now)
    {
        if (IsUnlocked && Settings.AutoLockEnabled && now - _lastInteraction >= TimeSpan.FromMinutes(Math.Clamp(Settings.AutoLockMinutes, 1, 120)))
            Lock(null!);
    }
}
