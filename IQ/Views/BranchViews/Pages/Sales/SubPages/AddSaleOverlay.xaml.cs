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

namespace IQ.Views.BranchViews.Pages.Sales.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddSaleOverlay : Page
    {
        public string? CurrentInvoiceId;
        public string? CurrentModelID;
        public string? CurrentBrandID;
        public int? CurrentQuantitySold;
        public Decimal? CurrentSellingPrice;
        public string? CurrentSoldTo;
        public string? CurrentCustomerContactInfo;

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddSaleOverlay()
        {
            this.InitializeComponent();
        }

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentInvoiceId = InvoiceTextBox.Text;
            CurrentModelID = ModelIDAutoSuggestBox.Text;
            CurrentBrandID = BrandIDAutoSuggestBox.Text;
            CurrentQuantitySold = int.Parse(QuantitySoldTextBox.Text);
            CurrentSellingPrice = Decimal.Parse(SellingPriceTextBox.Text);
            CurrentSoldTo = SoldToTextBox.Text;
            CurrentCustomerContactInfo = CustomerInfoTextBox.Text;

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
                        cmd.CommandText = $"INSERT INTO \"{App.UserName}\".Sales (InvoiceID, ModelID, BrandID, QuantitySold, SellingPrice, SoldTo, CustomerContactInfo) VALUES (@invoiceID, @modelID, @brandID, @qtySold, @sellingPrice, @SoldTo, @customerInfo)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("invoiceID", CurrentInvoiceId);
                        cmd.Parameters.AddWithValue("modelID", CurrentModelID);
                        cmd.Parameters.AddWithValue("brandID", CurrentBrandID);
                        cmd.Parameters.AddWithValue("qtySold", CurrentQuantitySold);
                        cmd.Parameters.AddWithValue("sellingPrice", CurrentSellingPrice);
                        cmd.Parameters.AddWithValue("SoldTo", CurrentSoldTo);
                        cmd.Parameters.AddWithValue("customerInfo", CurrentCustomerContactInfo);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        _ = ShowCompletionAlertDialogAsync("New Sale Row Inserted Successfully");
                    }

                    // Close the connection
                    conn.Close();
                }

                SalesPage.OverlayInstance.SetVisibility(Visibility.Collapsed);

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
            alertDialog.XamlRoot = OverlayGrid.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

        }

        // This method sets the visibility and raises the event
        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
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
