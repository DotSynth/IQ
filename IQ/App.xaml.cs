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
using System.IO;
using Windows.Storage;

namespace IQ
{
    public partial class App : Application
    {
        public static string? Username;

        public App()
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("AKIAXH3AA2SOIL5MO2RW", "nbx9FE6lVBDaEHHMMqKOn1t/ZE7cIdM9sLAcv74t");
            var s3Config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.EUNorth1, // Replace with your desired AWS region
            };

            var s3Client = new AmazonS3Client(awsCredentials, s3Config);

            this.RequestedTheme = ApplicationTheme.Light;
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            base.OnLaunched(args);
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User)))
            {
                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                // load a setting that is local to the device
                String? localValue = localSettings.Values["IQ SETTING"] as string;

                // load a composite setting
                Windows.Storage.ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)localSettings.Values["USER LOGIN"];
                if (composite != null)
                {
                    ConnectionString = composite["DataServer"] as string;
                    Username = composite["Username"] as string;
                }
                if (ConnectionString != null)
                {
                    if (DatabaseExtensions.ConnectToDb(ConnectionString) == true)
                    {
                        if (DatabaseExtensions.GetCurrentUserRole() == "ADMIN")
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
        private string? ConnectionString;
    }
}
