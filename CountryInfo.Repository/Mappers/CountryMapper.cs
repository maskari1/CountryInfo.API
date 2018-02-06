
namespace CountryInfo.Repository.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using CountryInfo.DTO;
    using CountryInfo.Repository.Helpers;

    public static class CountryMapper
    {
        public static CountryWithPostalCodes ToDtoWithPostalCodes(this Entities.Country c)
        {
            var country = new CountryWithPostalCodes
                       {
                           CountryId = c.CountryID,
                           Name = c.Name,
                           Abbreviation = c.Abbreviation,
                           PostalCodeFormat = c.PostalCodeFormat,
                       };

            foreach (var p in c.PostalCodes)
            {
                country.PostalCodes.Add(p.ToDTO());
            }

            return country;
        }

        public static Country ToDTO(this Entities.Country c)
        {
            var country = new Country
            {
                CountryId = c.CountryID,
                Name = c.Name,
                Abbreviation = c.Abbreviation,
                PostalCodeFormat = c.PostalCodeFormat
            };

            return country;
        }

        public static object ToObject(this Entities.Country country, List<string> fields)
        {
            var lstOfFieldsToWorkWith = new List<string>(fields);

            if (!lstOfFieldsToWorkWith.Any())
            {
                return country.ToDTO(); 
            }

            // does it include any postalcodes-related field?
            var lstOfPostalCodeFields = lstOfFieldsToWorkWith.Where(f => f.Contains("postalcodes")).ToList();

            // if one of those fields is "postalcodes", we need to ensure the FULL postalcodes is returned.  
            // if it's only subfields, only those subfields have to be returned.
            var returnPartialPostalcodes = lstOfPostalCodeFields.Any() && !lstOfPostalCodeFields.Contains("postalcodes");

            // if we don't want to return the full postalcode, we need to know which fields
            if (returnPartialPostalcodes)
            {
                // remove all postalcode-related fields from the list of fields,
                // as we will use the ToObject function in AreaPostalCodeMapper for that.
                lstOfFieldsToWorkWith.RemoveRange(lstOfPostalCodeFields);
                lstOfPostalCodeFields = lstOfPostalCodeFields.Select(f => f.Substring(f.IndexOf(".") + 1)).ToList();
            }
            else
            {
                // we shouldn't return a partial postalcodes, but the consumer might still have
                // asked for a subfield together with the main field, ie: postalcodes,postalcodes.name.  
                // we need to remove those subfields in that case.
                lstOfPostalCodeFields.Remove("postalcodes");
                lstOfFieldsToWorkWith.RemoveRange(lstOfPostalCodeFields);
            }

            // if we have a postalcodes
            var countryWithPostalcodes = country.ToDtoWithPostalCodes();

            var objectToReturn = new ExpandoObject();
            foreach (var field in lstOfFieldsToWorkWith)
            {
                // need to include public and instance, b/c specifying a binding flag overwrites the
                // already-existing binding flags.
                var fieldValue = countryWithPostalcodes.GetType()
                    .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    .GetValue(countryWithPostalcodes, null);

                // add the field to the ExpandoObject
                ((IDictionary<String, Object>)objectToReturn).Add(field, fieldValue);
            }

            if (returnPartialPostalcodes)
            {
                // add a list of postalcodes, and in that, add all those postalcodes
               var postalCodes = country.PostalCodes.Select(postalCode => postalCode.ToObject(lstOfPostalCodeFields)).ToList();

                ((IDictionary<String, Object>)objectToReturn).Add("postalcodes", postalCodes);
            }

            return objectToReturn;
        }

        public static Entities.Country ToEntity(this DTO.Country c)
        {
            var country = new Entities.Country
            {
                CountryID = c.CountryId,
                Name = c.Name,
                Abbreviation = c.Abbreviation,
                PostalCodeFormat = c.PostalCodeFormat
            };

            return country;
        }
    }
}

