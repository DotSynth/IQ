using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.ReturnOutwards.SubPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.ReturnOutwards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReturnOutwardsPage : Page
    {
        public BranchROutsViewModel ViewModel { get; } = new BranchROutsViewModel();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        // Initialize OverlayInstance
        public static AddROutsOverlay OverlayInstance = new AddROutsOverlay();

        public ReturnOutwardsPage()
        {
            this.InitializeComponent();
            _ = LoadSuggestionsAsync();
            BranchROutsDatePicker.SelectedDate = DateFilter;
            BranchROutsDatePicker.MaxYear = DateTime.UtcNow.Date;
            DataContext = ViewModel;

            // Subscribe to the VisibilityChanged event of the popup page
            OverlayInstance.VisibilityChanged += PopupPageVisibilityChanged!;
        }

        private void BranchROutsDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            DateFilter = BranchROutsDatePicker.Date.UtcDateTime;
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
            Frame.Navigate(typeof(ReturnOutwardsPage));
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
                    using (NpgsqlCommand command = new NpgsqlCommand($"SELECT DISTINCT ReturnID FROM \"{App.UserName}\".ReturnOutwards WHERE DATE(Date) = @time;", connection))
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
                BranchROutsAutoSuggestBox.ItemsSource = suggestions;
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

        private void ROutsAddButton_Click(object sender, RoutedEventArgs e)
        {

            // Show the popup by setting its visibility to Visible
            OverlayInstance.SetVisibility(Visibility.Visible);
            ROutsOverlayPopUp.IsOpen = true;

        }

        private void BranchROutsAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        private async void BranchROutsAutoSuggestBox_QuerySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<BranchROut> searchResults = await DatabaseExtensions.QueryROutsResultsFromDatabase(userQuery);

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdateROutsPageWithResults(searchResults);
            }
            else
            {
                UpdateROutsPageWithResults(ViewModel.BranchROut);
            }
        }

        private void UpdateROutsPageWithResults(ObservableCollection<BranchROut> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                BranchROutsDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }

        private async void BranchROutsAutoSuggestBox_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryROutsSuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }
    }
}
