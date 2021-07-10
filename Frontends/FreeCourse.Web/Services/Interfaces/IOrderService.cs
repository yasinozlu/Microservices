using FreeCourse.Web.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IOrderService
    {

        //Senkron iletişim- Order mikroservisine istek gönderikecek.
        Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput);


        //Asenkron iletişim - sipariş bilgileri rabbitMQ ya gönderilecek.
        Task SuspendOrder(CheckoutInfoInput checkoutInfoInput);

        Task<List<OrderViewModel>> GetOrder();
    }
}
