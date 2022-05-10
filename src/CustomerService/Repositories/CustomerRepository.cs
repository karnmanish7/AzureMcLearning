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
      

        public CustomerRepository(BankMgmtDBContext bankMgmtDBContext)
        {
            _bankMgmtDBContext = bankMgmtDBContext;
       
        }

        public async Task<Customer> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _bankMgmtDBContext.Customers.SingleOrDefaultAsync(x => x.Username == username);

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

            await _bankMgmtDBContext.Customers.AddAsync(Customer);
            await _bankMgmtDBContext.SaveChangesAsync();

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

        public async Task Update(Customer userParam, string password = null)
        {
            var user = await _bankMgmtDBContext.Customers.FindAsync(userParam.Id);

            if (user == null)
                throw new Exception("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (await _bankMgmtDBContext.Customers.AnyAsync(x => x.Username == userParam.Username))
                    throw new Exception("Username " + userParam.Username + " is already taken");

                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;
            if (!string.IsNullOrWhiteSpace(userParam.Address))
                user.Address = userParam.Address;
            if (userParam.ContactNo !=0 )
                user.ContactNo = userParam.ContactNo;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

             _bankMgmtDBContext.Customers.Update(user);
            await _bankMgmtDBContext.SaveChangesAsync();
        }

        public async Task<Loan> ApplyLoan(Loan loan)
        {
            try
            {
                await _bankMgmtDBContext.Loans.AddAsync(loan);
                await _bankMgmtDBContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
            

            return loan;
        }
        public async Task<IEnumerable<Loan>> ViewLoanByCustomerId(int customerId)
        {
            IList<Loan> list= await _bankMgmtDBContext.Loans.Where(c => c.CustomerId == customerId).ToListAsync();
            return list;
        }

    }
}
