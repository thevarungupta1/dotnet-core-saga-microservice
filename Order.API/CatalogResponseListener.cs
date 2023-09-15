using Newtonsoft.Json;
using Order.API.Database;
using Plain.RabbitMQ;
using Shared.Models;

namespace Order.API
{
    public class CatalogResponseListener : IHostedService
    {
        private ISubscriber _subscribe;
        private readonly IServiceScopeFactory _scopeFactory;

        public CatalogResponseListener( ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            _subscribe = subscriber;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscribe.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        public bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<CatalogResponse>(message);
            if (!response.IsSuccess)
            {
                using(var scope = _scopeFactory.CreateScope())
                {
                    var _orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                    // if transaction is not successfull, remove ordering items
                    var orderItem = _orderingContext.OrderItems.Where(o => o.ProductId == response.CatalogId &&
                    o.OrderId == response.OrderId).FirstOrDefault();
                    _orderingContext.OrderItems.Remove(orderItem);
                    _orderingContext.SaveChanges();
                }
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
