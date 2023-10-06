using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.Classes
{
    public class CompanyTIn
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
        public string? TransferredFrom
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
