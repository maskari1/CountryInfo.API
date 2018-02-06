using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CountryInfo.DTO
{
    public class CountryWithPostalCodes
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string PostalCodeFormat { get; set; }
        public ICollection<AreaPostalCode> PostalCodes { get; set; } = new List<AreaPostalCode>();
    }
}
