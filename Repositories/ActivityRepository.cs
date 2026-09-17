using ActiBoard.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace ActiBoard.Repositories
{
    public class ActivityRepository
    {
        private readonly string _filePath;
        private readonly List<Activity> _activities;

        public ActivityRepository(string themeId)
        {
            string themeDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "Themes",
                themeId);

            Directory.CreateDirectory(themeDirectory);


            _filePath = Path.Combine(
                themeDirectory,
                "activities.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            _activities = LoadFromFile();
        }

        public List<Activity> GetActivities()
        {
            return _activities;
        }

        public Activity? GetByName(string name)
        {
            return _activities.FirstOrDefault(
                activity => activity.Name == name);
        }

        public void Add(Activity activity)
        {
            _activities.Add(activity);
            SaveToFile();
        }
        public void Save()
        {
            SaveToFile();
        }

        private List<Activity> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Activity>();
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Activity>();
            }

            return JsonSerializer.Deserialize<List<Activity>>(json, _jsonOptions)
                   ?? new List<Activity>();
        }

        private void SaveToFile()
        {
            string? directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(_activities, _jsonOptions);

            File.WriteAllText(_filePath, json);
        }

        public List<Activity> GetByLocationName(string locationName)
        {
            return _activities
                .Where(activity =>
                    activity.LocationNames != null &&
                    activity.LocationNames.Contains(locationName))
            .ToList();
        }

        public void Remove(Activity activity)
        {
            _activities.Remove(activity);
            SaveToFile();
        }

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}
