﻿using IQ.Helpers.DataTableOperations.Classes;
using IQ.Helpers.FileOperations;
using IQ.Views;
using IQ.Views.AdminViews.Pages.ReturnInwards;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace IQ.Helpers.DataTableOperations.ViewModels
{
    public class CompanyRInsViewModel
    {
        private ObservableCollection<CompanyRIn> _companyRIns;

        public ObservableCollection<CompanyRIn> CompanyRIn
        {
            get { return _companyRIns; }
            set { _companyRIns = value; }
        }

        public CompanyRInsViewModel()
        {
            _companyRIns = new ObservableCollection<CompanyRIn>();
            LoadBranchRInsData();
        }

        private void LoadBranchRInsData()
        {
            string connectionString = App.ConnectionString!;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM \"{CompanyRInsPage.SelectedView}\".ReturnInwards WHERE DATE(Date) = @time;", connection))
                {
                    cmd.Parameters.AddWithValue("time", CompanyRInsPage.DateFilter!.Value.DateTime);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var RIn = new CompanyRIn
                            {
                                ReturnID = reader.GetString(0),
                                ModelID = reader.GetString(1),
                                BrandID = reader.GetString(2),
                                QuantityReturned = reader.GetInt32(3),
                                ReturnedBy = reader.GetString(4),
                                ReasonForReturn = reader.GetString(5),
                                SignedBy = reader.GetString(6),
                            };

                            _companyRIns.Add(RIn);
                        }
                    }
                }
            }
        }
    }
}
