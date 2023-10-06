using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class UsersViewModel
    {
        private ObservableCollection<UserLogins> _userLogin;

        public ObservableCollection<UserLogins> UserLogin
        {
            get { return _userLogin; }
            set { _userLogin = value; }
        }

        public UsersViewModel()
        {
            _userLogin = new ObservableCollection<UserLogins>();
            LoaduserLoginData();
        }

        private void LoaduserLoginData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.UserName}\".UserLogins;", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var userLogin = new UserLogins
                            {
                                Username = reader.GetString(0),
                                Password = reader.GetString(1),
                                AccessType = reader.GetString(2),
                            };

                            _userLogin.Add(userLogin);
                        }
                    }
                }
            }
        }
    }
}
