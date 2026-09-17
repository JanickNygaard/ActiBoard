using ActiBoard.Model;
using ActiBoard.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ActiBoard.View.Controls
{
    public partial class LocationEditorPopup : UserControl
    {
        public event KeyboardRequestedEventHandler? KeyboardRequested;

        private string? _selectedImagePath;

        public LocationEditorPopup()
        {
            InitializeComponent();
        }

        private void ChooseImageButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Vælg lokationsbillede",
                Filter = "Billedfiler|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            if (DataContext is not LocationViewModel viewModel)
            {
                return;
            }

            _selectedImagePath = dialog.FileName;

            // Viser det valgte billede med det samme i editoren.
            viewModel.LocationEditorImagePath = dialog.FileName;
        }

        private void SaveLocationButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is not LocationViewModel viewModel)
            {
                return;
            }

            string locationName =
                viewModel.LocationEditorName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(locationName))
            {
                MessageBox.Show("Du skal indtaste et navn.");
                return;
            }

            bool isEditing = viewModel.SelectedLocation is not null;

            if (!isEditing &&
                string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                MessageBox.Show("Du skal vælge et billede.");
                return;
            }

            try
            {
                string imagePath =
                    viewModel.LocationEditorImagePath ?? string.Empty;

                // Der kopieres kun et nyt billede, hvis brugeren
                // faktisk har valgt et nyt.
                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    imagePath =
                        CopyImageToAppFolder(_selectedImagePath);
                }

                if (isEditing)
                {
                    viewModel.UpdateSelectedLocation(
                        locationName,
                        imagePath);
                }
                else
                {
                    var location = new Location
                    {
                        Name = locationName,
                        LocationImagePath = imagePath,
                        IsOpen = false,
                        CurrentActivity = null,
                        ResponsibleEmployee = null
                    };

                    viewModel.AddLocation(location);
                }

                ClearForm();

                viewModel
                    .BackToLocationManagerCommand
                    .Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lokationen kunne ikke gemmes:\n{ex.Message}");
            }
        }

        private void ClearForm()
        {
            _selectedImagePath = null;

            if (DataContext is not LocationViewModel viewModel)
            {
                return;
            }

            viewModel.SelectedLocation = null;
            viewModel.LocationEditorName = string.Empty;
            viewModel.LocationEditorImagePath = string.Empty;
        }

        private static string CopyImageToAppFolder(
            string sourcePath)
        {
            string imageDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "Locations");

            Directory.CreateDirectory(imageDirectory);

            string extension = Path.GetExtension(sourcePath);

            string safeFileName =
                $"{Guid.NewGuid()}{extension}";

            string destinationPath = Path.Combine(
                imageDirectory,
                safeFileName);

            File.Copy(
                sourcePath,
                destinationPath,
                overwrite: false);

            return destinationPath;
        }
    }
}