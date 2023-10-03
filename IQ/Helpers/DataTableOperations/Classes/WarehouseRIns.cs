using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Helpers.DataTableOperations.Classes
{
    class WarehouseRIn
    {
        public string? ReturnID
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
        public int? QuantityReturned
        {
            get; set;
        }
        public string? ReturnedBy
        {
            get; set;
        }
        public string? ReasonForReturn
        {
            get; set;
        }
        public string? SignedBy
        {
            get; set;
        }
    }
}
