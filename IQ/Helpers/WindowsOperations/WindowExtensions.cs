using IQ.Helpers.DatabaseOperations;
using IQ.Views;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.UI;
using WinRT.Interop;

namespace IQ.Helpers.WindowsOperations
{
    public static class WindowExtensions
    {
        private static Window? m_window;

        public static AppWindow GetAppWindow(this Window window)
        {
            IntPtr windowHandle = WindowNative.GetWindowHandle(window);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            return AppWindow.GetFromWindowId(windowId);
        }

        public static (int Width, int Height) GetCurrentWindowSize(this Window window)
        {
            SizeInt32 size = window.GetAppWindow().Size;
            return (size.Width, size.Height);
        }

        public static void SetWindowSize(this Window window, int width, int height)
        {
            AppWindow appWindow = window.GetAppWindow();
            SizeInt32 size = new(width, height);
            appWindow.Resize(size);
        }

        public static void SetIsAlwaysOnTop(this Window window, bool value)
        {
            AppWindow appWindow = window.GetAppWindow();

            if (appWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                overlappedPresenter.IsAlwaysOnTop = value;
                return;
            }

            throw new NotSupportedException($"Always on top is not supported with {appWindow.Presenter.Kind}.");
        }

        public static bool GetIsAlwaysOnTop(this Window window)
        {
            AppWindow appWindow = window.GetAppWindow();

            if (appWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                return overlappedPresenter.IsAlwaysOnTop;
            }

            throw new NotSupportedException($"Always on top is not supported with {appWindow.Presenter.Kind}.");
        }

        public static async void ExitApp(Window m)
        {
            var result = await ShowCompletionAlertDialogAsync("Are You Sure You want to Exit?", m);
            if (result == ContentDialogResult.Secondary)
            {
                Application.Current.Exit();
            }

        }

        private static async Task<ContentDialogResult> ShowCompletionAlertDialogAsync(string alert, Window m)
        {
            // Create a ContentDialog
            ContentDialog alertDialog = new ContentDialog
            {
                // Set the title, content, and close button text
                Title = "Alert",
                Content = alert,
                PrimaryButtonText = "NO",
                SecondaryButtonText = "YES",
                IsSecondaryButtonEnabled = true,
                IsPrimaryButtonEnabled = true,
            };

            // Set the foreground to hex color #020066
            alertDialog.Foreground = new SolidColorBrush(Color.FromArgb(255, 2, 0, 102));

            // Set the XamlRoot property to the same as an element in the app window
            // For example, if you have a StackPanel named MyPanel in your XAML
            alertDialog.XamlRoot = m.Content.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

            return result;
        }

        public static async void Logout(Window m)
        {
            var result = await ShowCompletionAlertDialogAsync(@"Are You Sure You want to Log Out?
This Will Clear Login Information.", m);
            if (result == ContentDialogResult.Secondary)
            {

                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User)) && (DatabaseExtensions.CloseConnection() == true))
                {
                    // If file found, delete it
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User));
                    m_window = new LoginWindow();
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Frame rootFrame = new Frame();
                    m_window.Activate();
                    m.Close();
                }
            }
        }
    }
}