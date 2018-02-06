namespace CountryInfo.API
{
    using System.Web.Http;
    using Microsoft.Practices.Unity;
    using Newtonsoft.Json.Serialization;
    using System.Data.Entity;
    using System.Net.Http.Headers;
    using System.Web.Http.ExceptionHandling;

    using CountryInfo.API.Formatters;
    using CountryInfo.API.MessageHandler;
    using CountryInfo.API.Services;
    using CountryInfo.Repository;
    using CountryInfo.Repository.Entities;


    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Create a new Unity dependency injection container
            var container = new UnityContainer();

            // Register dependencies
            container.RegisterType<ICountryInfoRepository, CountryInfoEFRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<DbContext, CountryInfoContext>(new HierarchicalLifetimeManager(), new InjectionConstructor());

            // override the default dependency resolver with container
            config.DependencyResolver = new UnityResolver(container);

            // tracing: Install-Package Microsoft.AspNet.WebApi.Tracing
            config.EnableSystemDiagnosticsTracing();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            //config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(
                new MediaTypeHeaderValue("application/json-patch+json"));

            config.Formatters.JsonFormatter.SerializerSettings.Formatting
                = Newtonsoft.Json.Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();

            // configure caching
            config.MessageHandlers.Add(new CacheCow.Server.CachingHandler(config));

            // configure formatter
            config.Formatters.Add(new CountryCsvFormatter());

            // configure exception handler and logger
            config.Services.Replace(typeof(IExceptionHandler), new GenericExceptionHandler());
            config.Services.Replace(typeof(IExceptionLogger), new Log4NetExceptionLogger());

            config.MessageHandlers.Add(new ExecutionTimeLoggerMessageHandler());

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Country, DTO.Country>();
                cfg.CreateMap<Country, DTO.CountryWithPostalCodes>();
                cfg.CreateMap<DTO.Country, Country>();
            });
        }
    }
}
