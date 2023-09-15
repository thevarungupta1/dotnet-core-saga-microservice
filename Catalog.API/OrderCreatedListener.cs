using Catalog.API.Database;
using Catalog.API.Models;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using Shared.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API
{
    public class OrderCreatedListener : IHostedService
    {
        private readonly IPublisher _publisher;
        private ISubscriber _subscriber;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderCreatedListener(IPublisher publisher, ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            _publisher = publisher;
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> headers) 
        {
            var response = JsonConvert.DeserializeObject<OrderRequest>(message);
            using (var scope = _scopeFactory.CreateScope()) 
            {
                var _context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
                try
                {
                    CatalogItem catalogItem = _context.CatalogItems.Find(response.CatalogId);
                    if(catalogItem == null || catalogItem.AvaliableStock < response.Units) 
                       throw new Exception();
                    catalogItem.AvaliableStock = catalogItem.AvaliableStock - response.Units;
                    _context.Entry(catalogItem).State = EntityState.Modified;
                    _context.SaveChanges();

                    _publisher.Publish(JsonConvert.SerializeObject(
                        new CatalogResponse { OrderId = response.OrderId, CatalogId = response.CatalogId, IsSuccess = true }
                        ), "catalog_response_routingkey", null);
                    
                }catch(Exception ex)
                {

                }
                return true;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
