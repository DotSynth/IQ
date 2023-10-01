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

namespace IQ.Views.BranchViews.Pages.Purchases.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPurchaseOverlay : Page
    {

        public static string? CurrentInvoiceID;
        public static string? CurrentModelID;
        public static string? CurrentBrandID;
        public static string? CurrentAddOns;
        public static int? CurrentQuantityBought;
        public static Decimal? CurrentBuyingPrice;
        public static string? CurrentPurchasedFrom;
        public static string? CurrentSupplierContactInfo;
        bool IsCompleted;

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddPurchaseOverlay()
        {
            this.InitializeComponent();
        }

        private async void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentInvoiceID = InvoiceTextBox.Text;
            CurrentModelID = ModelIDAutoSuggestBox.Text;
            CurrentBrandID = BrandIDAutoSuggestBox.Text;
            CurrentAddOns = AddOnsTextBox.Text;
            CurrentQuantityBought = int.Parse(QuantityBoughtTextBox.Text);
            CurrentBuyingPrice = Decimal.Parse(BuyingPriceTextBox.Text);
            CurrentPurchasedFrom = PurchasedFromTextBox.Text;
            CurrentSupplierContactInfo = SupplierInfoTextBox.Text;

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
                        cmd.CommandText = "INSERT INTO BranchPurchases (InvoiceID, ModelID, BrandID, AddOns, QuantityBought, BuyingPrice, PurchasedFrom, SupplierContactInfo) VALUES (@invoiceID, @modelID, @brandID, @addOns, @qtyBought, @buyingPrice, @purchasedFrom, @supplierInfo)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("invoiceID", CurrentInvoiceID);
                        cmd.Parameters.AddWithValue("modelID", CurrentModelID);
                        cmd.Parameters.AddWithValue("brandID", CurrentBrandID);
                        cmd.Parameters.AddWithValue("addOns", CurrentAddOns);
                        cmd.Parameters.AddWithValue("qtyBought", CurrentQuantityBought);
                        cmd.Parameters.AddWithValue("buyingPrice", CurrentBuyingPrice);
                        cmd.Parameters.AddWithValue("purchasedFrom", CurrentPurchasedFrom);
                        cmd.Parameters.AddWithValue("supplierInfo", CurrentSupplierContactInfo);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        await ShowCompletionAlertDialogAsync("New Purchase Row Inserted Successfully");
                    }

                    IsCompleted = await TriggerDbSubAction_PurchaseAsync(conn);

                    // Close the connection
                    conn.Close();

                }

                PurchasesPage.OverlayInstance.SetVisibility(Visibility.Collapsed);

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

        private async Task<bool> TriggerDbSubAction_PurchaseAsync(NpgsqlConnection con)
        {
            // Check if the model exists in the inventory
            using var checkModelCommand = new NpgsqlCommand("SELECT COUNT(*) FROM BranchInventory WHERE ModelID = @modelID", con);
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
                alertDialog.XamlRoot = InventoryOverlayGrid.XamlRoot;

                // Show the ContentDialog and get the result
                ContentDialogResult result = await alertDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    // Insert the model into the inventory
                    using var insertModelCommand = new NpgsqlCommand(@"
            INSERT INTO BranchInventory (ModelID, BrandID, AddOns, QuantityInStock, UnitPrice)
            VALUES (@modelID, @brandID, @addOns, @quantityBought, @buyingPrice)", con);

                    insertModelCommand.Parameters.AddWithValue("modelID", CurrentModelID!);
                    insertModelCommand.Parameters.AddWithValue("brandID", CurrentBrandID!);
                    insertModelCommand.Parameters.AddWithValue("addOns", CurrentAddOns!);
                    insertModelCommand.Parameters.AddWithValue("quantityBought", CurrentQuantityBought!);
                    insertModelCommand.Parameters.AddWithValue("buyingPrice", CurrentBuyingPrice!);

                    insertModelCommand.ExecuteNonQuery();

                    isCompleted = true;
                    return isCompleted;
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
                // Model exists in the inventory, update the quantityInStock
                using var updateModelCommand = new NpgsqlCommand(@"
        UPDATE BranchInventory
        SET QuantityInStock = QuantityInStock + @quantityBought
        WHERE ModelID = @modelID", con);

                updateModelCommand.Parameters.AddWithValue("modelID", CurrentModelID!);
                updateModelCommand.Parameters.AddWithValue("quantityBought", CurrentQuantityBought!);

                updateModelCommand.ExecuteNonQuery();

                isCompleted = false;
                return isCompleted;
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
            alertDialog.XamlRoot = InventoryOverlayGrid.XamlRoot;

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
