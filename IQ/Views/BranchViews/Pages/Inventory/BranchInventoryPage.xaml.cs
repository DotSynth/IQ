using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Views.BranchViews.Pages.Inventory.SubPages;
using IQ.Views.BranchViews.Pages.Sales;
using IQ.Views.BranchViews.Pages.Sales.SubPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.Inventory
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BranchInventoryPage : Page
    {

        public BranchInventoryViewModel ViewModel { get; } = new BranchInventoryViewModel();
        private List<string> suggestions = new List<string>();
        // Initialize OverlayInstance
        public static AddInventoryOverlay OverlayInstance = new AddInventoryOverlay();

        public BranchInventoryPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;

            // Subscribe to the VisibilityChanged event of the popup page
            OverlayInstance.VisibilityChanged += PopupPageVisibilityChanged!;
        }

        public async Task RefreshPage()
        {
            // Do something before the delay
            // Navigate away to a placeholder page
            Frame.Navigate(typeof(PLaceHolderPage));

            // Delay for 5 seconds (5000 milliseconds)
            await Task.Delay(50000);

            // Continue with the next line of code after the delay
            // Navigate back to the original page to refresh it
            Frame.Navigate(typeof(BranchInventoryPage));
        }


        private void PopupPageVisibilityChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("PopupPageVisibilityChanged called");
            Debug.WriteLine($"PopupPageVisibilityChanged: OverlayInstance.Visibility = {OverlayInstance.Visibility}");

            // Check if the popup page's visibility is collapsed
            if (OverlayInstance.Visibility == Visibility.Collapsed)
            {
                // Trigger the RefreshPage() function
                _ = RefreshPage();
            }
        }

        private void InventoryAddButton_Click(object sender, RoutedEventArgs e)
        {

            // Show the popup by setting its visibility to Visible
            OverlayInstance.SetVisibility(Visibility.Visible);
            InventoryOverlayPopUp.IsOpen = true;

        }

        private void BranchInventoryAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }

        private async void BranchInventoryAutoSuggestBox_QuerySubmittedAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<BranchInventory> searchResults = await DatabaseExtensions.QueryInventoryResultsFromDatabase(userQuery);
                Debug.WriteLine("PopupPageVisibilityChanged called");

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdateInventoryPageWithResults(searchResults);
            }
            else
            {
                UpdateInventoryPageWithResults(ViewModel.BranchInventory);
            }
        }

        private void UpdateInventoryPageWithResults(ObservableCollection<BranchInventory> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                BranchInventoryDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }



        private async void BranchInventoryAutoSuggestBox_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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
    }
}
