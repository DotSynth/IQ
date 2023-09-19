using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews;
using IQ.Views.BranchViews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;
using Windows.System;
using Windows.UI.ViewManagement;

namespace IQ
{
    public partial class App : Application
    {

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
                if (ConnectionString != null)
                {
                    if (DatabaseExtensions.ConnectToDb(ConnectionString) == true)
                    {
                        if (DatabaseExtensions.GetCurrentUserRole() == "Admin")
                        {
                            m_window = new AdminWindow();

                            // Create a Frame to act as the navigation context and navigate to the first page
                            Frame rootFrame = new Frame();
                            m_window.Activate();
                        }
                        else
                        {
                            m_window = new BranchWindow();
                            // Create a Frame to act as the navigation context and navigate to the first page
                            Frame rootFrame = new Frame();
                            m_window.Activate();
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
