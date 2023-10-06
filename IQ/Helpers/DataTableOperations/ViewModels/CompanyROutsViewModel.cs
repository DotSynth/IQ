using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.ReturnOutwards;
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
    public class CompanyROutsViewModel
    {
        private ObservableCollection<CompanyROut> _companyROuts;

        public ObservableCollection<CompanyROut> CompanyhROut
        {
            get { return _companyROuts; }
            set { _companyROuts = value; }
        }

        public CompanyROutsViewModel()
        {
            _companyROuts = new ObservableCollection<CompanyROut>();
            LoadBranchROutsData();
        }

        private void LoadBranchROutsData()
        {
            string connectionString = StructureTools.BytesToIQXFile(File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LoginWindow.User))).ConnectionString;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanyROutsPage.SelectedView}\".ReturnOutwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanyROutsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ROut = new CompanyROut
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedTo = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            _companyROuts.Add(ROut);
                        }
                    }
                }
            }
        }
    }
}
