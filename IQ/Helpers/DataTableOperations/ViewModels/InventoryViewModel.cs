using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views.BranchViews.Pages.Sales;
using IQ.Views;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IQ.Views.BranchViews.Pages.Inventory;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class BranchInventoryViewModel
    {
        private ObservableCollection<BranchInventory> _branchInventory;

        public ObservableCollection<BranchInventory> BranchInventory
        {
            get { return _branchInventory; }
            set { _branchInventory = value; }
        }

        public BranchInventoryViewModel()
        {
            _branchInventory = new ObservableCollection<BranchInventory>();
            LoadBranchSalesData();
        }

        private void LoadBranchSalesData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM BranchInventory;", connection))
                {                    
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Inventory = new BranchInventory
                            {
                                ModelID = reader.GetString(0),
                                BrandID = reader.GetString(1),
                                AddOns = reader.GetString(2),
                                QuantityInStock = reader.GetInt32(3),
                                UnitPrice = reader.GetDecimal(4),
                                TotalWorth = reader.GetDecimal(5),
                            };

                            _branchInventory.Add(Inventory);
                        }
                    }
                }
            }
        }
    }
}
