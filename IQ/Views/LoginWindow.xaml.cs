using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.FileOperations;
using IQ.Helpers.WindowsOperations;
using IQ.Views.AdminViews;
using IQ.Views.BranchViews;
using IQ.Views.WarehouseViews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace IQ.Views
{
    public sealed partial class LoginWindow : Window
    {
        public static string? DataServer;
        public static string User = @"UserLogin.IQX";
        public static string? Port;
        public static string? Database;
        public static string? Username;
        public static string? Password;
        public static string? ConnectionString;
        public static string[] Datastore = [];
        private const int Height = 750;
        private const int Width = 400;
        private Window? m_window;

        public LoginWindow()
        {
            this.SetWindowSize(400, 750);
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(400, 750);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(LoginTitleBar);

        }

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
            {
                passworBoxString.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                passworBoxString.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }

        public void LoadBranchWindow()
        {
            m_window = new BranchWindow();
            // Create a Frame to act as the navigation context and navigate to the Branch Window
            Frame rootFrame = new Frame();
            m_window.Activate();
        }

        public void LoadAdminWindow()
        {
            m_window = new AdminWindow();
            // Create a Frame to act as the navigation context and navigate to the Admin Window
            Frame rootFrame = new Frame();
            m_window.Activate();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <exception cref="NotSupportedException"></exception>
        private void Login(object sender, RoutedEventArgs e)
        {
            DataServer = ServerString.Text;
            Port = PortString.Text;
            Database = DatabaseString.Text;
            Username = UsernameString.Text;
            Password = passworBoxString.Password;
            App.ConnectionString = ConnectionString = $"Server={DataServer};Database={Database};Port={Port};User Id ={Username};Password={Password};Include Error Detail=true";
            Datastore = [DataServer, Port, Database, Username, Password, ConnectionString];
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // Save a setting locally on the device
            localSettings.Values["IQ SETTING"] = "CURRRENT DEVICE";

            // Save a composite setting locally on the device
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
            composite["DataServer"] = DataServer;
            composite["Port"] = Port;
            composite["Database"] = Database;
            composite["Username"] = Username;
            composite["Password"] = Password;
            composite["ConnectionString"] = ConnectionString;
            localSettings.Values["USER LOGIN"] = composite;
            if (ConnectionString != null)
            {
                if (DatabaseExtensions.ConnectToDb(ConnectionString, this) == true)
                {
                    App.Username = Username;
                    if (DatabaseExtensions.IsAnAdministrator())
                    {
                        DatabaseExtensions.TriggerDbMassAction_Admin();
                        LoadAdminWindow();
                        this.Close();
                    }
                    else if (DatabaseExtensions.GetCurrentUserRole() == "BRANCH")
                    {
                        DatabaseExtensions.TriggerDbMassAction_Branch();
                        LoadBranchWindow();
                        this.Close();
                    }
                    else
                    {
                        DatabaseExtensions.TriggerDbMassAction_Warehouse();
                        LoadWarehouseWindow();
                        this.Close();
                    }
                }
            }

        }

        private void LoadWarehouseWindow()
        {
            m_window = new WarehouseWindow();
            // Create a Frame to act as the navigation context and navigate to the Admin Window
            Frame rootFrame = new Frame();
            m_window.Activate();
        }
    }
}
