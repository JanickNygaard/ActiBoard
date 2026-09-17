using ActiBoard.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ActiBoard.Repositories
{
    public class ThemeRepository
    {
        private readonly string _filePath;
        private readonly List<Theme> _themes;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public ThemeRepository()
        {
            _filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "themes.json");

            _themes = LoadFromFile();
        }

        public List<Theme> GetThemes()
        {
            return _themes;
        }

        public Theme? GetById(string id)
        {
            return _themes.FirstOrDefault(
                theme => theme.Id == id);
        }

        public void Add(Theme theme)
        {
            _themes.Add(theme);
            SaveToFile();
        }

        public void Remove(Theme theme)
        {
            _themes.Remove(theme);
            SaveToFile();
        }

        public void Save()
        {
            SaveToFile();
        }

        private List<Theme> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Theme>();
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Theme>();
            }

            return JsonSerializer.Deserialize<List<Theme>>(
                       json,
                       _jsonOptions)
                   ?? new List<Theme>();
        }

        private void SaveToFile()
        {
            string? directory =
                Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(
                _themes,
                _jsonOptions);

            File.WriteAllText(_filePath, json);
        }
    }
}