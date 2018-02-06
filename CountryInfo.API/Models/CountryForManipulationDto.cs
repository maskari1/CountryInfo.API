namespace CountryInfo.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CountryForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(30, ErrorMessage = "The name shouldn't have more than 30 characters.")]
        public string Name { get; set; }

        [MaxLength(5, ErrorMessage = "The abbreviation shouldn't have more than 5 characters.")]
        public string Abbreviation { get; set; }

        [MaxLength(10, ErrorMessage = "The postalCodeFormat shouldn't have more than 10 characters.")]
        public string PostalCodeFormat { get; set; }

    }
}