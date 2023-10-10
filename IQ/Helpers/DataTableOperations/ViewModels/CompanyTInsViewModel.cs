using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.TransferInwards;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class CompanyTInsViewModel
    {
        private ObservableCollection<CompanyTIn> _companyTIns;

        public ObservableCollection<CompanyTIn> CompanyTIn
        {
            get { return _companyTIns; }
            set { _companyTIns = value; }
        }

        public CompanyTInsViewModel()
        {
            _companyTIns = new ObservableCollection<CompanyTIn>();
            LoadBranchTInsData();
        }

        private void LoadBranchTInsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanyTInsPage.SelectedView}\".TransferInwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanyTInsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var TIn = new CompanyTIn
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

                            _companyTIns.Add(TIn);
                        }
                    }
                }
            }
        }
    }
}
