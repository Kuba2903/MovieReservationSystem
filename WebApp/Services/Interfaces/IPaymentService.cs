using Data.Models;

namespace WebApp.Services.Interfaces
{
    public interface IPaymentService
    {
        string CreatePaymentSession(List<PaymentItem> items, string successUrl, string cancelUrl);
    }
}
