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
using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages;
using Npgsql;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using IQ.Views.WarehouseViews.Pages.TransferOutwards.SubPages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.WarehouseViews.Pages.TransferOutwards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransferOutwardsPage : Page
    {
        public WHTOutsViewModel ViewModel { get; } = new WHTOutsViewModel();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        // Initialize OverlayInstance
        public static AddTOutsOverlay OverlayInstance = new AddTOutsOverlay();

        public TransferOutwardsPage()
        {
            this.InitializeComponent();
            WarehouseTOutsDatePicker.SelectedDate = DateFilter;
            WarehouseTOutsDatePicker.MaxYear = DateTime.UtcNow.Date;
            DataContext = ViewModel;

            // Subscribe to the VisibilityChanged event of the popup page
            OverlayInstance.VisibilityChanged += PopupPageVisibilityChanged!;
        }

        private void WarehouseTOutsDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            DateFilter = WarehouseTOutsDatePicker.Date.UtcDateTime;
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
            Frame.Navigate(typeof(TransferOutwardsPage));
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
                    using (NpgsqlCommand command = new NpgsqlCommand($"SELECT DISTINCT TransferID FROM {App.UserName}.TransferOutwards WHERE DATE(Date) = @time;", connection))
                    {
                        command.Parameters.AddWithValue("time", DateFilter!.Value.DateTime!);
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
                WarehouseTOutsAutoSuggestBox.ItemsSource = suggestions;
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

        private void TOutsAddButton_Click(object sender, RoutedEventArgs e)
        {

            // Show the popup by setting its visibility to Visible
            OverlayInstance.SetVisibility(Visibility.Visible);
            TOutsOverlayPopUp.IsOpen = true;

        }

        private void WarehouseTOutsAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        private async void WarehouseTOutsAutoSuggestBox_QuerySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<WarehouseTOut> searchResults = await DatabaseExtensions.QueryWHTOutsResultsFromDatabase(userQuery);

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdateTOutsPageWithResults(searchResults);
            }
            else
            {
                UpdateTOutsPageWithResults(ViewModel.WarehouseTOut);
            }
        }

        private void UpdateTOutsPageWithResults(ObservableCollection<WarehouseTOut> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                WarehouseTOutsDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }

        private async void WarehouseTOutsAutoSuggestBox_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryWHTOutsSuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }
    }
}
