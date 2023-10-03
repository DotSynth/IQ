using IQ.Helpers.WindowsOperations;
using IQ.Views.WarehouseViews.Pages.Inventory;
using IQ.Views.WarehouseViews.Pages.ReturnInwards;
using IQ.Views.WarehouseViews.Pages.ReturnOutwards;
using IQ.Views.WarehouseViews.Pages.TransferInwards;
using IQ.Views.WarehouseViews.Pages.TransferOutwards;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;

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
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(WarehouseTitleBar);
            ApplicationView.PreferredLaunchViewSize = new Size(1200, 840);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
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
                    switch (itemContent.Name)
                    {
                        case "TransferInwardsPage":
                            contentFrame.Navigate(typeof(TransferInwardsPage));
                            break;

                        case "TransferOutwardsPage":
                            contentFrame.Navigate(typeof(TransferOutwardsPage));
                            break;

                        case "ReturnInwardsPage":
                            contentFrame.Navigate(typeof(ReturnInwardsPage));
                            break;

                        case "ReturnOutwardsPage":
                            contentFrame.Navigate(typeof(ReturnOutwardsPage));
                            break;

                        case "WarehouseInventoryPage":
                            contentFrame.Navigate(typeof(WarehouseInventoryPage));
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

    }
}
