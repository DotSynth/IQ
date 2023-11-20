using IQ.Helpers.WindowsOperations;
using IQ.Views.AdminViews.Pages.Inventory;
using IQ.Views.AdminViews.Pages.ManageUsers;
using IQ.Views.AdminViews.Pages.Overview;
using IQ.Views.AdminViews.Pages.Purchases;
using IQ.Views.AdminViews.Pages.ReturnInwards;
using IQ.Views.AdminViews.Pages.ReturnOutwards;
using IQ.Views.AdminViews.Pages.Sales;
using IQ.Views.AdminViews.Pages.TransferInwards;
using IQ.Views.AdminViews.Pages.TransferOutwards;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.AdminViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            // Set the initial window size
            this.SetWindowSize(1600, 900);
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AdminTitleBar);
        }

        private void AdminViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
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
                        case "ManageUsersPage":
                            contentFrame.Navigate(typeof(ManageUsersPage), contentFrame);
                            break;

                        case "CompanySalesPage":
                            contentFrame.Navigate(typeof(CompanySalesPage), contentFrame);
                            break;

                        case "CompanyPurchasesPage":
                            contentFrame.Navigate(typeof(CompanyPurchasesPage), contentFrame);
                            break;

                        case "CompanyRInsPage":
                            contentFrame.Navigate(typeof(CompanyRInsPage), contentFrame);
                            break;

                        case "CompanyROutsPage":
                            contentFrame.Navigate(typeof(CompanyROutsPage), contentFrame);
                            break;

                        case "CompanyTOutsPage":
                            contentFrame.Navigate(typeof(CompanyTOutsPage), contentFrame);
                            break;

                        case "CompanyTInsPage":
                            contentFrame.Navigate(typeof(CompanyTInsPage), contentFrame);
                            break;

                        case "CompanyInventoryPage":
                            contentFrame.Navigate(typeof(CompanyInventoryPage), contentFrame);
                            break;

                        case "CompanyOverviewPage":
                            contentFrame.Navigate(typeof(CompanyOverviewPage), contentFrame);
                            break;
                    }
                }
            }

        }

        private void AdminNavLoaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in AdminNavigator.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "CompanyOverviewPage")
                {
                    AdminNavigator.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(CompanyOverviewPage));
        }

        private void AdminWindowExit_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApp(this);
        }

        /// <exception cref="System.AppDomainUnloadedException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        private void AdminWindowLogout_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.Logout(this);
        }

        /*
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
        */

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

        private void contentFrame_NavigationFailed(object sender, Microsoft.UI.Xaml.Navigation.NavigationFailedEventArgs e)
        {
            contentFrame.Navigate(typeof(ErrorPage), contentFrame);
        }
    }
}
