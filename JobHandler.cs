using Dapper;
using Hangfire;
using HangfireJobHandler.Attributes;
using Serilog;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HangfireJobHandler
{
    [Queue("handler")]
    public class JobHandler : IJobHandler
    {
        private readonly ILogger _logger;

        public JobHandler(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<bool> TryEnqueueJobAsync(string jobId)
        {
            string query = $@"SELECT COUNT(1)
                FROM [{Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA")}].[{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}]
                WHERE JobId = '{jobId}'";
            using (var connection = new SqlConnection($"{Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING")};database={Environment.GetEnvironmentVariable("HANGFIRE_DATABASE")};"))
            {
                int count = await connection.ExecuteScalarAsync<int>(query);
                if(count == 0)
                {
                    string command = $@"INSERT INTO [{Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA")}].[{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}] (JobId)
                    VALUES ('{jobId}')";
                    await connection.ExecuteAsync(command);
                    return true;
                }
            }
            _logger.Information($"Skipping duplicate job: {jobId}.");
            return false;
        }

        [ContinuationsSupportIncludingFailedState]
        public async Task DeleteJobFromQueueAsync(string jobId)
        {
            string command = $@"DELETE FROM [{Environment.GetEnvironmentVariable("HANGFIRE_SCHEMA")}].[{Environment.GetEnvironmentVariable("HANGFIRE_JOB_TABLE")}]
                                WHERE JobId = '{jobId}'";
            using (var connection = new SqlConnection($"{Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING")};database={Environment.GetEnvironmentVariable("HANGFIRE_DATABASE")};"))
            {
                await connection.ExecuteAsync(command);
            }
        }
    }
}
