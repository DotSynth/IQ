﻿using CommunityToolkit.WinUI.UI;
using IQ.Helpers.DataTableOperations.ViewModels;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.Sales.SubPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews.Pages.Sales
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SalesPage : Page
    {
        public BranchSalesViewModel ViewModel { get; } = new BranchSalesViewModel();
        private List<string> suggestions = new List<string>();
        public static DateTimeOffset? DateFilter = DateTime.UtcNow.Date;
        // Initialize OverlayInstance
        public static AddSaleOverlay OverlayInstance = new AddSaleOverlay();


        public SalesPage()
        {
            this.InitializeComponent();
            _ = LoadSuggestionsAsync();
            BranchSalesDatePicker.SelectedDate = DateFilter;
            BranchSalesDatePicker.MaxYear = DateTime.UtcNow.Date;
            DataContext = ViewModel;

            // Subscribe to the VisibilityChanged event of the popup page
            OverlayInstance.VisibilityChanged += PopupPageVisibilityChanged!;
        }

        private void RefreshPage()
        {
            // Navigate away to a placeholder page
            Frame.Navigate(typeof(PLaceHolderPage));

            _ = DelayedExecutionAsync();

            // Navigate back to the original page to refresh it
            Frame.Navigate(typeof(SalesPage));
        }

        private void BranchSalesDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            DateFilter = BranchSalesDatePicker.Date.UtcDateTime;
            RefreshPage();
        }

        async Task DelayedExecutionAsync()
        {
            // Do something before the delay

            // Delay for 5 seconds (5000 milliseconds)
            await Task.Delay(50000);

            // Continue with the next line of code after the delay
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
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT DISTINCT InvoiceID FROM BranchSales WHERE DATE(Date) = @time;", connection)) {
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
                BranchSalesAutoSuggestBox.ItemsSource = suggestions;
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
            Debug.WriteLine("PopupPageVisibilityChanged called");
            Debug.WriteLine($"PopupPageVisibilityChanged: OverlayInstance.Visibility = {OverlayInstance.Visibility}");

            // Check if the popup page's visibility is collapsed
            if (OverlayInstance.Visibility == Visibility.Collapsed)
            {
                // Trigger the RefreshPage() function
                RefreshPage();
            }
        }

        private void SalesAddButton_Click(object sender, RoutedEventArgs e)
        { 

            // Show the popup by setting its visibility to Visible
            OverlayInstance.SetVisibility(Visibility.Visible);
            SaleOverlayPopUp.IsOpen = true;

        }
    }
}
