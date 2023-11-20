using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Helpers.FileOperations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IQ.Views.BranchViews.Pages.TransferInwards.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTransferInwardOverlay : Page
    {
        public static string? CurrentTransferID;
        public static string? CurrentModelID;
        public static string? CurrentBrandID;
        public static string? CurrentAddOns;
        public static int? CurrentQuantityTransferred;
        public static string? CurrentTransferredFrom;
        public static string? CurrentSignedBy;
        public static Decimal? CurrentTransferredProductPrice;
        bool IsCompleted;
        public static DateTime? CurrentDate;

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddTransferInwardOverlay()
        {
            this.InitializeComponent();
            ThisDatepicker.SelectedDate = DateTime.UtcNow.Date;
            ThisDatepicker.MaxYear = DateTime.UtcNow.Date;
        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        private async void AddTInsButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTransferID = TransferIDTextBox.Text;
            CurrentModelID = ModelIDAutoSuggestBox.Text;
            CurrentBrandID = BrandIDAutoSuggestBox.Text;
            CurrentAddOns = AddOnsTextBox.Text;
            CurrentQuantityTransferred = int.Parse(QuantityTransferredTextBox.Text);
            CurrentTransferredFrom = TransferredFromTextBox.Text;
            CurrentSignedBy = SignedByTextBox.Text;
            CurrentTransferredProductPrice = Decimal.Parse(TransferredProductPriceTextBox.Text);
            CurrentDate = ThisDatepicker.Date.UtcDateTime;

            // Create a connection string
            string connString = App.ConnectionString!;

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
                        cmd.CommandText = $"INSERT INTO \"{App.Username}\".TransferInwards (TransferID, ModelID, BrandID, AddOns, QuantityTransferred, TransferredFrom, SignedBy, TransferredProductPrice, Date) VALUES (@TransferID, @modelID, @brandID, @addOns, @qtyTransferred, @transferredFrom, @signedBy, @TInProductPrice, @date)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("TransferID", CurrentTransferID);
                        cmd.Parameters.AddWithValue("modelID", CurrentModelID);
                        cmd.Parameters.AddWithValue("brandID", CurrentBrandID);
                        cmd.Parameters.AddWithValue("addOns", CurrentAddOns);
                        cmd.Parameters.AddWithValue("qtyTransferred", CurrentQuantityTransferred);
                        cmd.Parameters.AddWithValue("transferredFrom", CurrentTransferredFrom);
                        cmd.Parameters.AddWithValue("signedBy", CurrentSignedBy);
                        cmd.Parameters.AddWithValue("TInProductPrice", CurrentTransferredProductPrice);
                        cmd.Parameters.AddWithValue("date", CurrentDate);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        await ShowCompletionAlertDialogAsync("New Transfer Inwards Row Inserted Successfully");
                    }

                    IsCompleted = await TriggerDbSubAction_TransferInwardsAsync(conn);

                    // Close the connection
                    conn.Close();

                }
                Views.Loading.BTIViewModel = new BranchTInsViewModel()!;
                Views.Loading.BIViewModel = new BranchInventoryViewModel()!;
                TransferInwardsPage.OverlayInstance.SetVisibility(Visibility.Collapsed);

                if (IsCompleted == true)
                {
                    await ShowCompletionAlertDialogAsync("New Inventory Row Inserted Successfully");
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                await ShowCompletionAlertDialogAsync(error);
            }


        }

        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        private async Task<bool> TriggerDbSubAction_TransferInwardsAsync(NpgsqlConnection con)
        {

            // Check if the model exists in the inventory
            using var checkModelCommand = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{App.Username}\".Inventory WHERE ModelID = @modelID", con);
            checkModelCommand.Parameters.AddWithValue("modelID", CurrentModelID!);

            int modelCount = Convert.ToInt32(checkModelCommand.ExecuteScalar());
            bool isCompleted;

            if (modelCount == 0)
            {
                // Create a ContentDialog
                ContentDialog alertDialog = new ContentDialog
                {
                    // Set the title, content, and close button text
                    Title = "Alert",
                    Content = "This model does not exist in the inventory. Do you want to add it?",
                    PrimaryButtonText = "No",
                    SecondaryButtonText = "Add",
                    IsSecondaryButtonEnabled = true,
                    IsPrimaryButtonEnabled = true,
                };

                // Set the foreground to hex color #020066
                alertDialog.Foreground = new SolidColorBrush(Color.FromArgb(255, 2, 0, 102));

                // Set the XamlRoot property to the same as an element in the app window
                // For example, if you have a StackPanel named MyPanel in your XAML
                alertDialog.XamlRoot = TInsOverlayGrid.XamlRoot;

                // Show the ContentDialog and get the result
                ContentDialogResult result = await alertDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    try
                    {
                        // Insert the model into the inventory
                        using var insertModelCommand = new NpgsqlCommand($@"
            INSERT INTO ""{App.Username}"".Inventory (ModelID, BrandID, AddOns, QuantityInStock, UnitPrice)
            VALUES (@modelID, @brandID, @addOns, @quantityTransferred, @TInProductPrice)", con);

                        insertModelCommand.Parameters.AddWithValue("modelID", CurrentModelID!);
                        insertModelCommand.Parameters.AddWithValue("brandID", CurrentBrandID!);
                        insertModelCommand.Parameters.AddWithValue("addOns", CurrentAddOns!);
                        insertModelCommand.Parameters.AddWithValue("quantityTransferred", CurrentQuantityTransferred!);
                        insertModelCommand.Parameters.AddWithValue("TInProductPrice", CurrentTransferredProductPrice!);

                        insertModelCommand.ExecuteNonQuery();

                        isCompleted = true;
                        Views.Loading.BIViewModel = new BranchInventoryViewModel()!;
                        return isCompleted;
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        await ShowCompletionAlertDialogAsync(error);
                        isCompleted = true;
                        return isCompleted;
                    }
                }
                else
                {
                    alertDialog.Visibility = Visibility.Collapsed;
                    isCompleted = false;
                    return isCompleted;
                }
            }
            else
            {
                try
                {
                    // Model exists in the inventory, update the quantityInStock
                    using var updateModelCommand = new NpgsqlCommand($@"
        UPDATE ""{App.Username}"".Inventory
        SET QuantityInStock = QuantityInStock + @quantityTransferred
        WHERE ModelID = @modelID", con);

                    updateModelCommand.Parameters.AddWithValue("modelID", CurrentModelID!);
                    updateModelCommand.Parameters.AddWithValue("quantityTransferred", CurrentQuantityTransferred!);

                    updateModelCommand.ExecuteNonQuery();

                    isCompleted = false;
                    Views.Loading.BIViewModel = new BranchInventoryViewModel()!;
                    return isCompleted;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    await ShowCompletionAlertDialogAsync(error);
                    isCompleted = true;
                    return isCompleted;
                }
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
            alertDialog.XamlRoot = TInsOverlayGrid.XamlRoot;

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
                    string searchResultBrand = await DatabaseExtensions.QueryBrandNameFromDatabase(userQuery);
                    string searchResultAddons = await DatabaseExtensions.QueryAddonsFromDatabase(userQuery);

                    // Display the searchResults on your SalesPage or in a DataGrid
                    BrandIDAutoSuggestBox.Text = searchResultBrand;
                    AddOnsTextBox.Text = searchResultAddons;
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
