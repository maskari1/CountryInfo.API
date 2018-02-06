using System.Collections.Generic;

namespace CountryInfo.Repository.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Country : EntityBase
    {
        public int HistoryID { get; set; }

        [Required]
        public int CountryID { get; set; }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string PostalCodeFormat { get; set; }

        public ICollection<AreaPostalCode> PostalCodes { get; set; } = new HashSet<AreaPostalCode>();
        public ICollection<CountryDivision> CountryDivisions { get; set; } = new HashSet<CountryDivision>();
    }
}
