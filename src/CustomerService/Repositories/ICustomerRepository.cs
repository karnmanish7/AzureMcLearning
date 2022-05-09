using CustomerService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Repositories
{
    public interface ICustomerRepository
    {
        
        Task<Customer> CreateCustomer(Customer user, string password);
        Task<Customer> Authenticate(string username, string password);
        Task<Customer> GetById(int id);

    }
}
