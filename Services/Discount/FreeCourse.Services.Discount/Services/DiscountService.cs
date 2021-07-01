using Dapper;
using FreeCourse.Services.Discount.Dtos;
using FreeCourse.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public DiscountService(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            var status = await _dbConnection.ExecuteAsync("delete from discount where id=@Id", new { Id = id });

            return status > 0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("Discount not found", 404);
        }

        public async Task<Response<List<DiscountDto>>> GetAll()
        {
            var discounts = await _dbConnection.QueryAsync<DiscountDto>("Select * from discount");
            return Response<List<DiscountDto>>.Success(discounts.ToList(),200);
        }

        public async Task<Response<DiscountDto>> GetByCodeAndUserId(string code, string userId)
        {
            var discounts = await _dbConnection.QueryAsync<DiscountDto>("select * from discount where userid=@UserID and code=@Code", new
            {
                UserId=userId,
                Code=code
            });
            var hasDiscount = discounts.FirstOrDefault();

            if (hasDiscount==null)
            {
                return Response<DiscountDto>.Fail("Discount Not Found", 404);

            }
            return Response<DiscountDto>.Success(hasDiscount,200);
        }

        public async Task<Response<DiscountDto>> GetById(int id)
        {
            var discount = (await _dbConnection.QueryAsync<DiscountDto>("select * from discount where id=@Id", new { Id=id })).SingleOrDefault();
            if (discount==null)
            {
                return Response<DiscountDto>.Fail("discount not found", 404);
            }
            return Response<DiscountDto>.Success(discount, 200);
        }

        public async Task<Response<NoContent>> Save(DiscountDto discountDto)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid,rate,code) VALUES(@UserId,@Rate,@Code)",discountDto);

            if (saveStatus>0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail(("An error occurred while adding"), 500);

        }

        public async Task<Response<NoContent>> Update(DiscountDto discountDto)
        {
            var saveStatus = await _dbConnection.ExecuteAsync("update discount set userid=@UserId,code=@Code,rate=@Rate where id=@Id",new{

                    Id=discountDto.Id,
                    UserId=discountDto.UserId,
                    Code=discountDto.Code,
                    Rate=discountDto.Rate

            });

            if (saveStatus > 0)
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail(("Discount not found"), 404);
        }
    }
}
