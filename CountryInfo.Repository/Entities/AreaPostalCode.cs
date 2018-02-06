namespace CountryInfo.Repository.Entities
{
    public class AreaPostalCode : EntityBase
    {
        public int PostalCodeID { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PreferredCity { get; set; }
        public string StateAbbrev { get; set; }
        public string County { get; set; }
        public int CountryID { get; set; }
        public string StateCode { get; set; }
        public int SourceType { get; set; }

        public Country Country { get; set; }
    }
}
