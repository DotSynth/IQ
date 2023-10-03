using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.WarehouseViews.Pages.TransferInwards;
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
    class WHTInsViewModel
    {
        private ObservableCollection<WarehouseTIn> _warehouseTIns;

        public ObservableCollection<WarehouseTIn> WarehouseTIn
        {
            get { return _warehouseTIns; }
            set { _warehouseTIns = value; }
        }

        public WHTInsViewModel()
        {
            _warehouseTIns = new ObservableCollection<WarehouseTIn>();
            LoadWarehouseTInsData();
        }

        private void LoadWarehouseTInsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM WarehouseTransferInwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", TransferInwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var TIn = new WarehouseTIn
                            {
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredFrom = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            _warehouseTIns.Add(TIn);
                        }
                    }
                }
            }
        }
    }
}
