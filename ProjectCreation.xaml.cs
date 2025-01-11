using System;
using Microsoft.Maui.Controls;

namespace PocketSprite;

public partial class ProjectCreation : ContentPage
{
    public ProjectCreation()
    {
        InitializeComponent();
    }

    private void OnCreateClicked(object sender, EventArgs e)
    {
        // Retrieve input values
        string projectName = ProjectNameEntry.Text;
        string projectWidthText = ProjectWidthEntry.Text;
        string projectHeightText = ProjectHeightEntry.Text;
        string projectPixelSizeText = ProjectPixelSizeEntry.Text;
        string backgroundColor = ProjectBackgroundColorPicker.SelectedItem as string ?? string.Empty;

        // Input validation
        if (string.IsNullOrWhiteSpace(projectName))
        {
            DisplayAlert("Error", "Project name is required.", "OK");
            return;
        }

        if (!int.TryParse(projectWidthText, out int projectWidth) || projectWidth <= 0)
        {
            DisplayAlert("Error", "Please enter a valid project width.", "OK");
            return;
        }

        if (!int.TryParse(projectHeightText, out int projectHeight) || projectHeight <= 0)
        {
            DisplayAlert("Error", "Please enter a valid project height.", "OK");
            return;
        }

        if (!int.TryParse(projectPixelSizeText, out int projectPixelSize) || projectPixelSize <= 0)
        {
            DisplayAlert("Error", "Please enter a valid pixel size.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(backgroundColor))
        {
            DisplayAlert("Error", "Please select a background color.", "OK");
            return;
        }

        // Create the project (logic here)
        DisplayAlert("Success", 
                     $"Project '{projectName}' created with dimensions {projectWidth}x{projectHeight} and background color {backgroundColor}!", 
                     "OK");

        // Navigate or initialize your project workspace here
        // Example:
        // Navigation.PushAsync(new ProjectWorkspacePage(projectName, projectWidth, projectHeight, projectPixelSize, backgroundColor));
    }

    private new void DisplayAlert(string title, string message, string cancel)
    {
        // Platform-agnostic alert
        this.DisplayAlert(title, message, cancel);
    }
}