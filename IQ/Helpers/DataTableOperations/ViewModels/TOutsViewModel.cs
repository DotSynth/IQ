using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.BranchViews.Pages.TransferOutwards;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchTOutsViewModel
    {
        private ObservableCollection<BranchTOut> _branchTOuts;

        public ObservableCollection<BranchTOut> BranchTOut
        {
            get { return _branchTOuts; }
            set { _branchTOuts = value; }
        }

        public BranchTOutsViewModel()
        {
            _branchTOuts = new ObservableCollection<BranchTOut>();
            LoadBranchTOutsData();
        }

        private void LoadBranchTOutsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM {App.UserName}.TransferOutwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", TransferOutwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var TOut = new BranchTOut
                            {
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredTo = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            _branchTOuts.Add(TOut);
                        }
                    }
                }
            }
        }
    }
}
