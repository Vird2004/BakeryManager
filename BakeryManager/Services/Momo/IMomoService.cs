using BakeryManager.Models.MoMo;
using BakeryManager.Models;

namespace BakeryManager.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
