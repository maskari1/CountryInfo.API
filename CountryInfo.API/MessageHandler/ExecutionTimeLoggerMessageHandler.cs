
namespace CountryInfo.API.MessageHandler
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Tracing;

    public class ExecutionTimeLoggerMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = await base.SendAsync(request, cancellationToken);
            stopWatch.Stop();

            // add execution time to the response header
            response.Headers.Add("X-ExecutionTime", stopWatch.ElapsedMilliseconds.ToString());

            // and write it to the trace
            request.GetConfiguration()
                .Services.GetTraceWriter()
                .Info(request, "Timing", request.RequestUri.ToString(), stopWatch.ElapsedMilliseconds);

            return response;
        }
    }
}