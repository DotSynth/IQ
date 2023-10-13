using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.BranchViews.Pages.Sales;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{

    public class BranchSalesViewModel
    {
        private ObservableCollection<BranchSale> _branchSales;

        public ObservableCollection<BranchSale> BranchSales
        {
            get { return _branchSales; }
            set { _branchSales = value; }
        }

        public BranchSalesViewModel()
        {
            _branchSales = new ObservableCollection<BranchSale>();
            LoadBranchSalesData();
        }

        private void LoadBranchSalesData()
        {
            string connectionString = App.ConnectionString!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.Username}\".Sales WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", SalesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sale = new BranchSale
                            {
                                InvoiceId = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantitySold = reader.GetInt32(3),
                                SellingPrice = reader.GetDecimal(4),
                                SoldTo = reader.GetString(5),
                                CustomerContactInfo = reader.GetString(6),
                            };

                            _branchSales.Add(sale);
                        }
                    }
                }
            }
        }
    }


}

