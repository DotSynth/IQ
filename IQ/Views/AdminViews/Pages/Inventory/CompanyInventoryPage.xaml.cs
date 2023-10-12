using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Views.BranchViews.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.AdminViews.Pages.Inventory
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompanyInventoryPage : Page
    {
        public static string? SelectedView = App.Username;
        public static string? PreviousView = SelectedView;
        public List<string> Schemas { get; } = new List<string>();
        public CompanyInventoryViewModel ViewModel { get; } = new CompanyInventoryViewModel();
        public Decimal? Total { get; } = new Decimal();

        public CompanyInventoryPage()
        {
            this.InitializeComponent();
            Schemas = DatabaseExtensions.GetSchemas();
            // Bind the list of schemas to the ComboBox
            USERComboBox.ItemsSource = Schemas;
            Total = DatabaseExtensions.RetrieveTotalInventoryWorth();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the ComboBox value here
            USERComboBox.SelectedItem = SelectedView;
            TotalWorth.Text = $"Total Inventory Worth: {Total.ToString()}";
        }

        private void USERComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected schema (e.g., SchemaComboBox.SelectedItem)
            SelectedView = USERComboBox.SelectedItem as string;

            // Do something with the selected schema
            if (!string.IsNullOrEmpty(SelectedView))
            {
                // Perform actions based on the selected schema
                CheckVariable();

            }
        }

        private void CheckVariable()
        {
            if (!string.IsNullOrEmpty(SelectedView))
            {
                if (SelectedView != PreviousView)
                {
                    RefreshPage();
                    PreviousView = SelectedView;
                }
            }
        }

        public async void RefreshPage()
        {
            // Do something before the delay
            // Navigate away to a placeholder page
            Frame.Navigate(typeof(PLaceHolderPage));

            await Task.Delay(2000);
            // Continue with the next line of code after the delay
            // Navigate back to the original page to refresh it
            Frame.Navigate(typeof(CompanyInventoryPage));
        }



        private void CompanyInventoryAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        private async void CompanyInventoryAutoSuggestBox_QuerySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<CompanyInventory> searchResults = await DatabaseExtensions.QueryCompanyInventoryResultsFromDatabase(userQuery);

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdateInventoryPageWithResults(searchResults);
            }
            else
            {
                UpdateInventoryPageWithResults(ViewModel.CompanyInventory);
            }
        }

        private void UpdateInventoryPageWithResults(ObservableCollection<CompanyInventory> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                CompanyInventoryDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }

        private async void CompanyInventoryAutoSuggestBox_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryCompanyInventorySuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }
    }
}
