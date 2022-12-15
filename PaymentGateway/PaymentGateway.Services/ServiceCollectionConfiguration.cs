using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Repositories;
using PaymentGateway.Services.Validators;
using Polly;
using Polly.Extensions.Http;

namespace PaymentGateway.Services
{
    public static class ServiceCollectionConfiguration
    {
        public static void RegisterPaymentGatewayServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<PaymentDbContext>();
            serviceCollection.AddTransient<PaymentService>();

            serviceCollection.AddSingleton<CardValidator>();
            serviceCollection.AddSingleton<AuthenticationService>();

            serviceCollection.AddHttpClient<BankingService>()
                             .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
