namespace CountryInfo.API.SelfHosted
{
    using System.Web.Http;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            WebApiConfig.Register(configuration);

            app.UseWebApi(configuration);
        }
    }
}
