using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.Purchases.SubPages;
using IQ.Views.BranchViews.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using IQ.Views.WarehouseViews.Pages.Purchases.SubPages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IQ.Views.WarehouseViews.Pages.Purchases
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PurchasesPage : Page
	{
        public WHPurchasesViewModel ViewModel { get; } = new WHPurchasesViewModel();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        // Initialize OverlayInstance
        public static AddPurchase OverlayInstance = new AddPurchase();

        public PurchasesPage()
        {
            this.InitializeComponent();
            _ = LoadSuggestionsAsync();
            WarehousePurchasesDatePicker.SelectedDate = DateFilter;
            WarehousePurchasesDatePicker.MaxYear = DateTime.UtcNow.Date;
            DataContext = ViewModel;

            // Subscribe to the VisibilityChanged event of the popup page
            OverlayInstance.VisibilityChanged += PopupPageVisibilityChanged!;
        }

        private void WarehousePurchasesDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            DateFilter = WarehousePurchasesDatePicker.Date.UtcDateTime;
            RefreshPage();
        }

        public async void RefreshPage()
        {
            // Do something before the delay
            // Navigate away to a placeholder page
            Frame.Navigate(typeof(PLaceHolderPage));

            await Task.Delay(2000);
            // Continue with the next line of code after the delay
            // Navigate back to the original page to refresh it
            Frame.Navigate(typeof(PurchasesPage));
        }


        private async Task LoadSuggestionsAsync()
        {
            try
            {
                // Establish a connection to your PostgreSQL database
                using (NpgsqlConnection connection = new NpgsqlConnection(StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString))
                {
                    await connection.OpenAsync();

                    // Query the database to retrieve values from the 'columnName' column
                    using (NpgsqlCommand command = new NpgsqlCommand($"SELECT DISTINCT InvoiceID FROM \"{App.UserName}\".Purchases WHERE DATE(Date) = @time;", connection))
                    {
                        command.Parameters.AddWithValue("time", DateFilter!.Value.DateTime);
                        using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string suggestion = reader.GetString(0);
                                suggestions.Add(suggestion);
                            }
                        }
                    }
                }

                // Set the list of suggestions as the ItemsSource for the AutoSuggestBox
                WarehousePurchasesAutoSuggestBox.ItemsSource = suggestions;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                string error = ex.Message;
                // You should implement proper error handling here.
            }
        }

        private void PopupPageVisibilityChanged(object sender, EventArgs e)
        {
            // Check if the popup page's visibility is collapsed
            if (OverlayInstance.Visibility == Visibility.Collapsed)
            {
                // Trigger the RefreshPage() function
                RefreshPage();
            }
        }

        private void PurchasesAddButton_Click(object sender, RoutedEventArgs e)
        {

            // Show the popup by setting its visibility to Visible
            OverlayInstance.SetVisibility(Visibility.Visible);
            PurchaseOverlayPopUp.IsOpen = true;

        }

        private void WarehousePurchasesAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        private async void WarehousePurchasesAutoSuggestBox_QuerySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<WarehousePurchase> searchResults = await DatabaseExtensions.QueryWHPurchasesResultsFromDatabase(userQuery);

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdatePurchasesPageWithResults(searchResults);
            }
            else
            {
                UpdatePurchasesPageWithResults(ViewModel.WarehousePurchase);
            }
        }

        private void UpdatePurchasesPageWithResults(ObservableCollection<WarehousePurchase> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                WarehousePurchasesDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }



        private async void WarehousePurchasesAutoSuggestBox_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryWHPurchasesSuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }
    }
}
