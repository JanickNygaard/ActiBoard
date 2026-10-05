using ActiBoard.Commands;
using ActiBoard.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ActiBoard.ViewModel
{
    public partial class LocationViewModel
    {
        private PopupType _galleryReturnPopup = PopupType.None;
        public ObservableCollection<GalleryImage> GalleryImages { get; } = new();
        public string GalleryTitle { get; private set; } = "Vælg billede";
        public string GalleryMessage { get; private set; } = string.Empty;
        public ICommand OpenImageGalleryCommand { get; private set; } = null!;
        public ICommand SelectGalleryImageCommand { get; private set; } = null!;
        public ICommand CloseImageGalleryCommand { get; private set; } = null!;

        private void InitializeImageGallery()
        {
            OpenImageGalleryCommand = new RelayCommand(_ => OpenImageGallery());
            SelectGalleryImageCommand = new RelayCommand(image =>
            {
                if (image is GalleryImage galleryImage)
                    SelectGalleryImage(galleryImage);
            });
            CloseImageGalleryCommand = new RelayCommand(_ =>
            {
                if (CurrentPopup == PopupType.ImageGallery)
                    CurrentPopup = _galleryReturnPopup;
            });
        }

        private void OpenImageGallery()
        {
            string category;
            string label;
            switch (CurrentPopup)
            {
                case PopupType.ActivityEditor:
                    category = "Activities"; label = "aktiviteter"; break;
                case PopupType.LocationEditor:
                    category = "Locations"; label = "lokationer"; break;
                case PopupType.EmployeeEditor:
                    category = "Employees"; label = "medarbejdere"; break;
                case PopupType.ThemeEditor:
                    category = "Themes"; label = "temaer"; break;
                default:
                    return;
            }

            _galleryReturnPopup = CurrentPopup;
            GalleryImages.Clear();
            GalleryTitle = $"Vælg billede – {label}";
            GalleryMessage = string.Empty;
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Images", "Gallery", category);
            int skipped = 0;
            try
            {
                if (Directory.Exists(directory))
                {
                    string[] extensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff" };
                    foreach (string file in Directory.EnumerateFiles(directory)
                        .Where(file => extensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                        .OrderBy(file => Path.GetFileNameWithoutExtension(file), StringComparer.CurrentCultureIgnoreCase))
                    {
                        // Validate with WPF's decoder and release the file before copying.
                        try
                        {
                            using var stream = File.OpenRead(file);
                            var decoder = BitmapDecoder.Create(stream,
                                BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            if (decoder.Frames.Count == 0) { skipped++; continue; }
                        }
                        catch { skipped++; continue; }
                        GalleryImages.Add(new GalleryImage
                        {
                            Name = Path.GetFileNameWithoutExtension(file),
                            ImagePath = file,
                            Category = category
                        });
                    }
                }
                if (GalleryImages.Count == 0)
                    GalleryMessage = "Der er endnu ingen billeder i denne kategori. Du kan stadig vælge et billede fra computeren.";
                else if (skipped > 0)
                    GalleryMessage = "Enkelte billeder kunne ikke åbnes og er derfor udeladt.";
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                GalleryMessage = "Billederne kunne ikke indlæses. Prøv igen, eller vælg et billede fra computeren.";
            }
            OnPropertyChanged(nameof(GalleryTitle));
            OnPropertyChanged(nameof(GalleryMessage));
            CurrentPopup = PopupType.ImageGallery;
        }

        private void SelectGalleryImage(GalleryImage image)
        {
            if (CurrentPopup != PopupType.ImageGallery || !GalleryImages.Contains(image))
                return;

            try
            {
                // Activity/location editors copy the current image when saving.
                // Employee/theme editors store a private copy immediately.
                string path = image.ImagePath;
                if (_galleryReturnPopup == PopupType.EmployeeEditor ||
                    _galleryReturnPopup == PopupType.ThemeEditor)
                {
                    string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "Images", image.Category);
                    Directory.CreateDirectory(directory);
                    path = Path.Combine(directory, $"{Guid.NewGuid():N}{Path.GetExtension(image.ImagePath)}");
                    File.Copy(image.ImagePath, path);
                }

                switch (_galleryReturnPopup)
                {
                    case PopupType.ActivityEditor: ActivityEditorImagePath = path; break;
                    case PopupType.LocationEditor: LocationEditorImagePath = path; break;
                    case PopupType.EmployeeEditor: EmployeeEditorImagePath = path; break;
                    case PopupType.ThemeEditor: ThemeEditorBackgroundImagePath = path; break;
                    default: return;
                }
                CurrentPopup = _galleryReturnPopup;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show($"Billedet kunne ikke vælges:\n{ex.Message}");
            }
        }
    }
}