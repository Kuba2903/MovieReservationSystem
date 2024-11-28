namespace WebApp.Services.Implementations
{
    using Data.Models;
    using Stripe;
    using Stripe.Checkout;
    using WebApp.Services.Interfaces;

    public class StripePaymentService : IPaymentService
    {

        private readonly string _apiKey;

        public StripePaymentService()
        {
            _apiKey = "sk_test_51QQ9siJXtk4Cep3wQ0dTFGRGfVTs4n0GIk6LgMAzTuWM7BLftanZ89HD46i7XitPGx5f8GzncQ5ObctUaRLIteeW00IN3bl38L";
            StripeConfiguration.ApiKey = _apiKey;
        }

        public string CreatePaymentSession(List<PaymentItem> items, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Amount * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Description,
                        },
                    },
                    Quantity = 1,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            var session = service.Create(options);
            return session.Url;
        }
    }

}
