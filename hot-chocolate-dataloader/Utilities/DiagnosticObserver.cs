using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HotChocolate.Examples.Paging
{
    public class DiagnosticObserver : IDiagnosticObserver
    {
        private readonly ILogger _logger;

        public DiagnosticObserver(ILogger<DiagnosticObserver> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [DiagnosticName("HotChocolate.Execution.Query")]
        public void OnQuery(IQueryContext context)
        {
            _logger.LogInformation("------ OnQuery ------");
            // This method is used as marker to enable begin and end events
            // in the case that you want to explicitly track the start and the
            // end of this event.
        }

        [DiagnosticName("HotChocolate.Execution.Query.Start")]
        public void BeginQueryExecute(IQueryContext context)
        {
            _logger.LogInformation("------ Start BeginQueryExecute ------");
            _logger.LogInformation(context.Request.Query.ToString());
            _logger.LogInformation("------ End BeginQueryExecute ------");
        }

        [DiagnosticName("HotChocolate.Execution.Query.Stop")]
        public void EndQueryExecute(IQueryContext context)
        {
            _logger.LogInformation("------ Start BeginQueryExecute ------");

            if (context.Result is IReadOnlyQueryResult result)
            {
                using (var stream = new MemoryStream())
                {
                    var resultSerializer = new JsonQueryResultSerializer();
                    resultSerializer.SerializeAsync(result, stream).Wait();

                    var streamArray = stream.ToArray();
                    _logger.LogInformation(Encoding.UTF8.GetString(streamArray));
                }
            }

            _logger.LogInformation("------ End BeginQueryExecute ------");
        }


        [DiagnosticName("HotChocolate.Execution.Resolver.Error")]
        public void OnResolverError(IResolverContext context, IEnumerable<IError> errors)
        {
            _logger.LogInformation("------ Start OnResolverError ------");

            foreach(var error in errors)
            {
                _logger.LogInformation($"{error.Code}: {error.Message}. {error?.Exception?.Message} {error?.Exception?.StackTrace}");
            }

            _logger.LogInformation("------ End OnResolverError ------");
        }
    }
}