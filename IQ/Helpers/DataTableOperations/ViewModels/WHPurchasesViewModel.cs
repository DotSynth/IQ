﻿using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.WarehouseViews.Pages.Purchases;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class WHPurchasesViewModel
    {
        private ObservableCollection<WarehousePurchase> _warehousePurchases;

        public ObservableCollection<WarehousePurchase> WarehousePurchase
        {
            get { return _warehousePurchases; }
            set { _warehousePurchases = value; }
        }

        public WHPurchasesViewModel()
        {
            _warehousePurchases = new ObservableCollection<WarehousePurchase>();
            LoadWarehousePurchasesData();
        }

        private void LoadWarehousePurchasesData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.UserName}\".Purchases WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", PurchasesPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purchase = new WarehousePurchase
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

                            _warehousePurchases.Add(purchase);
                        }
                    }
                }
            }
        }
    }
}