using AutoMapper;
using CustomerService.Controllers;
using CustomerService.Entities;
using CustomerService.Helpers;
using CustomerService.Models;
using CustomerService.Repositories;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Test
{
    class CustomerControllerTest
    {
        private CustomerController _customerController;
        private Mock<ICustomerRepository> _customerRepository;
        private Mock<ILoggerManager> _logger;

        private IMapper _mapper;
        
        private  IOptions<AppSettings> _appSettings;

        public Customer _customer;
        public Loan _loan;
        public RegisterModel _registerModel;
        public CustomerControllerTest()
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new AutoMapperProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }

        [SetUp]
        public void Setup()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _logger = new Mock<ILoggerManager>();

            _customer = new Customer() { FirstName = "Manish", Username = "Manish",  ContactNo = 98041234, Email = "mk@gmail.com", Address = "Bangalore", Country = "India", State = "TamilNadu", AccountType = AccountType.Corporate, DOB = Convert.ToDateTime("13-07-1994"), CreatedDate = DateTime.UtcNow, PAN = "DEZo0908" };
            _registerModel = new RegisterModel() { FirstName = "Manish", Username = "Manish", Password = "", ContactNo = 9600209, Email = "mk@gmail.com", Address = "Coimbatore", Country = "India", State = "TamilNadu",  DOB = Convert.ToDateTime("13-07-1994"), CreatedDate = DateTime.UtcNow, PAN = "DEZer456" };
            //_loan = new Loan() { CustomerId = 1, LoanType = "Personal", LoanAmount = 200000, LoanDate = DateTime.UtcNow, LoanDuration = 24, ROI = 7 };
            _appSettings = Options.Create(new AppSettings() { Secret = "secretkeyFWZaVee62ABUQ5d6HcYy33fYtuX9ehdbR3SuL4L" });  ;
            
            
        }

        [Test]
        public void CreateAccount_OkResult()
        {
            // Arrange
            _customerRepository.Setup(x => x.CreateCustomer(_customer, _registerModel.Password));
            var controller = new CustomerController(_customerRepository.Object,_mapper,_appSettings,_logger.Object);

            // Act
            var actionResult = controller.Register(_registerModel);

            // Assert
            
            Assert.IsNotNull(actionResult);
        }

    }
}
