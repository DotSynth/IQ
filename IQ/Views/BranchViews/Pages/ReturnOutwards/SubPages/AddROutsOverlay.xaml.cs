using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.FileOperations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.ReturnOutwards.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddROutsOverlay : Page
    {
        public static string? CurrentReturnID;
        public static string? CurrentModelID;
        public static string? CurrentBrandID;
        public static int? CurrentQuantityReturned;
        public static string? CurrentReturnedTo;
        public static string? CurrentReasonForReturn;
        public static string? CurrentSignedBy;

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddROutsOverlay()
        {
            this.InitializeComponent();
        }

        private void AddROutsButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentReturnID = ReturnIDTextBox.Text;
            CurrentModelID = ModelIDAutoSuggestBox.Text;
            CurrentBrandID = BrandIDAutoSuggestBox.Text;
            CurrentQuantityReturned = int.Parse(QuantityReturnedTextBox.Text);
            CurrentReturnedTo = ReturnedToTextBox.Text;
            CurrentReasonForReturn = ReasonForReturnTextBox.Text;
            CurrentSignedBy = SignedByTextBox.Text;

            // Create a connection string
            string connString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            try
            {
                // Create a connection object
                using (var conn = new NpgsqlConnection(connString))
                {
                    // Open the connection
                    conn.Open();

                    // Create a command object
                    using (var cmd = new NpgsqlCommand())
                    {
                        // Assign the connection to the command
                        cmd.Connection = conn;

                        // Write the SQL statement for inserting data
                        cmd.CommandText = "INSERT INTO BranchReturnOutwards (ReturnID, ModelID, BrandID, QuantityReturned, ReturnedTo, ReasonForReturn, SignedBy) VALUES (@ReturnID, @modelID, @brandID, @qtyReturned, @returnedTo, @reasonForReturn @signedBy)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("ReturnID", CurrentReturnID);
                        cmd.Parameters.AddWithValue("modelID", CurrentModelID);
                        cmd.Parameters.AddWithValue("brandID", CurrentBrandID);
                        cmd.Parameters.AddWithValue("qtyReturned", CurrentQuantityReturned);
                        cmd.Parameters.AddWithValue("returnedTo", CurrentReturnedTo);
                        cmd.Parameters.AddWithValue("reasonForReturn", CurrentReasonForReturn);
                        cmd.Parameters.AddWithValue("signedBy", CurrentSignedBy);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        _ = ShowCompletionAlertDialogAsync("New Return Outwards Row Inserted Successfully");
                    }

                    // Close the connection
                    conn.Close();
                }

                ReturnOutwardsPage.OverlayInstance.SetVisibility(Visibility.Collapsed);

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                _ = ShowCompletionAlertDialogAsync(error);
            }


        }

        private async Task ShowCompletionAlertDialogAsync(string alert)
        {
            // Create a ContentDialog
            ContentDialog alertDialog = new ContentDialog
            {
                // Set the title, content, and close button text
                Title = "Alert",
                Content = alert,
                CloseButtonText = "OK"
            };

            // Set the foreground to hex color #020066
            alertDialog.Foreground = new SolidColorBrush(Color.FromArgb(255, 2, 0, 102));

            // Set the XamlRoot property to the same as an element in the app window
            // For example, if you have a StackPanel named MyPanel in your XAML
            alertDialog.XamlRoot = ROutsOverlayGrid.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

        }

        // This method sets the visibility and raises the event
        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);

            Debug.WriteLine($"Visibility changed to {visibility}");
        }

        private async void ModelIDAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
                if (!string.IsNullOrWhiteSpace(sender.Text))
                {
                    // Perform a database query based on the user's queryText
                    string userQuery = sender.Text;
                    string searchResult = await DatabaseExtensions.QueryBrandNameFromDatabase(userQuery);
                    Debug.WriteLine("PopupPageVisibilityChanged called");

                    // Display the searchResults on your SalesPage or in a DataGrid
                    BrandIDAutoSuggestBox.Text = searchResult;
                }
            }
        }

        private async void ModelIDAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryInventorySuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }

        private void BrandIDAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        private async void BrandIDAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryBrandIDSuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }
    }
}
