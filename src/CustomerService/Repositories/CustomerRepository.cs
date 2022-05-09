using CustomerService.DBContext;
using CustomerService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private BankMgmtDBContext _bankMgmtDBContext;
        private ICustomerRepository _customerRepository;

        public CustomerRepository(BankMgmtDBContext bankMgmtDBContext,ICustomerRepository customerRepository)
        {
            _bankMgmtDBContext = bankMgmtDBContext;
            _customerRepository = customerRepository;
        }

        public async Task<Customer> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _bankMgmtDBContext.Customers.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public async Task<Customer> CreateCustomer(Customer Customer, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (_bankMgmtDBContext.Customers.Any(x => x.Username == Customer.Username))
                //throw new AppException("Username \"" + user.Username + "\" is already taken");
                throw new Exception("Username \"" + Customer.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            Customer.PasswordHash = passwordHash;
            Customer.PasswordSalt = passwordSalt;

            _bankMgmtDBContext.Customers.Add(Customer);
            _bankMgmtDBContext.SaveChanges();

            return Customer;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                passwordSalt = hmac.Key;
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }

            }
            return true;
        }

        public async Task<Customer> GetById(int id)
        {
            return await _bankMgmtDBContext.Customers.FindAsync(id);
        }
    }
}
