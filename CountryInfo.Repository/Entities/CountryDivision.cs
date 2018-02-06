using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryInfo.Repository.Entities
{
    public class CountryDivision : EntityBase
    {
        public decimal GUID { get; set; }
        public string Code { get; set; }
        public bool IsADTType { get; set; }
        public int? CountryID { get; set; }
        public string Abbreviation { get; set; }

        public Country Country { get; set; }
    }
}
