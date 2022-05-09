using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Entities
{
    public class Loan
    {
        public int LoanId { get; set; }
        public string LoanType { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal LoanAmount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal InterestRate { get; set; }
        public int LoanDuration { get; set; }

        [ForeignKey("FK_CustomerId")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime CreatedDtate { get; set; }
        public DateTime UpdatedDtate { get; set; }
        
    }
}
