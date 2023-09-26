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
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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
                            contentFrame.Navigate(typeof(SalesPage));
                            break;

                        case "PurchasesPage":
                            contentFrame.Navigate(typeof(PurchasesPage));
                            break;

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

                        case "CommitHistoryPage":
                            contentFrame.Navigate(typeof(CommitHistoryPage));
                            break;

                        case "BranchInventoryPage":
                            contentFrame.Navigate(typeof(BranchInventoryPage));
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
