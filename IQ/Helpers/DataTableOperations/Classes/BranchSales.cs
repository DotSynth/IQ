using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.Classes
{
    public class BranchSale
    {
        public int? InvoiceId { get; set; }
        public string? ModelID { get; set; }
        public string? BrandID { get; set; }
        public int? QuantitySold { get; set; }
        public float? SellingPrice { get; set; }
        public string? SoldTo { get; set; }
        public string? CustomerContactInfo { get; set; }
        public DateTime SoldOn { get; set; }
    }

}
