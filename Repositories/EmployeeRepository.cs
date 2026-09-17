using ActiBoard.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ActiBoard.Repositories
{
    public class EmployeeRepository
    {
        private readonly string _filePath;
        private readonly List<Employee> _employees;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public EmployeeRepository()
        {
            _filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "employees.json");

            _employees = LoadFromFile();
        }

        public List<Employee> GetEmployees()
        {
            return _employees;
        }

        public Employee? GetByName(string name)
        {
            return _employees.FirstOrDefault(
                employee => employee.Name == name);
        }

        public void Add(Employee employee)
        {
            _employees.Add(employee);
            SaveToFile();
        }

        public void Remove(Employee employee)
        {
            _employees.Remove(employee);
            SaveToFile();
        }

        public void Save()
        {
            SaveToFile();
        }

        private List<Employee> LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Employee>();
            }

            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Employee>();
            }

            return JsonSerializer.Deserialize<List<Employee>>(
                       json,
                       _jsonOptions)
                   ?? new List<Employee>();
        }

        private void SaveToFile()
        {
            string? directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(
                _employees,
                _jsonOptions);

            File.WriteAllText(_filePath, json);
        }
    }
}
