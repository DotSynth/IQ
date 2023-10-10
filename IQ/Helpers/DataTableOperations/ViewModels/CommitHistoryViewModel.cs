using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchCommitsViewModel
    {
        private ObservableCollection<BranchCommits> _branchCommits;

        public ObservableCollection<BranchCommits> BranchCommit
        {
            get { return _branchCommits; }
            set { _branchCommits = value; }
        }

        public BranchCommitsViewModel()
        {
            _branchCommits = new ObservableCollection<BranchCommits>();
            LoadBranchCommitsData();
        }

        private void LoadBranchCommitsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.UserName}\".CommitHistory;", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Commit = new BranchCommits
                            {
                                CommitID = reader.GetString(0),
                                CommitDate = reader.GetDateTime(1),
                            };

                            _branchCommits.Add(Commit);
                        }
                    }
                }
            }
        }
    }
}
