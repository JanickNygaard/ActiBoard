using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace ActiBoard.Services;

public sealed class AppSettings
{
    public string PinHash { get; set; } = "";
    public string PinSalt { get; set; } = "";
    public bool AutoLockEnabled { get; set; } = true;
    public int AutoLockMinutes { get; set; } = 5;
    public string ContactName { get; set; } = "Janick Nygaard Rasmussen";
    public string ContactEmail { get; set; } = "";
    public string ContactPhone { get; set; } = "51335062";
}

public sealed class AppSettingsStore
{
    private readonly string _path;
    public AppSettings Value { get; private set; }

    public AppSettingsStore(string directory)
    {
        _path = Path.Combine(directory, "settings.json");
        Value = File.Exists(_path)
            ? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_path)) ?? throw new InvalidDataException("Indstillinger kunne ikke læses.")
            : new AppSettings();
    }

    public bool VerifyPin(string pin)
    {
        if (string.IsNullOrEmpty(Value.PinHash)) return pin == "1828";
        byte[] salt = Convert.FromBase64String(Value.PinSalt);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(pin, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hash, Convert.FromBase64String(Value.PinHash));
    }

    public void ChangePin(string pin)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        string oldHash = Value.PinHash, oldSalt = Value.PinSalt;
        Value.PinSalt = Convert.ToBase64String(salt);
        Value.PinHash = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(pin, salt, 100_000, HashAlgorithmName.SHA256, 32));
        try { Save(); }
        catch { Value.PinHash = oldHash; Value.PinSalt = oldSalt; throw; }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        string temporary = _path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(Value, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temporary, _path, true);
    }
}
