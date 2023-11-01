using IQ.Helpers.DatabaseOperations;
using IQ.Helpers.FileOperations;
using IQ.Helpers.WindowsOperations;
using IQ.Views.AdminViews;
using IQ.Views.BranchViews;
using IQ.Views.WarehouseViews;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Loading : Window
    {
        private Window? m_window;

        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="AggregateException"></exception>
        public Loading()
        {
            // Set the initial window size
            this.SetWindowSize(500, 500);
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(TitleBar);

            // Set the source of the Image control to your GIF file
            BitmapImage bitmapImage = new(new Uri("ms-appx:///Assets/Images/IqLogoNoBackground.png"));
            LoadingAnimation.Source = bitmapImage;
            _ = LoadAllPages();
        }

        private async Task<bool> LoadAllPages()
        {
            await Task.Delay(2000);
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // load a setting that is local to the device            
            String? localValue = localSettings.Values["IQ SETTING"] as string;

            // load a composite setting
            Windows.Storage.ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)localSettings.Values["USER LOGIN"];
            if ((localValue != null) && (composite != null))
            {
                App.ConnectionString = composite["ConnectionString"] as string;
                App.Username = composite["Username"] as string;

                if (App.ConnectionString != null)
                {
                    if (DatabaseExtensions.ConnectToDb(App.ConnectionString, this) == true)
                    {
                        if (DatabaseExtensions.IsAnAdministrator())
                        {
                            if (DatabaseExtensions.TriggerDbMassAction_Admin())
                            {
                                LoadAdminWindow();

                                this.Close();
                                return Task.CompletedTask.IsCompleted;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (DatabaseExtensions.GetCurrentUserRole() == "BRANCH")
                        {
                            if (DatabaseExtensions.TriggerDbMassAction_Branch())
                            {
                                LoadBranchWindow();
                                this.Close();
                                return Task.CompletedTask.IsCompleted;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (DatabaseExtensions.TriggerDbMassAction_Warehouse())
                            {
                                LoadWarehouseWindow();
                                this.Close();
                                return Task.CompletedTask.IsCompleted;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return await LoadAllPages();
                    }
                }
                else
                {
                    // Perform login and get user role
                    m_window = new LoginWindow();
                    Frame rootFrame = new Frame();
                    m_window.Activate();
                    this.Close();
                    return Task.CompletedTask.IsCompleted;
                }
            }
            else
            {
                // Perform login and get user role
                m_window = new LoginWindow();
                Frame rootFrame = new Frame();
                m_window.Activate();
                this.Close();
                return Task.CompletedTask.IsCompleted;
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



        private void LoadWarehouseWindow()
        {
            m_window = new WarehouseWindow();
            // Create a Frame to act as the navigation context and navigate to the Admin Window
            Frame rootFrame = new Frame();
            m_window.Activate();
        }
    }
}
