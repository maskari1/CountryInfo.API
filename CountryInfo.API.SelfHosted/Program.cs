namespace CountryInfo.API.SelfHosted
{
    using System;
    using Microsoft.Owin.Hosting;

    public class Program
    {
        static void Main(string[] args)
        {
            const string Url = "http://localhost:8080";
            using (WebApp.Start<Startup>(Url))
            {
                Console.WriteLine($"Server started at {Url}");
                Console.WriteLine("Press any key to terminate...");
                Console.ReadKey();
            }
        }
    }
}
