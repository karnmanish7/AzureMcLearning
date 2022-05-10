using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Entities
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }
        public string LoanType { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal LoanAmount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal InterestRate { get; set; }
        public int LoanDuration { get; set; }

        
        

        // Foreign key 
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customers { get; set; }

        public DateTime CreatedDtate { get; set; }
        public DateTime UpdatedDtate { get; set; }
        
    }
}
