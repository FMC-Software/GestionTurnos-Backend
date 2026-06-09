using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestionTurnos.Infrastructure.BackgroundServices
{
    public class SubscriptionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SubscriptionWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 3. Usamos el provider para crear un "Scope" (alcance) temporal
                using (var scope = _serviceProvider.CreateScope())
                {
                    var processor = scope.ServiceProvider.GetRequiredService<SubscriptionProcessor>();
                    await processor.ExecuteAsync();
                }

                // Esperar 24 horas (puedes bajarlo a 1 minuto para hacer pruebas)
                await Task.Delay(TimeSpan.FromHours(0.1), stoppingToken);
            }
        }
    }
}