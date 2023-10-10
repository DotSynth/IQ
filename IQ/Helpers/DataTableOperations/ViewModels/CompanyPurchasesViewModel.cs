using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.Purchases;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class CompanyPurchasesViewModel
    {
        private ObservableCollection<CompanyPurchase> _companyPurchases;

        public ObservableCollection<CompanyPurchase> CompanyPurchase
        {
            get { return _companyPurchases; }
            set { _companyPurchases = value; }
        }

        public CompanyPurchasesViewModel()
        {
            _companyPurchases = new ObservableCollection<CompanyPurchase>();
            LoadBranchPurchasesData();
        }

        private void LoadBranchPurchasesData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanyPurchasesPage.SelectedView}\".Purchases WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanyPurchasesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purchase = new CompanyPurchase
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

                            _companyPurchases.Add(purchase);
                        }
                    }
                }
            }
        }
    }
}
