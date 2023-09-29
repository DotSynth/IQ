using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.Sales;
using Npgsql;
using System.Threading.Tasks;
using Windows.UI;
using CommunityToolkit.WinUI.UI.Animations;

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

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddPurchaseOverlay()
        {
            this.InitializeComponent();
        }

        private void AddPurchaseButton_Click(object sender, RoutedEventArgs e)
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
                        cmd.Parameters.AddWithValue("qtySold", CurrentQuantityBought);
                        cmd.Parameters.AddWithValue("buyingPrice", CurrentBuyingPrice);
                        cmd.Parameters.AddWithValue("purchasedFrom", CurrentPurchasedFrom);
                        cmd.Parameters.AddWithValue("supplierInfo", CurrentSupplierContactInfo);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        _ = ShowCompletionAlertDialogAsync("New Purchase Row Inserted Successfully");
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

        private async Task<bool> TriggerDbSubAction_PurchaseAsync()
        {
            var ConnectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;
            var con = new NpgsqlConnection(
                            connectionString: ConnectionString);
            con.Open();
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
                    PrimaryButtonText = "Add",
                    SecondaryButtonText = "No",
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

                if (result == ContentDialogResult.Primary)
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

                isCompleted = true;
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
    }
}
