using IQ.Helpers.WindowsOperations;
using IQ.Views.AdminViews.Pages.ManageUsers;
using IQ.Views.WarehouseViews.Pages.Inventory;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

                    }
                }
            }

        }

        private void AdminNavLoaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in AdminNavigator.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "ManageUsersPage")
                {
                    AdminNavigator.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(ManageUsersPage));
        }

        private void AdminWindowExit_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApp(this);
        }

        private void AdminWindowLogout_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.Logout(this);
        }
    }
}
