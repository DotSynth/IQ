using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.WarehouseViews.Pages.ReturnOutwards;
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
    public class WHROutsViewModel
    {
        private ObservableCollection<WarehouseROut> _warehouseROuts;

        public ObservableCollection<WarehouseROut> WarehouseROut
        {
            get { return _warehouseROuts; }
            set { _warehouseROuts = value; }
        }

        public WHROutsViewModel()
        {
            _warehouseROuts = new ObservableCollection<WarehouseROut>();
            LoadWarehouseROutsData();
        }

        private void LoadWarehouseROutsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM {App.UserName}.ReturnOutwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", ReturnOutwardsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ROut = new WarehouseROut
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedTo = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            _warehouseROuts.Add(ROut);
                        }
                    }
                }
            }
        }
    }
}
