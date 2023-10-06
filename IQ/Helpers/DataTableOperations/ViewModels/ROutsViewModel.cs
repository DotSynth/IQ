using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.BranchViews.Pages.ReturnOutwards;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchROutsViewModel
    {
        private ObservableCollection<BranchROut> _branchROuts;

        public ObservableCollection<BranchROut> BranchROut
        {
            get { return _branchROuts; }
            set { _branchROuts = value; }
        }

        public BranchROutsViewModel()
        {
            _branchROuts = new ObservableCollection<BranchROut>();
            LoadBranchROutsData();
        }

        private void LoadBranchROutsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.UserName}\".ReturnOutwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", ReturnOutwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ROut = new BranchROut
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedTo = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            _branchROuts.Add(ROut);
                        }
                    }
                }
            }
        }
    }
}
