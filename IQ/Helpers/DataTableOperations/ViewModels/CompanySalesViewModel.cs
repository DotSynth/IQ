using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.Sales;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class CompanySalesViewModel
    {
        private ObservableCollection<CompanySale> _companySales;

        public ObservableCollection<CompanySale> CompanySales
        {
            get { return _companySales; }
            set { _companySales = value; }
        }

        public CompanySalesViewModel()
        {
            _companySales = new ObservableCollection<CompanySale>();
            LoadBranchSalesData();
        }

        private void LoadBranchSalesData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanySalesPage.SelectedView}\".Sales WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanySalesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sale = new CompanySale
                            {
                                InvoiceId = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantitySold = reader.GetInt32(3),
                                SellingPrice = reader.GetDecimal(4),
                                SoldTo = reader.GetString(5),
                                CustomerContactInfo = reader.GetString(6),
                            };

                            _companySales.Add(sale);
                        }
                    }
                }
            }
        }
    }
}
