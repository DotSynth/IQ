using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.TransferOutwards;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class CompanyTOutsViewModel
    {
        private ObservableCollection<CompanyTOut> _companyTOuts;

        public ObservableCollection<CompanyTOut> CompanyTOut
        {
            get { return _companyTOuts; }
            set { _companyTOuts = value; }
        }

        public CompanyTOutsViewModel()
        {
            _companyTOuts = new ObservableCollection<CompanyTOut>();
            LoadBranchTOutsData();
        }

        private void LoadBranchTOutsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanyTOutsPage.SelectedView}\".TransferOutwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanyTOutsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var TOut = new CompanyTOut
                            {
                                TransferID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                AddOns = reader.GetString(3),
                                QuantityTransferred = reader.GetInt32(4),
                                TransferredTo = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                                TransferredProductPrice = reader.GetDecimal(7),
                            };

                            _companyTOuts.Add(TOut);
                        }
                    }
                }
            }
        }
    }
}
