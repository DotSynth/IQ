using System;

namespace IQ.Helpers.DataTableOperations.Classes
{
    public class BranchSale
    {
        public string? InvoiceId
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
        public int? QuantitySold
        {
            get; set;
        }
        public Decimal? SellingPrice
        {
            get; set;
        }
        public string? SoldTo
        {
            get; set;
        }
        public string? CustomerContactInfo
        {
            get; set;
        }
    }

}
