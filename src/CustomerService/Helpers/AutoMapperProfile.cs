using AutoMapper;
using CustomerService.Entities;
using CustomerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Helpers
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel, Customer>();
            CreateMap<UpdateModel, Customer>();
        }
        
    }
}
