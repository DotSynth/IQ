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
        private static Window? m_window;
        public static string? ConnectionString;
        public static string? Username;

        public App()
        {
            this.RequestedTheme = ApplicationTheme.Light;
            this.InitializeComponent();
        }

        /// <exception cref="UriFormatException"></exception>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            base.OnLaunched(args);
            m_window = new Loading();
            // Create a Frame to act as the navigation context and navigate to the first page
            Frame rootFrame = new Frame();
            m_window.Activate();

        }
    }
}
