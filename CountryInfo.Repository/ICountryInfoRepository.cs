namespace CountryInfo.Repository
{
    using System.Linq;
    using CountryInfo.Repository.Entities;

    public interface ICountryInfoRepository
    {
        Country GetCountry(int id, bool includePostalCodes);
        IQueryable<Country> GetCountries();
        IQueryable<Country> GetCountriesWithPostalCodes();
        RepositoryActionResult<Country> UpdateCountry(Country country);
        RepositoryActionResult<Country> InsertCountry(Country country);
        RepositoryActionResult<Country> DeleteCountry(int id);
        Entities.AreaPostalCode GetPostalCode(int id);
        IQueryable<AreaPostalCode> GetPostalCodes();
        IQueryable<AreaPostalCode> GetPostalCodes(int countryId);
        RepositoryActionResult<AreaPostalCode> UpdatePostalCode(AreaPostalCode postalCode);
        RepositoryActionResult<AreaPostalCode> InsertPostalCode(AreaPostalCode postalCode);
        RepositoryActionResult<AreaPostalCode> DeletePostalCode(int id);
    }
}
