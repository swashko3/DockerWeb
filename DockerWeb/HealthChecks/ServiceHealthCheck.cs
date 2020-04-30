using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DockerWeb.HealthChecks
{
    public class ServiceHealthCheck : IHealthCheck
    {
        private string Url { get; set; }

        public ServiceHealthCheck(string url)
        {
            Url = url;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var uri = new Uri(Url);
                    var response = await client.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"{Url} Service not responding with 200 OK");
                    }
                }
                catch (Exception ex)
                {
                    return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
                }
            }
            return HealthCheckResult.Healthy();
        }
    }
}