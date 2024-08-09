using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics;

namespace AllEvents.TicketManagement.Persistance.Caching
{
    public class QueryHandlerInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<QueryHandlerInterceptor> _logger;
        private readonly TimeSpan _threshold;

        public QueryHandlerInterceptor(ILogger<QueryHandlerInterceptor> logger, TimeSpan threshold)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _threshold = threshold;
        }

        public async Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAndLogIfSlowAsync(command, async () =>
                await base.ReaderExecutingAsync(command, eventData, result, cancellationToken));
        }

        public async Task<InterceptionResult<int>> NonQueryExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAndLogIfSlowAsync(command, async () =>
                await base.NonQueryExecutingAsync(command, eventData, result, cancellationToken));
        }

        public async Task<InterceptionResult<object>> ScalarExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAndLogIfSlowAsync(command, async () =>
                await base.ScalarExecutingAsync(command, eventData, result, cancellationToken));
        }

        private async Task<T> ExecuteAndLogIfSlowAsync<T>(
            DbCommand command,
            Func<Task<T>> execute)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await execute();
            stopwatch.Stop();

            LogIfSlow(command, stopwatch.Elapsed);
            return result;
        }

        private void LogIfSlow(DbCommand command, TimeSpan elapsed)
        {
            if (elapsed > _threshold)
            {
                _logger.LogWarning(
                    "Slow Query Detected: {Elapsed} - {CommandText}",
                    elapsed,
                    command.CommandText);
            }
        }
    }
}
