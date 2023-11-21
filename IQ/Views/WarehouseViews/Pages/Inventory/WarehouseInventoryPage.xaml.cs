using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Views.BranchViews.Pages;
using IQ.Views.WarehouseViews.Pages.Inventory.SubPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.WarehouseViews.Pages.Inventory
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WarehouseInventoryPage : Page
    {
        public WHInventoryViewModel? ViewModel { get; set; } = Views.Loading.WIViewModel;
        private List<string> suggestions = new List<string>();

        // Initialize OverlayInstance
        public static AddInventoryOverlay OverlayInstance = new AddInventoryOverlay();

        public WarehouseInventoryPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;

            // Subscribe to the VisibilityChanged event of the popup page
            OverlayInstance.VisibilityChanged += PopupPageVisibilityChanged!;
        }

        public async void RefreshPage()
        {
            // Do something before the delay
            Views.Loading.WIViewModel = new WHInventoryViewModel()!;

            // Navigate away to a placeholder page
            Frame.Navigate(typeof(PLaceHolderPage));

            await Task.Delay(2000);
            // Continue with the next line of code after the delay
            // Navigate back to the original page to refresh it
            Frame.Navigate(typeof(WarehouseInventoryPage));
            Frame.NavigationFailed += Frame_NavigationFailed;
        }

        private void Frame_NavigationFailed(object sender, Microsoft.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {
            Frame.Navigate(typeof(ErrorPage), Frame);
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

        private void InventoryAddButton_Click(object sender, RoutedEventArgs e)
        {

            // Show the popup by setting its visibility to Visible
            OverlayInstance.SetVisibility(Visibility.Visible);
            InventoryOverlayPopUp.IsOpen = true;

        }

        private void WarehouseInventoryAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        /// <exception cref="System.IO.IOException"></exception>
        private async void WarehouseInventoryAutoSuggestBox_QuerySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<WarehouseInventory> searchResults = await DatabaseExtensions.QueryWHInventoryResultsFromDatabase(userQuery);

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdateInventoryPageWithResults(searchResults);
            }
            else
            {
                UpdateInventoryPageWithResults(Views.Loading.WIViewModel!.WarehouseInventory);
            }
        }

        /// <exception cref="System.IO.IOException"></exception>
        private void UpdateInventoryPageWithResults(ObservableCollection<WarehouseInventory> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                WarehouseInventoryDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }



        private async void WarehouseInventoryAutoSuggestBox_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryWHInventorySuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }
    }
}
