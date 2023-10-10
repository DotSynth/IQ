﻿using System;

namespace IQ.Helpers.DataTableOperations.Classes
{
    public class CompanyTOut
    {
        public string? TransferID
        {
            get; set;
        }
        public string? ModelID
        {
            get; set;
        }
        public string? BrandID
        {
            get; set;
        }
        public string? AddOns
        {
            get; set;
        }
        public int? QuantityTransferred
        {
            get; set;
        }
        public string? TransferredTo
        {
            get; set;
        }
        public string? SignedBy
        {
            get; set;
        }
        public Decimal? TransferredProductPrice
        {
            get; set;
        }
    }
}