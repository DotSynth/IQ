using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.Classes
{
    public class CompanyPurchase
    {
        public string? InvoiceID
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
        public int? QuantityBought
        {
            get; set;
        }
        public Decimal? BuyingPrice
        {
            get; set;
        }
        public string? PurchasedFrom
        {
            get; set;
        }
        public string? SupplierContactInfo
        {
            get; set;
        }
    }
}
