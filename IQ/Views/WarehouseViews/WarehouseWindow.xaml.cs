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

        private void CheckUpdates_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ExportAllEntriesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseTOutsPdfForMonth(this);
            await PDFOperations.CreateWarehouseROutsPdfForMonth(this);
            await PDFOperations.CreateWarehousePurchasesPdfForMonth(this);
            await PDFOperations.CreateWarehouseTInsPdfForMonth(this);
            await PDFOperations.CreateWarehouseRInsPdfForMonth(this);
        }

        private async void ExportTOutsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseTOutsPdfForMonth(this);
        }

        private async void ExportROutsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseROutsPdfForMonth(this);
        }

        private async void ExportPurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehousePurchasesPdfForMonth(this);
        }

        private async void ExportTInsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseTInsPdfForMonth(this);
        }

        private async void ExportRInsButton_Click(object sender, RoutedEventArgs e)
        {
            await PDFOperations.CreateWarehouseRInsPdfForMonth(this);
        }
    }
}
