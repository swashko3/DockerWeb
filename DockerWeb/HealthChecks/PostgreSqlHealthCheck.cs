using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace DockerWeb.HealthChecks
{
    public class PostgreSqlHealthCheck: IHealthCheck
    {
        private static readonly string DefaultQuery = "SELECT 1";
        string Connection { get; set; }
        public string TestQuery { get; set; }


        public PostgreSqlHealthCheck(string connection) : this(connection, testQuery: DefaultQuery)
        {
        }

        public PostgreSqlHealthCheck(string connection, string testQuery)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            TestQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using(var conn = new NpgsqlConnection(Connection))
            {
                try
                {
                    await conn.OpenAsync(cancellationToken);

                    if (TestQuery!= null)
                    {
                        var cmd = conn.CreateCommand();
                        cmd.CommandText = TestQuery;

                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (DbException ex)
                {
                    return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
                }

                return HealthCheckResult.Healthy();
            }
        }
    }
}
