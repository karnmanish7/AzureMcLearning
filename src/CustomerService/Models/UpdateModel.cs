using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    public class UpdateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public int Pincode { get; set; }
        public string Country { get; set; }
        public string PAN { get; set; }
        public string Email { get; set; }
        public int ContactNo { get; set; }
        public DateTime DOB { get; set; }
       
        public DateTime CreatedDtate { get; set; }
        public DateTime UpdatedDtate { get; set; }

    }
}
