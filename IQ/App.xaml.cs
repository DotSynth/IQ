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

namespace IQ
{
    public partial class App : Application
    {
        public static string? UserName;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User)))
            {
                var ConnectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;
                UserName = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).Username;
                if (ConnectionString != null)
                {
                    if (DatabaseExtensions.ConnectToDb(ConnectionString) == true)
                    {
                        Debug.WriteLine(DatabaseExtensions.GetCurrentUserRole());
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
    }
}
