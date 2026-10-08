using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;

namespace ActiBoard.Services;

public sealed record BackupEntry(string Path, DateTime Created, string Description)
{
    public string DisplayName => $"{Created:dd-MM-yyyy HH:mm:ss} — {Description}";
}

public sealed class BackupService
{
    private readonly string _root;
    public string DirectoryPath { get; }

    public BackupService(string root, string directory) { _root = root; DirectoryPath = directory; }

    public IEnumerable<BackupEntry> List() => Directory.Exists(DirectoryPath)
        ? Directory.GetFiles(DirectoryPath, "*.zip").Select(path =>
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            int separator = name.IndexOf('_');
            return new BackupEntry(path, File.GetLastWriteTime(path), separator < 0 ? "Backup" : name[(separator + 1)..]);
        }).OrderByDescending(entry => entry.Created)
        : [];

    public string Create(string description)
    {
        Directory.CreateDirectory(DirectoryPath);
        string safeName = string.Concat(description.Select(c => System.IO.Path.GetInvalidFileNameChars().Contains(c) ? '-' : c));
        string path = System.IO.Path.Combine(DirectoryPath, $"{DateTime.Now:yyyyMMdd-HHmmss-fffffff}_{safeName}.zip");
        string temporary = path + ".tmp";
        try
        {
            using (var archive = ZipFile.Open(temporary, ZipArchiveMode.Create))
                foreach (string directory in new[] { "Data", "Images" })
                {
                    string source = System.IO.Path.Combine(_root, directory);
                    if (!Directory.Exists(source)) continue;
                    foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                        archive.CreateEntryFromFile(file, System.IO.Path.GetRelativePath(_root, file).Replace('\\', '/'), CompressionLevel.Fastest);
                }
            File.Move(temporary, path);
            return path;
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    public void Restore(string path)
    {
        string staging = System.IO.Path.Combine(_root, ".restore-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(staging);
        try
        {
            using (var archive = ZipFile.OpenRead(path))
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.StartsWith("Data/", StringComparison.Ordinal) && !entry.FullName.StartsWith("Images/", StringComparison.Ordinal))
                        throw new InvalidDataException("Backuppen indeholder en ukendt mappe.");
                    string target = System.IO.Path.GetFullPath(System.IO.Path.Combine(staging, entry.FullName));
                    if (!target.StartsWith(staging + System.IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Ugyldig filsti i backup.");
                    if (string.IsNullOrEmpty(entry.Name)) continue;
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target)!);
                    entry.ExtractToFile(target);
                }
            string stagedData = System.IO.Path.Combine(staging, "Data");
            if (!File.Exists(System.IO.Path.Combine(stagedData, "themes.json"))) throw new InvalidDataException("Backuppen mangler temadata.");
            foreach (string json in Directory.GetFiles(stagedData, "*.json", SearchOption.AllDirectories))
                using (JsonDocument.Parse(File.ReadAllText(json))) { }

            // Always preserve the current state before replacing data.
            Create("Før gendannelse");
            string stagedImages = System.IO.Path.Combine(staging, "Images");
            if (Directory.Exists(stagedImages))
                foreach (string file in Directory.GetFiles(stagedImages, "*", SearchOption.AllDirectories))
                {
                    string target = System.IO.Path.Combine(_root, "Images", System.IO.Path.GetRelativePath(stagedImages, file));
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target)!);
                    File.Copy(file, target, true);
                }
            string currentData = System.IO.Path.Combine(_root, "Data"), oldData = System.IO.Path.Combine(staging, "previous-Data");
            Directory.Move(currentData, oldData);
            try { Directory.Move(stagedData, currentData); }
            catch { Directory.Move(oldData, currentData); throw; }
        }
        finally { Directory.Delete(staging, true); }
    }
}
