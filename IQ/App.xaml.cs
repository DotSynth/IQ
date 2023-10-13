using Amazon;
using Amazon.S3;
using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews;
using IQ.Views.BranchViews;
using IQ.Views.WarehouseViews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage;

namespace IQ
{
    public partial class App : Application
    {
        public static string? Username;

        public App()
        {
            this.RequestedTheme = ApplicationTheme.Light;
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            base.OnLaunched(args);

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // load a setting that is local to the device            
            String? localValue = localSettings.Values["IQ SETTING"] as string;

            // load a composite setting
            Windows.Storage.ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)localSettings.Values["USER LOGIN"];
            if ((localValue != null) && (composite != null))
            {
                ConnectionString = composite["ConnectionString"] as string;
                Username = composite["Username"] as string;

                if (ConnectionString != null)
                {
                    if (DatabaseExtensions.ConnectToDb(ConnectionString) == true)
                    {
                        if (DatabaseExtensions.IsAnAdministrator())
                        {                         
                            if (DatabaseExtensions.TriggerDbMassAction_Admin())
                            {
                                m_window = new AdminWindow();

                                // Create a Frame to act as the navigation context and navigate to the first page
                                Frame rootFrame = new Frame();
                                m_window.Activate();
                            }
                            else
                            {
                                Exit();
                            }
                        }
                        else if (DatabaseExtensions.GetCurrentUserRole() == "BRANCH")
                        {
                            if (DatabaseExtensions.TriggerDbMassAction_Branch())
                            {
                                m_window = new BranchWindow();
                                // Create a Frame to act as the navigation context and navigate to the first page
                                Frame rootFrame = new Frame();
                                m_window.Activate();
                            }
                            else
                            {
                                Exit();
                            }
                        }
                        else
                        {
                            if (DatabaseExtensions.TriggerDbMassAction_Warehouse())
                            {
                                m_window = new WarehouseWindow();
                                // Create a Frame to act as the navigation context and navigate to the first page
                                Frame rootFrame = new Frame();
                                m_window.Activate();
                            }
                            else
                            {
                                Exit();
                            }
                        }
                    }
                }
            }
            else
            {
                // Perform login and get user role
                m_window = new LoginWindow();
                Frame rootFrame = new Frame();
                m_window.Activate();
            }
        }
        private Window? m_window;
        public static string? ConnectionString;
    }
}
