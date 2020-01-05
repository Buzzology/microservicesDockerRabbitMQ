using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using publisher_api.Services;

namespace publisher_api.HealthChecks
{
    public class RabbitMqHealthCheck : IHealthCheck
    {

        private readonly IMessageService _messageService;

        public RabbitMqHealthCheck(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            _messageService.Enqueue("health-check");
            return HealthCheckResult.Healthy();
        }
    }
}