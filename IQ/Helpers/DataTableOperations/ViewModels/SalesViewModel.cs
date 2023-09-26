using IQ.Helpers.FileOperations;
using IQ.Views;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using IQ.Helpers.DataTableOperations.Classes;
using NpgsqlTypes;
using Windows.Networking.Proximity;
using IQ.Views.BranchViews.Pages.Sales;

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
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM BranchSales WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", SalesPage.DateFilter!);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sale = new BranchSale
                            {
                                InvoiceId = reader.GetInt32(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantitySold = reader.GetInt32(3),
                                SellingPrice = reader.GetDouble(4),
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

