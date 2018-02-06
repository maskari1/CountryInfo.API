using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Repository.Tests
{
    using System;
    using System.Linq;

    using CountryInfo.Repository;
    using CountryInfo.Repository.Entities;

    using Z.EntityFramework.Plus;

    [TestClass]
    public class CountryInfoRepositoryTests
    {
        [TestMethod]
        public void TestGetCountries()
        {
            var repo = new CountryInfoEFRepository(new CountryInfoContext());
            var countries = repo.GetCountries().ToList();
            Assert.IsTrue(countries.Count() > 0);
        }

        [TestMethod]
        public void TestGetCountriesWithPostalCodes()
        {
            var repo = new CountryInfoEFRepository(new CountryInfoContext());
            var countries = repo.GetCountriesWithPostalCodes().IncludeFilter(p => p.PostalCodes.Take(10)).ToList();
            Assert.IsTrue(countries.Count > 0);
        }
        
        [TestMethod]
        public void TestInsertCountry()
        {
            var repo = new CountryInfoEFRepository(new CountryInfoContext());

            var result = repo.InsertCountry(
                new Country
                    {
                        Name = "Test",
                        Abbreviation = "T"
                });

            Assert.IsTrue(result.Status == RepositoryActionStatus.Created);
        }

        [TestMethod]
        public void TestDeleteCountry()
        {
            var repo = new CountryInfoEFRepository(new CountryInfoContext());
            var result = repo.DeleteCountry(8);

            Assert.IsTrue(result.Status == RepositoryActionStatus.Deleted);
        }

        [TestMethod]
        public void TestInsertPostalCode()
        {
            var repo = new CountryInfoEFRepository(new CountryInfoContext());

            var result = repo.InsertPostalCode(
                new AreaPostalCode
                {
                   PostalCode = "666666666",
                   City = "Nice City",
                   PreferredCity = "Nice City",
                   CountryID = 16,
                   StateAbbrev = "NS",
                   County = "Nice Country",
                   StateCode = "Nice State"
                });

            Assert.IsTrue(result.Status == RepositoryActionStatus.Created);
        }
    }
}
