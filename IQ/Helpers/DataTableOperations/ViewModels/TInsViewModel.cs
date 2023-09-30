using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.Purchases;
using IQ.Views;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IQ.Views.BranchViews.Pages.TransferInwards;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchTInsViewModel
    {
        private ObservableCollection<BranchTIn> _branchTIns;

        public ObservableCollection<BranchTIn> BranchTIn
        {
            get { return _branchTIns; }
            set { _branchTIns = value; }
        }

        public BranchTInsViewModel()
        {
            _branchTIns = new ObservableCollection<BranchTIn>();
            LoadBranchTInsData();
        }

        private void LoadBranchTInsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM BranchTransferInwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", TransferInwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purchase = new BranchTIn
                            {
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredFrom = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            _branchTIns.Add(purchase);
                        }
                    }
                }
            }
        }
    }
}
