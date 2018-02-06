
namespace CountryInfo.API.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    public class GenericExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            context.Result = new InternalServerErrorCustomResult(context.Exception, context.Request);
        }
    }

    public class InternalServerErrorCustomResult : IHttpActionResult
    {
        private Exception _exception;

        private HttpRequestMessage _request;

        public InternalServerErrorCustomResult(Exception exception, HttpRequestMessage request)
        {
            this._exception = exception;
            this._request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                               {
                                   RequestMessage = _request,
                                   Content = new StringContent("An error has occurred: " + _exception.Message),
                                   ReasonPhrase = "Embarrassing!"
                               };
            return response;
        }
    }
}