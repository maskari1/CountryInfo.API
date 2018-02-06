
using AutoMapper;

namespace CountryInfo.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using WebApi.OutputCache.V2;

    using CountryInfo.API.Filters;
    using CountryInfo.API.Models;
    using CountryInfo.API.Routings;
    using CountryInfo.DTO;
    using CountryInfo.Repository;
    using CountryInfo.Repository.Mappers;

    using Marvin.JsonPatch;

    [RoutePrefix("api")]
    public class PostalCodesController : ApiController
    {
        private const int MaxPageSize = 10;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICountryInfoRepository _repository;

        public PostalCodesController(ICountryInfoRepository repo)
        {
            _repository = repo;
            Log.Info("PostalCodesController is created");
        }

        [VersionedRoute("countries/{countryId}/postalcodes/{id}", 1)]
        [VersionedRoute("postalcodes/{id}", 1)]
        public IHttpActionResult Get(int id, int? countryId = null)
        {
            Log.InfoFormat($"Version1: Getting postal code with id {id}");

            Repository.Entities.AreaPostalCode postalCode = null;

            if (countryId == null)
            {
                postalCode = _repository.GetPostalCode(id);
            }
            else
            {
                var postalCodes = _repository.GetPostalCodes((int)countryId);

                if (postalCodes != null)
                {
                    postalCode = postalCodes.FirstOrDefault(pc => pc.PostalCodeID == id);
                }
            }

            if (postalCode != null)
            {
                var returnValue = postalCode.ToDTO();
                return Ok(returnValue);
            }

            return NotFound();
        }

        [CacheOutput(ClientTimeSpan = 30)]   // corresponds to CacheControl max-age HTTP header in response
        [VersionedRoute("countries/{countryId}/postalcodes/{id}", 2)]
        [VersionedRoute("postalcodes/{id}", 2)]
        public IHttpActionResult GetV2(int id, int? countryId = null, string fields = null)
        {
            Log.InfoFormat($"Version2: Getting postal code with id {id}");

            var lstOfFields = new List<string>();

            if (fields != null)
            {
                lstOfFields = fields.ToLower().Split(',').ToList();
            }

            Repository.Entities.AreaPostalCode postalCode = null;

            if (countryId == null)
            {
                postalCode = _repository.GetPostalCode(id);
            }
            else
            {
                var postalCodes = _repository.GetPostalCodes((int)countryId);

                if (postalCodes != null)
                {
                    postalCode = postalCodes.FirstOrDefault(pc => pc.PostalCodeID == id);
                }
            }

            if (postalCode != null)
            {
                var returnValue = postalCode.ToObject(lstOfFields);
                return Ok(returnValue);
            }

            return NotFound();
        }

        // Data shaping: countries/1/postalcodes?fields=city,stateAbbrev,statecode
        // countries/1/postalcodes?page=200
        [Route("countries/{countryId}/postalcodes", Name = "PostalCodesForCountry")]
        public IHttpActionResult Get(
            int countryId,
            string fields = null,
            string sort = "StateAbbrev",
            int page = 1,
            int pageSize = MaxPageSize)
        {
            var postalCodes = _repository.GetPostalCodes(countryId);
            if (postalCodes == null)
            {
                return NotFound();
            }

            var lstOfFields = new List<string>();

            if (fields != null)
            {
                lstOfFields = fields.ToLower().Split(',').ToList();
            }

            // ensure the page size isn't larger than the maximum.
            if (pageSize > MaxPageSize)
            {
                pageSize = MaxPageSize;
            }

            // calculate data for metadata
            var totalCount = postalCodes.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var urlHelper = new UrlHelper(Request);

            var prevLink = page > 1
                                ? urlHelper.Link(
                                    "PostalCodesForCountry",
                                    new { page = page - 1, pageSize, countryId, fields, sort })
                                : string.Empty;

            var nextLink = page < totalPages
                                ? urlHelper.Link(
                                    "PostalCodesForCountry",
                                    new { page = page + 1, pageSize, countryId, fields, sort })
                                : string.Empty;

            var paginationHeader =
                new
                    {
                        currentPage = page,
                        pageSize,
                        totalCount,
                        totalPages,
                        previousPageLink = prevLink,
                        nextPageLink = nextLink
                    };

            HttpContext.Current.Response.Headers.Add(
                "X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

            return
                Ok(
                    postalCodes.OrderBy(sort)
                        .Skip(pageSize * (page - 1))
                        .Take(pageSize)
                        .ToList()
                        .Select(p => p.ToObject(lstOfFields)));
        }

        [ValidateFilter]
        public IHttpActionResult Put(int id, [FromBody] DTO.AreaPostalCode postalCode)
        {
            var postalCodeEntity = postalCode.ToEntity();
            postalCodeEntity.PostalCodeID = id;

            var result = _repository.UpdatePostalCode(postalCodeEntity);
            if (result.Status == RepositoryActionStatus.Updated)
            {
                return Ok(result.Entity.ToDTO());
            }

            if (result.Status == RepositoryActionStatus.NotFound)
            {
                return NotFound();
            }

            return BadRequest();
        }

        [ValidateFilter]
        public IHttpActionResult Post([FromBody] AreaPostalCodeForCreationDto postalCode)
        {
            var postalcodeDto = new AreaPostalCode {
                PostalCode = postalCode.PostalCode,
                City = postalCode.City,
                PreferredCity = postalCode.PreferredCity,
                StateAbbrev = postalCode.StateAbbrev,
                County = postalCode.County,
                SourceType = postalCode.SourceType,
                StateCode = postalCode.StateCode,
                CountryId = postalCode.CountryId
            };

            var result = _repository.InsertPostalCode(postalcodeDto.ToEntity());
            if (result.Status == RepositoryActionStatus.Created)
            {
                var newPostalCode = result.Entity.ToDTO();
                return Created(Request.RequestUri + "/" + newPostalCode.PostalCodeId, newPostalCode);
            }

            return BadRequest();
        }

        // usage 
        // [
        //    { "op": "replace", "path": "/county", "value": "Middlesex County" }
        // ]

        [ValidateFilter]
        [HttpPatch, Route("postalcodes/{id}", Name = "PartialUpdatePostalCodesById")]
        public IHttpActionResult Patch(int id, [FromBody] JsonPatchDocument<DTO.AreaPostalCode> postalCodePatchDoc)
        {
            var postalCodeEntity = this._repository.GetPostalCode(id);
            if (postalCodeEntity == null)
            {
                return NotFound();
            }

            var postalCode = postalCodeEntity.ToDTO();
            postalCodePatchDoc.ApplyTo(postalCode);

            var result = _repository.UpdatePostalCode(postalCode.ToEntity());

            if (result.Status == RepositoryActionStatus.Updated)
            {
                return Ok(result.Entity.ToDTO());
            }

            return BadRequest();
        }

        [HttpDelete, Route("postalcodes/{id}")]
        public IHttpActionResult Delete(int id)
        {
            var result = _repository.DeletePostalCode(id);

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
