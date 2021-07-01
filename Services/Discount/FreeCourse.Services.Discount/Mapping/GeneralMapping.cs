using AutoMapper;
using FreeCourse.Services.Discount.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Models.Discount, DiscountDto>().ReverseMap();
        }
    }
}
