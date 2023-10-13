using IQ.Helpers.FileOperations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.Inventory.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddInventoryOverlay : Page
    {
        public string? CurrentModelID;
        public string? CurrentBrandID;
        public string? CurrentAddOns;
        public int? CurrentQuantityInStock;
        public Decimal? CurrentUnitPrice;
        public Decimal? CurrentTotalWorth;

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddInventoryOverlay()
        {
            this.InitializeComponent();
        }

        private void AddInventoryButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentModelID = ModelIDTextBox.Text;
            CurrentBrandID = BrandIDTextBox.Text;
            CurrentAddOns = AddOnsTextBox.Text;
            CurrentQuantityInStock = int.Parse(QuantityInStockTextBox.Text);
            CurrentUnitPrice = Decimal.Parse(UnitPriceTextBox.Text);

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
                        cmd.CommandText = $"INSERT INTO \"{App.Username}\".Inventory (ModelID, BrandID, AddOns, QuantityInStock, UnitPrice) VALUES (@modelID, @brandID, @addOns, @quantityInStock, @UnitPrice)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("modelID", CurrentModelID);
                        cmd.Parameters.AddWithValue("brandID", CurrentBrandID);
                        cmd.Parameters.AddWithValue("addOns", CurrentAddOns);
                        cmd.Parameters.AddWithValue("quantityInStock", CurrentQuantityInStock);
                        cmd.Parameters.AddWithValue("UnitPrice", CurrentUnitPrice);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        _ = ShowCompletionAlertDialogAsync("New Inventory Row Inserted Successfully");
                    }

                    // Close the connection
                    conn.Close();
                }

                BranchInventoryPage.OverlayInstance.SetVisibility(Visibility.Collapsed);

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
    }
}
