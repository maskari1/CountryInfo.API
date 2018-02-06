
namespace CountryInfo.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Http;

    using AutoMapper;

    using CountryInfo.API.Filters;
    using CountryInfo.API.Helpers;
    using CountryInfo.API.Models;
    using CountryInfo.DTO;
    using CountryInfo.Repository;
    using CountryInfo.Repository.Mappers;
    using CountryInfo.API.Routings;
    using Marvin.JsonPatch;

    [RoutePrefix("api")]
    public class CountriesController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICountryInfoRepository _repository;

        public CountriesController(ICountryInfoRepository repo)
        {
            _repository = repo;
            Log.Info("CountriesController is created");
        }

        // [HttpGet, Route("countries/{id}")]
        [VersionedRoute("countries/{id}", 1)]
        public IHttpActionResult Get(int id , bool includePostalCodes = false)
        {
            Log.InfoFormat(@"Getting country with id {0}", id);

            var country = _repository.GetCountry(id, includePostalCodes);
            if (country == null)
            {
                return NotFound();
            }

            if (includePostalCodes)
            {
                var countryResult = Mapper.Map<DTO.CountryWithPostalCodes>(country);
                return Ok(countryResult);
            }

            var countryDto = Mapper.Map<DTO.Country>(country);
            return Ok(countryDto);
        }

        // examples
        // countries/?fields=name,countryid,postalcodes.county
        // countries/?fields=name,countryid,postalcodes

        // api/countries?sort=-name   //descending with -
        public IHttpActionResult Get(string fields = null, string sort = "name")
        {
            Log.InfoFormat(@"Getting all the countries sorted by {0}", sort);

            var includePostalCodes = false;
            var lstOfFields = new List<string>();

            // we should include postalcodes when the fields-string contains "postalcodes", or "postalcodes.name", …
            if (fields != null)
            {
                lstOfFields = fields.ToLower().Split(',').ToList();
                includePostalCodes = lstOfFields.Any(f => f.Contains("postalcodes"));
            }

            var countries = includePostalCodes 
                ? _repository.GetCountriesWithPostalCodes() 
                : _repository.GetCountries();

            return
                Ok(
                    countries.ApplySort(sort)
                        .ToList()
                        .Select(c => c.ToObject(lstOfFields)));
        }

        // usage 
        //[
        //    { "op": "replace", "path": "/name", "value": "New Country" }
        //

        [ValidateFilter]
        [HttpPatch, Route("countries/{id}", Name = "PartialUpdateCountryById")]
        public IHttpActionResult Patch(int id, [FromBody] JsonPatchDocument<DTO.Country> countryPatchDoc)
        {
            var country = _repository.GetCountry(id, false);
            if (country == null)
            {
                return NotFound();
            }

            var countryDto = Mapper.Map<DTO.Country>(country);
            countryPatchDoc.ApplyTo(countryDto);

            var result = _repository.UpdateCountry(countryDto.ToEntity());
            if (result.Status == RepositoryActionStatus.Updated)
            {
                return Ok(Mapper.Map<DTO.Country>(result.Entity));
            }

            return this.BadRequest();
         }

        //[CheckModelForNull]
        [ValidateFilter]
        [HttpPut, Route("countries/{id}", Name = "UpdateCountryById")]
        public IHttpActionResult Put(int id, [FromBody] CountryForManipulationDto country)
        {
            var countryEntity = Mapper.Map<Repository.Entities.Country>(country);
            countryEntity.CountryID = id;

            var result = _repository.UpdateCountry(countryEntity);

            if (result.Status == RepositoryActionStatus.Updated)
            {
                var updatedCountry = Mapper.Map<DTO.Country>(result.Entity);
                return Ok(updatedCountry);
            }

            if (result.Status == RepositoryActionStatus.NotFound)
            {
                return NotFound();
            }

            return BadRequest();
        }

        [ValidateFilter]
        public IHttpActionResult Post([FromBody] CountryForManipulationDto country)
        {
            var countryDto = new Country
                                 {
                                     Name = country.Name,
                                     Abbreviation = country.Abbreviation,
                                     PostalCodeFormat = country.PostalCodeFormat
                                 };

            // var countryEntity = countryDto.ToEntity();    // no automapper
            var countryEntity = Mapper.Map<Repository.Entities.Country>(countryDto);

            var result = _repository.InsertCountry(countryEntity);
            if (result.Status == RepositoryActionStatus.Created)
            {
                var newCountry = countryEntity.ToDTO();
                return Created(Request.RequestUri + "/" + newCountry.CountryId, newCountry);
            }

            return BadRequest();
        }

        [HttpDelete, Route("countries/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var result = _repository.DeleteCountry(id);

            if (result.Status == RepositoryActionStatus.Deleted)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            if (result.Status == RepositoryActionStatus.NotFound)
            {
                return NotFound();
            }

            return BadRequest();
        }
    }
}
