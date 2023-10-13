using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.BranchViews.Pages.Purchases;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchPurchasesViewModel
    {
        private ObservableCollection<BranchPurchase> _branchPurchases;

        public ObservableCollection<BranchPurchase> BranchPurchase
        {
            get { return _branchPurchases; }
            set { _branchPurchases = value; }
        }

        public BranchPurchasesViewModel()
        {
            _branchPurchases = new ObservableCollection<BranchPurchase>();
            LoadBranchPurchasesData();
        }

        private void LoadBranchPurchasesData()
        {
            string connectionString = App.ConnectionString!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.Username}\".Purchases WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", PurchasesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purchase = new BranchPurchase
                            {
                                InvoiceID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityBought = reader.GetInt32(4),
                                BuyingPrice = reader.GetDecimal(5),
                                PurchasedFrom = reader.GetString(6),
                                SupplierContactInfo = reader.GetString(7),
                            };

                            _branchPurchases.Add(purchase);
                        }
                    }
                }
            }
        }
    }
}
