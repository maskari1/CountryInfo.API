
namespace CountryInfo.Repository.Mappers
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using CountryInfo.DTO;

    public static class AreaPostalCodeMapper
    {
        public static AreaPostalCode ToDTO(this Entities.AreaPostalCode apc)
        {
            return new AreaPostalCode
                       {
                           City = apc.City,
                           PostalCode = apc.PostalCode,
                           PostalCodeId = apc.PostalCodeID,
                           County = apc.County,
                           PreferredCity = apc.PreferredCity,
                           StateAbbrev = apc.StateAbbrev,
                           StateCode = apc.StateCode,
                           CountryId = apc.CountryID
                       };
        }

        public static object ToObject(this Entities.AreaPostalCode p, List<string> fields)
        {
            var postalCode = p.ToDTO();
            var lstOfFieldsToWorkWith = new List<string>(fields);

            if (!lstOfFieldsToWorkWith.Any())
            {
                return postalCode;
            }

            var objectToReturn = new ExpandoObject();
            foreach (var field in lstOfFieldsToWorkWith)
            {
                var fieldValue = postalCode.GetType()
                    .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(postalCode, null);

                // add the field to the ExpandoObject
                ((IDictionary<string, object>)objectToReturn).Add(field, fieldValue);
            }

            return objectToReturn;
        }

        public static Entities.AreaPostalCode ToEntity(this DTO.AreaPostalCode apc)
        {
            return new Entities.AreaPostalCode
            {
                City = apc.City,
                PostalCode = apc.PostalCode,
                PostalCodeID = apc.PostalCodeId,
                County = apc.County,
                PreferredCity = apc.PreferredCity,
                StateAbbrev = apc.StateAbbrev,
                StateCode = apc.StateCode,
                CountryID = apc.CountryId
            };
        }
    }
}
