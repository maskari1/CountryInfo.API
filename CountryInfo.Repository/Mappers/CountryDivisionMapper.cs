namespace CountryInfo.Repository.Mappers
{
    using CountryInfo.DTO;

    public static class CountryDivisionMapper
    {
        public static CountryDivision ToDTO(this Entities.CountryDivision cd)
        {
            return new CountryDivision { Abbreviation = cd.Abbreviation, Code = cd.Code, CountryID = cd.CountryID };
        }
    }
}
