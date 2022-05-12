﻿using AutoMapper;
using CustomerService.Entities;
using CustomerService.Helpers;
using CustomerService.Models;
using CustomerService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerRepository _customerRepository;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CustomerController(ICustomerRepository customerRepository, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            var user = await _customerRepository.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.CustomerId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.CustomerId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<Customer>(model);

            try
            {
                // create user
                await _customerRepository.CreateCustomer(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _customerRepository.GetById(id);
            var model = _mapper.Map<RegisterModel>(user);
            return Ok(model);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModel model)
        {
            // map model to entity and set id
            var user = _mapper.Map<Customer>(model);
            user.CustomerId = id;

            try
            {
                // update user 
               await  _customerRepository.Update(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        [Route("applyLoan")]
        public async Task<IActionResult> ApplyLoan([FromBody] LoanModel model)
        {
            // map model to entity
            var loan = _mapper.Map<Loan>(model);

            try
            {
                // create user
                await _customerRepository.ApplyLoan(loan);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        [Route("applyLoan/{customerId}")]
        
        public async Task<IEnumerable<Loan>> ViewLoanByCustomerId(int id)
        {
            var loan = await _customerRepository.ViewLoanByCustomerId(id);
            IList<Loan> model = _mapper.Map<IList<Loan>>(loan);
            return model;

        }
        [HttpGet("{customerId}/{loanId}")]
        public async Task<IActionResult> ViewLoanWithCustAndLoanId(int id)
        {
            var user = await _customerRepository.GetById(id);
            var model = _mapper.Map<RegisterModel>(user);
            return Ok(model);
        }
    }
}
