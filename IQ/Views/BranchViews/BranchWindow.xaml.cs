using IQ.Helpers.FileOperations;
using IQ.Helpers.WindowsOperations;
using IQ.Views.BranchViews.Pages;
using IQ.Views.BranchViews.Pages.CommitHistory;
using IQ.Views.BranchViews.Pages.Inventory;
using IQ.Views.BranchViews.Pages.Purchases;
using IQ.Views.BranchViews.Pages.ReturnInwards;
using IQ.Views.BranchViews.Pages.ReturnOutwards;
using IQ.Views.BranchViews.Pages.Sales;
using IQ.Views.BranchViews.Pages.TransferInwards;
using IQ.Views.BranchViews.Pages.TransferOutwards;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure, and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BranchWindow : Window
    {
        // Define an event to notify when Commit Button is clicked
        public static dynamic? itemContent;
        public static bool JustLoaded;

        public BranchWindow()
        {
            // Set the initial window size
            this.SetWindowSize(1600, 900);
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(BranchTitleBar);
        }

        private void BranchViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                //contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                // Handle regular menu item invocation
                itemContent = args.InvokedItemContainer;
                if (itemContent != null)
                {
                    switch (itemContent.Name)
                    {
                        case "SalesPage":
                            contentFrame.Navigate(typeof(SalesPage), contentFrame);
                            break;

                        case "PurchasesPage":
                            contentFrame.Navigate(typeof(PurchasesPage), contentFrame);
                            break;

                        case "TransferInwardsPage":
                            contentFrame.Navigate(typeof(TransferInwardsPage), contentFrame);
                            break;

                        case "TransferOutwardsPage":
                            contentFrame.Navigate(typeof(TransferOutwardsPage), contentFrame);
                            break;

                        case "ReturnInwardsPage":
                            contentFrame.Navigate(typeof(ReturnInwardsPage), contentFrame);
                            break;

                        case "ReturnOutwardsPage":
                            contentFrame.Navigate(typeof(ReturnOutwardsPage), contentFrame);
                            break;

                        case "CommitsPage":
                            contentFrame.Navigate(typeof(CommitHistoryPage), contentFrame);
                            break;

                        case "BranchInventoryPage":
                            contentFrame.Navigate(typeof(BranchInventoryPage), contentFrame);
                            break;
                    }
                }
            }

        }
        private void BranchNavLoaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in BranchNavigator.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "SalesPage")
                {
                    JustLoaded = true;
                    itemContent = item;
                    BranchNavigator.SelectedItem = item;
                    JustLoaded = false;
                    break;
                }
            }
            contentFrame.Navigate(typeof(SalesPage));
        }

        private void BranchWindowExit_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApp(this);
        }

        private void BranchWindowLogout_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.Logout(this);
        }

        private async void CommitUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Guid uniqueKey = Guid.NewGuid();
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
                        cmd.CommandText = $"INSERT INTO \"{App.UserName}\".CommitHistory (CommitID) VALUES (@CommitID)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("CommitID", uniqueKey);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                    }

                    // Close the connection
                    conn.Close();
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                await ShowCompletionAlertDialogAsync(error);
            }

            if (itemContent != null)
            {
                if (JustLoaded == true)
                {
                    if (itemContent.Tag.ToString() == "CommitsPage")
                    {
                        contentFrame.Navigate(typeof(PLaceHolderPage), contentFrame);

                        await Task.Delay(2000);
                        contentFrame.Navigate(typeof(CommitHistoryPage), contentFrame);

                        // Create a ContentDialog
                        await ShowCompletionAlertDialogAsync("All entries have been pushed to the Admin Server");
                    }
                    else
                    {
                        await ShowCompletionAlertDialogAsync("All entries have been pushed to the Admin Server");
                    }
                }
                else
                {
                    if (itemContent.Name == "CommitsPage")
                    {
                        contentFrame.Navigate(typeof(PLaceHolderPage), contentFrame);

                        await Task.Delay(2000);
                        contentFrame.Navigate(typeof(CommitHistoryPage), contentFrame);

                        // Create a ContentDialog
                        await ShowCompletionAlertDialogAsync("All entries have been pushed to the Admin Server");
                    }
                    else
                    {
                        await ShowCompletionAlertDialogAsync("All entries have been pushed to the Admin Server");
                    }
                }
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
            alertDialog.XamlRoot = BranchWindowGrid.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

        }

        private async void ExportSalesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateSalesPdfForMonth(this);
        }

        private async void ExportAllEntriesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateSalesPdfForMonth(this);
            await PDFOperations.CreateTOutsPdfForMonth(this);
            await PDFOperations.CreateROutsPdfForMonth(this);
            await PDFOperations.CreatePurchasesPdfForMonth(this);
            await PDFOperations.CreateTInsPdfForMonth(this);
            await PDFOperations.CreateRInsPdfForMonth(this);
        }

        private async void ExportTOutsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateTOutsPdfForMonth(this);
        }

        private async void ExportROutsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateROutsPdfForMonth(this);
        }

        private async void ExportPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreatePurchasesPdfForMonth(this);
        }

        private async void ExportTInsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateTInsPdfForMonth(this);
        }

        private async void ExportRInsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateRInsPdfForMonth(this);
        }

        private void CheckUpdates_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
