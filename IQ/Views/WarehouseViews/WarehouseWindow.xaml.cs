using IQ.Helpers.FileOperations;
using IQ.Helpers.WindowsOperations;
using IQ.Views.WarehouseViews.Pages.Inventory;
using IQ.Views.WarehouseViews.Pages.Purchases;
using IQ.Views.WarehouseViews.Pages.ReturnInwards;
using IQ.Views.WarehouseViews.Pages.ReturnOutwards;
using IQ.Views.WarehouseViews.Pages.TransferInwards;
using IQ.Views.WarehouseViews.Pages.TransferOutwards;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.ApplicationModel;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.WarehouseViews
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WarehouseWindow : Window
    {
        public WarehouseWindow()
        {
            // Set the initial window size
            this.SetWindowSize(1600, 900);
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(WarehouseTitleBar);
        }

        private void WarehouseViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                //contentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                // Handle regular menu item invocation
                var itemContent = args.InvokedItemContainer;
                if (itemContent != null)
                {
                    switch (itemContent.Tag.ToString())
                    {
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

                        case "WarehouseInventoryPage":
                            contentFrame.Navigate(typeof(WarehouseInventoryPage), contentFrame);
                            break;
                    }
                }
            }

        }

        private void WarehouseNavLoaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in WarehouseNavigator.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "WareHouseInventoryPage")
                {
                    WarehouseNavigator.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(WarehouseInventoryPage));
        }

        private void WarehouseWindowExit_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApp(this);
        }

        private void WarehouseWindowLogout_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.Logout(this);
        }

        private async void CheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            Package package = Package.Current;
            PackageUpdateAvailabilityResult result = await package.CheckUpdateAvailabilityAsync();
            switch (result.Availability)
            {
                case PackageUpdateAvailability.Available:
                case PackageUpdateAvailability.Required:
                    //update is available
                    ShowCompletionAlertDialogAsync("Updates Available. Please Restart the app to Install.", this);
                    break;
                case PackageUpdateAvailability.NoUpdates:
                    //no updates available
                    ShowCompletionAlertDialogAsync("No Updates Available", this);
                    break;
                case PackageUpdateAvailability.Unknown:
                default:
                    break;
            }
        }

        public static async void ShowCompletionAlertDialogAsync(string alert, Window m)
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
            alertDialog.XamlRoot = m.Content.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

        }

        private async void ExportAllEntriesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseTOutsPdfForMonth(this);
            await PDFOperations.CreateWarehouseROutsPdfForMonth(this);
            await PDFOperations.CreateWarehousePurchasesPdfForMonth(this);
            await PDFOperations.CreateWarehouseTInsPdfForMonth(this);
            await PDFOperations.CreateWarehouseRInsPdfForMonth(this);

            ShowCompletionAlertDialogAsync("Exports Complete", this);
        }

        private async void ExportTOutsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseTOutsPdfForMonth(this);
            ShowCompletionAlertDialogAsync("Export Complete", this);
        }

        private async void ExportROutsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseROutsPdfForMonth(this);
            ShowCompletionAlertDialogAsync("Export Complete", this);
        }

        private async void ExportPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehousePurchasesPdfForMonth(this);
            ShowCompletionAlertDialogAsync("Export Complete", this);
        }

        private async void ExportTInsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseTInsPdfForMonth(this);
            ShowCompletionAlertDialogAsync("Export Complete", this);
        }

        private async void ExportRInsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseRInsPdfForMonth(this);
            ShowCompletionAlertDialogAsync("Export Complete", this);
        }
    }
}
