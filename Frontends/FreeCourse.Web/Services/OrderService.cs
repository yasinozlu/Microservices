using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Models.Payments;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentService _paymentService;
        private readonly HttpClient _httpClient;
        private readonly IBasketService _basketService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _paymentService = paymentService;
            _httpClient = httpClient;
            _basketService = basketService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();
            var paymentInfoInput = new FakePaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice,
            };

            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderCreatedViewModel() { Error = "Ödeme alınırken hata oluştu", IsSuccessfull = false };
            }

            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput { Province = checkoutInfoInput.Province,District =checkoutInfoInput.District, Street = checkoutInfoInput.Street , Line =checkoutInfoInput.Line,
                ZipCode = checkoutInfoInput.ZipCode},
            };

            basket.BasketItems.ForEach(x =>
            {
                var orderItems = new OrderItemCreateInput { ProductId = x.CourseId, Price = x.Price, PictureUrl = "", ProductName = x.CourseName };
                orderCreateInput.OrderItems.Add(orderItems);
            });

            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders",orderCreateInput);

            if (!response.IsSuccessStatusCode)
            {
                return new OrderCreatedViewModel() { Error = "Sipariş oluşturulurken hata oluştu", IsSuccessfull = false };
            }

            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();

            orderCreatedViewModel.Data.IsSuccessfull = true;
            await _basketService.Delete();
            return orderCreatedViewModel.Data;


        }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");

            return response.Data;
        }

        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Street = checkoutInfoInput.Street,
                    Line = checkoutInfoInput.Line,
                    ZipCode = checkoutInfoInput.ZipCode
                },
            };

            basket.BasketItems.ForEach(x =>
            {
                var orderItems = new OrderItemCreateInput { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };
                orderCreateInput.OrderItems.Add(orderItems);
            });


            var paymentInfoInput = new FakePaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice,
                Order = orderCreateInput,
            };

            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderSuspendViewModel() { Error = "Ödeme alınırken hata oluştu", IsSuccessfull = false };
            }
            await _basketService.Delete();
            return new OrderSuspendViewModel() { IsSuccessfull = true };
        }
    }
}
