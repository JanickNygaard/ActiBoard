using ActiBoard.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace ActiBoard.Repositories
{
    public class LocationRepository
    {
        private readonly string _filePath;
        private readonly List<Location> _locations;
        private readonly ActivityRepository _activityRepository;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public LocationRepository(
            string themeId,
            ActivityRepository activityRepository,
            EmployeeRepository employeeRepository)
        {
            _activityRepository = activityRepository;

            string themeDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "Themes",
                themeId);

            Directory.CreateDirectory(themeDirectory);

            _filePath = Path.Combine(
                themeDirectory,
                "locations.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            _locations = LoadFromFile();

            AddRuntimeData();
        }

        public List<Location> GetLocations()
        {
            return _locations;
        }

        public Location? GetByName(string name)
        {
            return _locations.FirstOrDefault(
                location => location.Name == name);
        }

        public void Add(Location location)
        {
            location.IsOpen = false;
            location.CurrentActivity = null;
            location.ResponsibleEmployee = null;

            location.AvailableActivities =
                new ObservableCollection<Activity>(
                    _activityRepository.GetByLocationName(location.Name));

            _locations.Add(location);

            SaveToFile();
        }

        public void Remove(Location location)
        {
            _locations.Remove(location);
            SaveToFile();
        }

        public void Save()
        {
            SaveToFile();
        }

        private List<Location> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Location>();
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Location>();
            }

            return JsonSerializer.Deserialize<List<Location>>(
                       json,
                       _jsonOptions)
                   ?? new List<Location>();
        }

        private void AddRuntimeData()
        {
            foreach (Location location in _locations)
            {
                location.IsOpen = false;
                location.CurrentActivity = null;
                location.ResponsibleEmployee = null;

                location.AvailableActivities =
                    new ObservableCollection<Activity>(
                        _activityRepository.GetByLocationName(location.Name));
            }
        }

        private void SaveToFile()
        {
            SaveLocations(_locations);
        }

        private void SaveLocations(List<Location> locations)
        {
            string? directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(
                locations,
                _jsonOptions);

            File.WriteAllText(_filePath, json);
        }

        private static List<Location> CreateDefaultLocations()
        {
            return new List<Location>
            {
                new()
                {
                    Name = "Gymnastiksal 1",
                    LocationImagePath = "/Images/Locations/Gym1.jpg"
                },

                new()
                {
                    Name = "Gymnastiksal 2",
                    LocationImagePath = "/Images/Locations/Gym2.jpg"
                },

                new()
                {
                    Name = "Kreaværksted",
                    LocationImagePath = "/Images/Locations/Krea.HEIC"
                },

                new()
                {
                    Name = "Byggeren",
                    LocationImagePath = "/Images/Locations/Byggeren.jpg"
                }
            };
        }
    }
}