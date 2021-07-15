using FreeCourse.Web.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IOrderService
    {

        //Senkron iletişim- Order mikroservisine istek gönderilecek.
        Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput);


        //Asenkron iletişim - sipariş bilgileri rabbitMQ ya gönderilecek.
        Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput);

        Task<List<OrderViewModel>> GetOrder();
    }
}
