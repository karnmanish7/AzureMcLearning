using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    public class LoanModel
    {
        public string LoanType { get; set; }
        
        public decimal LoanAmount { get; set; }
        
        public decimal InterestRate { get; set; }
        public int LoanDuration { get; set; }
    }
}
