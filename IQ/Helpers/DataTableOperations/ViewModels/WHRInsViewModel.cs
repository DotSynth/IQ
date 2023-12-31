﻿using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.WarehouseViews.Pages.ReturnInwards;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class WHRInsViewModel
    {
        private ObservableCollection<WarehouseRIn> _warehouseRIns;

        public ObservableCollection<WarehouseRIn> WarehouseRIn
        {
            get { return _warehouseRIns; }
            set { _warehouseRIns = value; }
        }

        public WHRInsViewModel()
        {
            _warehouseRIns = new ObservableCollection<WarehouseRIn>();
            LoadWarehouseRInsData();
        }

        private void LoadWarehouseRInsData()
        {
            string connectionString = App.ConnectionString!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{App.Username}\".ReturnInwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", ReturnInwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var RIn = new WarehouseRIn
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedBy = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            _warehouseRIns.Add(RIn);
                        }
                    }
                }
            }
        }
    }
}
