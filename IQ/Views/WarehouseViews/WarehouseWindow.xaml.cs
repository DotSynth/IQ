using IQ.Views.WarehouseViews.Pages.Inventory;
using IQ.Views.WarehouseViews.Pages.ReturnOutwards;
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
                if (item is NavigationViewItem && item.Tag.ToString() == "InventoryPage")
                {
                    WarehouseNavigator.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(WarehouseInventoryPage));
        }
        private void ContentFrame_NavigationFailed(Object sender, NavigationFailedEventArgs args)
        {

        }
    }
}
