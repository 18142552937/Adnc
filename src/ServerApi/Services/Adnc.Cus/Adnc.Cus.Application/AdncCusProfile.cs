﻿using AutoMapper;
using Adnc.Application.Shared.Dtos;
using Adnc.Cus.Core.Entities;
using Adnc.Core.Shared;
using Adnc.Cus.Application.Contracts.Dtos;

namespace Adnc.Cus.Application
{
    public class AdncCusProfile : Profile
    {
        public AdncCusProfile()
        {
            CreateMap(typeof(IPagedModel<>), typeof(PageModelDto<>)).ForMember("XData", opt => opt.Ignore());
            CreateMap<CustomerRegisterDto, Customer>();
            CreateMap<Customer, CustomerDto>();
        }
    }
}
