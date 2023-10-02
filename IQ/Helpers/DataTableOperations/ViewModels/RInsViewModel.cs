using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.TransferOutwards;
using IQ.Views;
using Npgsql;
using System.Collections.ObjectModel;
using System;
using System.IO;
using IQ.Views.BranchViews.Pages.ReturnInwards;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchRInsViewModel
    {
        private ObservableCollection<BranchRIn> _branchRIns;

        public ObservableCollection<BranchRIn> BranchRIn
        {
            get { return _branchRIns; }
            set { _branchRIns = value; }
        }

        public BranchRInsViewModel()
        {
            _branchRIns = new ObservableCollection<BranchRIn>();
            LoadBranchRInsData();
        }

        private void LoadBranchRInsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM BranchReturnInwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", ReturnInwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var RIn = new BranchRIn
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedBy = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            _branchRIns.Add(RIn);
                        }
                    }
                }
            }
        }
    }
}
