﻿using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages;
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

namespace IQ.Views.AdminViews.Pages.TransferOutwards
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompanyTOutsPage : Page
    {
        public static string? SelectedView = App.Username;
        public static string? PreviousView = SelectedView;
        public List<string> Schemas { get; } = new List<string>();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        public CompanyTOutsViewModel ViewModel { get; } = new CompanyTOutsViewModel();


        public CompanyTOutsPage()
        {
            this.InitializeComponent();
            Task task = LoadSuggestionsAsync();
            CompanyTOutsDatePicker.SelectedDate = DateFilter;
            CompanyTOutsDatePicker.MaxYear = DateTime.UtcNow.Date;
            Schemas = DatabaseExtensions.GetSchemas();
            USERComboBox.ItemsSource = Schemas;
            DataContext = ViewModel;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the ComboBox value here
            USERComboBox.SelectedItem = SelectedView;
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

        private void CompanyTOutsAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is string chosenSuggestion)
            {
                sender.Text = chosenSuggestion;
            }
        }


        private async void CompanyTOutsAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                // Perform a database query based on the user's queryText
                string userQuery = args.QueryText;
                ObservableCollection<CompanyTOut> searchResults = await DatabaseExtensions.QueryCompanyTOutsResultsFromDatabase(userQuery);

                // Display the searchResults on your SalesPage or in a DataGrid
                UpdateTOutsPageWithResults(searchResults);
            }
            else
            {
                UpdateTOutsPageWithResults(ViewModel.CompanyTOut);
            }
        }

        private void UpdateTOutsPageWithResults(ObservableCollection<CompanyTOut> searchResults)
        {
            try
            {
                // Assuming that BranchSalesDataGrid is the name of your DataGrid
                CompanyTOutsDataGrid.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Debug.WriteLine(ex.Message);
            }
        }

        private async void CompanyTOutsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // Query the database for suggestions based on the user's input
                string userInput = sender.Text;
                List<string> suggestions = await DatabaseExtensions.QueryCompanyTOutsSuggestionsFromDatabase(userInput);

                // Set the suggestions for the AutoSuggestBox
                sender.ItemsSource = suggestions;
            }
        }

        private void CompanyTOutsDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            DateFilter = CompanyTOutsDatePicker.Date.UtcDateTime;
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
            Frame.Navigate(typeof(CompanyTOutsPage));
        }


        private async Task LoadSuggestionsAsync()
        {
            try
            {
                // Establish a connection to your PostgreSQL database
                using (NpgsqlConnection connection = new NpgsqlConnection(App.ConnectionString!))
                {
                    await connection.OpenAsync();

                    // Query the database to retrieve values from the 'columnName' column
                    using (NpgsqlCommand command = new NpgsqlCommand($"SELECT DISTINCT InvoiceID FROM \"{SelectedView}\".Purchases WHERE DATE(Date) = @time;", connection))
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
                CompanyTOutsAutoSuggestBox.ItemsSource = suggestions;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                string error = ex.Message;
                // You should implement proper error handling here.
            }
        }
    }
}
