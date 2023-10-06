﻿using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.Inventory;
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
    public class CompanyInventoryViewModel
    {
        private ObservableCollection<CompanyInventory> _companyInventory;

        public ObservableCollection<CompanyInventory> CompanyInventory
        {
            get { return _companyInventory; }
            set { _companyInventory = value; }
        }

        public CompanyInventoryViewModel()
        {
            _companyInventory = new ObservableCollection<CompanyInventory>();
            LoadCompanyInventoryData();
        }

        private void LoadCompanyInventoryData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanyInventoryPage.SelectedView}\".Inventory;", connection))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Inventory = new CompanyInventory
                            {
                                ModelID = reader.GetString(0),
                                BrandID = reader.GetString(1),
                                AddOns = reader.GetString(2),
                                QuantityInStock = reader.GetInt32(3),
                                UnitPrice = reader.GetDecimal(4),
                                TotalWorth = reader.GetDecimal(5),
                            };

                            _companyInventory.Add(Inventory);
                        }
                    }
                }
            }
        }
    }
}
