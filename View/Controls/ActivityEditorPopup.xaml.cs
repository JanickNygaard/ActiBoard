using ActiBoard.Model;
using ActiBoard.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ActiBoard.View.Controls
{
    public partial class ActivityEditorPopup : UserControl
    {
        public event KeyboardRequestedEventHandler? KeyboardRequested;

        private string? _selectedImagePath;

        public ActivityEditorPopup()
        {
            InitializeComponent();
        }

        private void ChooseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Vælg aktivitetsbillede",
                Filter = "Billedfiler|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() != true)
                return;

            if (DataContext is not LocationViewModel viewModel)
                return;

            _selectedImagePath = dialog.FileName;
            viewModel.ActivityEditorImagePath = dialog.FileName;
        }

        private void SaveActivityButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not LocationViewModel viewModel)
                return;

            string activityName =
                viewModel.ActivityEditorName?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(activityName))
            {
                MessageBox.Show("Du skal indtaste et navn.");
                return;
            }

            bool addToGym1 = viewModel.EditorGym1;
            bool addToGym2 = viewModel.EditorGym2;
            bool addToKrea = viewModel.EditorKrea;
            bool addToByggeren = viewModel.EditorByggeren;

            if (!addToGym1 &&
                !addToGym2 &&
                !addToKrea &&
                !addToByggeren)
            {
                MessageBox.Show("Vælg mindst én lokation.");
                return;
            }

            bool isEditing = viewModel.SelectedActivity is not null;

            if (!isEditing && string.IsNullOrWhiteSpace(_selectedImagePath))
            {
                MessageBox.Show("Du skal vælge et billede.");
                return;
            }

            try
            {
                string imagePath = viewModel.ActivityEditorImagePath;

                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    imagePath = CopyImageToAppFolder(_selectedImagePath);
                }

                var locationNames = new List<string>();

                if (addToGym1)
                    locationNames.Add("Gymnastiksal 1");

                if (addToGym2)
                    locationNames.Add("Gymnastiksal 2");

                if (addToKrea)
                    locationNames.Add("Kreaværksted");

                if (addToByggeren)
                    locationNames.Add("Byggeren");

                // Her afgør vi, om vi redigerer eller opretter
                if (isEditing)
                {
                    viewModel.UpdateSelectedActivity(
                        activityName,
                        imagePath,
                        locationNames);
                }
                else
                {
                    var activity = new Activity
                    {
                        Name = activityName,
                        ActivityImagePath = imagePath,
                        LocationNames = locationNames
                    };

                    viewModel.AddActivity(
                        activity,
                        addToGym1,
                        addToGym2,
                        addToKrea,
                        addToByggeren);
                }

                ClearForm();
                viewModel.BackToActivityManagerCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Aktiviteten kunne ikke gemmes:\n{ex.Message}");
            }
        }

        private void ClearForm()
        {
            _selectedImagePath = null;

            if (DataContext is not LocationViewModel viewModel)
                return;

            viewModel.SelectedActivity = null;
            viewModel.ActivityEditorName = "";
            viewModel.ActivityEditorImagePath = "";

            viewModel.EditorGym1 = false;
            viewModel.EditorGym2 = false;
            viewModel.EditorKrea = false;
            viewModel.EditorByggeren = false;
        }

        private string CopyImageToAppFolder(string sourcePath)
        {
            string imageDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images",
                "Activities");

            Directory.CreateDirectory(imageDirectory);

            string extension = Path.GetExtension(sourcePath);
            string safeFileName = $"{Guid.NewGuid()}{extension}";
            string destinationPath = Path.Combine(imageDirectory, safeFileName);

            File.Copy(sourcePath, destinationPath, overwrite: false);

            return destinationPath;
        }
    }
}