namespace CountryInfo.DTO
{
    public class AreaPostalCode
    {
        public int PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PreferredCity { get; set; }
        public string StateAbbrev { get; set; }
        public string County { get; set; }
        public int SourceType { get; set; }
        public string StateCode { get; set; }
        public int CountryId { get; set; }
    }
}
