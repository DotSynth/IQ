using IQ.Helpers.FileOperations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Npgsql;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IQ.Views.AdminViews.Pages.ManageUsers.Subpages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddUserOverlay : Page
    {
        public string? CurrentUserName;
        public string? CurrentPassword;
        public string? CurrentAccessType;
        public string? CurrentSyntaxAccessType;

        // Define an event to notify when visibility changes
        public event EventHandler? VisibilityChanged;

        public AddUserOverlay()
        {
            this.InitializeComponent();
        }

        private async void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUserName = UserNameTextBox.Text;
            CurrentPassword = PasswordTextBox.Text;

            // Create a connection string
            string connString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            try
            {
                // Create a connection object
                using (var conn = new NpgsqlConnection(connString))
                {
                    // Open the connection
                    conn.Open();

                    // Create a command object
                    using (var cmd = new NpgsqlCommand())
                    {
                        // Assign the connection to the command
                        cmd.Connection = conn;

                        // Write the SQL statement for inserting data
                        cmd.CommandText = $"INSERT INTO \"{App.Username}\".UserLogins (UserName, Password, AccessType) VALUES (@UserName, @Password, @AccessType)";

                        // Create parameters and assign values
                        cmd.Parameters.AddWithValue("UserName", CurrentUserName);
                        cmd.Parameters.AddWithValue("Password", CurrentPassword);
                        cmd.Parameters.AddWithValue("AccessType", CurrentAccessType!);

                        // Execute the command and get the number of rows affected
                        int rows = cmd.ExecuteNonQuery();
                        await ShowCompletionAlertDialogAsync("New User Created Successfully");
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand($"CREATE ROLE \"{CurrentUserName}\" WITH LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE INHERIT NOREPLICATION CONNECTION LIMIT -1 PASSWORD '{CurrentPassword}'; GRANT \"{CurrentAccessType}\" TO \"{CurrentUserName}\";", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand($"CREATE SCHEMA \"{CurrentUserName}\"", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand($"GRANT ALL ON SCHEMA \"{CurrentUserName}\" TO \"{CurrentUserName}\"", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    // Close the connection
                    conn.Close();
                }

                ManageUsersPage.OverlayInstance.SetVisibility(Visibility.Collapsed);

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                await ShowCompletionAlertDialogAsync(error);
            }


        }

        private async Task ShowCompletionAlertDialogAsync(string alert)
        {
            // Create a ContentDialog
            ContentDialog alertDialog = new ContentDialog
            {
                // Set the title, content, and close button text
                Title = "Alert",
                Content = alert,
                CloseButtonText = "OK"
            };

            // Set the foreground to hex color #020066
            alertDialog.Foreground = new SolidColorBrush(Color.FromArgb(255, 2, 0, 102));

            // Set the XamlRoot property to the same as an element in the app window
            // For example, if you have a StackPanel named MyPanel in your XAML
            alertDialog.XamlRoot = OverlayGrid.XamlRoot;

            // Show the ContentDialog and get the result
            ContentDialogResult result = await alertDialog.ShowAsync();

        }

        // This method sets the visibility and raises the event
        public void SetVisibility(Visibility visibility)
        {
            this.Visibility = visibility;
            VisibilityChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                CurrentAccessType = selectedItem.Content.ToString()!;

                // You can now use the selectedValue as needed.
                // For example, display it in a MessageBox or perform other actions.
                // MessageBox.Show($"Selected value: {selectedValue}");
            }
        }

    }
}
