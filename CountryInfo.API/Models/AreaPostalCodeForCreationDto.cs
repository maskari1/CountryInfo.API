using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountryInfo.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AreaPostalCodeForCreationDto
    {
        [Required(ErrorMessage = "You should fill out a postalCode.")]
        [MaxLength(9, ErrorMessage = "The name shouldn't have more than 9 characters.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "You should fill out a city.")]
        [MaxLength(50, ErrorMessage = "The city shouldn't have more than 50 characters.")]
        public string City { get; set; }

        public string PreferredCity { get; set; }

        [MaxLength(5, ErrorMessage = "The stateAbbrev shouldn't have more than 5 characters.")]
        public string StateAbbrev { get; set; }

        [MaxLength(25, ErrorMessage = "The county shouldn't have more than 25 characters.")]
        public string County { get; set; }

        public int SourceType { get; set; }

        public string StateCode { get; set; }
        public int CountryId { get; set; }
    }
}