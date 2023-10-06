using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class WHInventoryViewModel
    {
        private ObservableCollection<WarehouseInventory> _warehouseInventory;

        public ObservableCollection<WarehouseInventory> WarehouseInventory
        {
            get { return _warehouseInventory; }
            set { _warehouseInventory = value; }
        }

        public WHInventoryViewModel()
        {
            _warehouseInventory = new ObservableCollection<WarehouseInventory>();
            LoadWarehouseInventoryData();
        }

        private void LoadWarehouseInventoryData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.UserName}\".Inventory;", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Inventory = new WarehouseInventory
                            {
                                ModelID = reader.GetString(0),
                                BrandID = reader.GetString(1),
                                AddOns = reader.GetString(2),
                                QuantityInStock = reader.GetInt32(3),
                                UnitPrice = reader.GetDecimal(4),
                                TotalWorth = reader.GetDecimal(5),
                            };

                            _warehouseInventory.Add(Inventory);
                        }
                    }
                }
            }
        }
    }
}
