namespace CountryInfo.Repository.Entities
{
    using System;

    public class EntityBase
    {
        public bool Active { get; set; }
        public int Build { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string TouchedBy { get; set; }
        public DateTime TouchedWhen { get; set; }
        public short? SiteID { get; set; }
    }
}
