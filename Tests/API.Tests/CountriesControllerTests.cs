
namespace API.Tests
{
    using System;
    using System.Net.Http;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CountriesControllerTests
    {
        [TestMethod]
        public void TestGet()
        {
            var client = new HttpClient();
            var result = client.GetAsync(new Uri("http://localhost:23786/api/countries")).Result;

            var response = result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(response);
            Assert.IsTrue(result.IsSuccessStatusCode);
        }
    }
}
