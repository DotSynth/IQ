using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.Classes
{
    public class CompanyInventory
    {
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
        public int? QuantityInStock
        {
            get; set;
        }
        public Decimal? UnitPrice
        {
            get; set;
        }
        public Decimal? TotalWorth
        {
            get; set;
        }
    }
}
