using IQ.Helpers.WindowsOperations;
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
using Microsoft.UI.Xaml.Navigation;
using System;
// To learn more about WinUI, the WinUI project structure, and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.BranchViews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BranchWindow : Window
    {
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
                var itemContent = args.InvokedItemContainer;
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

                        case "CommitHistoryPage":
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
                    BranchNavigator.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(SalesPage));
        }
        private void ContentFrame_NavigationFailed(Object sender, NavigationFailedEventArgs args)
        {

        }
    }
}
