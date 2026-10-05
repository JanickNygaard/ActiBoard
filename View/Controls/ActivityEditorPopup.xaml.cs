using ActiBoard.Model;
using ActiBoard.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ActiBoard.View.Controls
{
    public partial class ActivityEditorPopup : UserControl
    {
        public event KeyboardRequestedEventHandler? KeyboardRequested;



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

            var locationNames = viewModel.ActivityEditorLocations
                .Where(option => option.IsSelected)
                .Select(option => option.Location.Name)
                .ToList();

            if (locationNames.Count == 0)
            {
                MessageBox.Show("Vælg mindst én lokation.");
                return;
            }

            bool isEditing = viewModel.SelectedActivity is not null;

            if (!isEditing && string.IsNullOrWhiteSpace(viewModel.ActivityEditorImagePath))
            {
                MessageBox.Show("Du skal vælge et billede.");
                return;
            }

            try
            {
                string imagePath = viewModel.ActivityEditorImagePath;

                if (File.Exists(imagePath) && (!isEditing || imagePath != viewModel.SelectedActivity?.ActivityImagePath))
                {
                    imagePath = CopyImageToAppFolder(imagePath);
                }

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

                    viewModel.AddActivity(activity);
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


            if (DataContext is not LocationViewModel viewModel)
                return;

            viewModel.SelectedActivity = null;
            viewModel.ActivityEditorName = "";
            viewModel.ActivityEditorImagePath = "";

            foreach (var option in viewModel.ActivityEditorLocations)
                option.IsSelected = false;
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