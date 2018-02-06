

namespace CountryInfo.Repository
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using CountryInfo.Repository.Entities;

    using Z.EntityFramework.Plus;

    public class CountryInfoEFRepository : ICountryInfoRepository
    {
        private CountryInfoContext _ctx;

        public CountryInfoEFRepository(CountryInfoContext ctx)
        {
            this._ctx = ctx;

            // Disable lazy loading - if not, related properties are auto-loaded when
            // they are accessed for the first time, which means they'll be included when
            // we serialize (b/c the serialization process accesses those properties).  
            // 
            // We don't want that, so we turn it off.  We want to eagerly load them (using Include)
            // manually.

            _ctx.Configuration.LazyLoadingEnabled = false;
        }

        public Country GetCountry(int id, bool includePostalCodes)
        {
            if (includePostalCodes)
            {
                return this._ctx.Countries.Include(c => c.PostalCodes).FirstOrDefault(c => c.CountryID == id);
            }

            return this._ctx.Countries.FirstOrDefault(c => c.CountryID == id);
        }

        public IQueryable<Country> GetCountries()
        {
            return this._ctx.Countries;
        }

        public IQueryable<Country> GetCountriesWithPostalCodes()
        {
            return this._ctx.Countries.Include("PostalCodes");
        }

        public RepositoryActionResult<Country> UpdateCountry(Country updateCountry)
        {
            try
            {
                // you can only update when an country already exists for this id
                var existingCountry = _ctx.Countries.FirstOrDefault(c => c.CountryID == updateCountry.CountryID);

                if (existingCountry == null)
                {
                    return new RepositoryActionResult<Country>(updateCountry, RepositoryActionStatus.NotFound);
                }

                Utils.FillCommonFields(updateCountry);

                // change the original entity status to detached; otherwise, we get an error on attach
                // as the entity is already in the dbSet

                // set original entity state to detached
                _ctx.Entry(existingCountry).State = EntityState.Detached;

                // attach & save
                _ctx.Countries.Attach(updateCountry);

                // set the updated entity state to modified, so it gets updated.
                _ctx.Entry(updateCountry).State = EntityState.Modified;


                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<Country>(updateCountry, RepositoryActionStatus.Updated);
                }

                return new RepositoryActionResult<Country>(updateCountry, RepositoryActionStatus.NothingModified, null);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Country>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<Country> InsertCountry(Country country)
        {
            try
            {
                Utils.FillCommonFields(country);
                _ctx.Countries.Add(country);
                var result = _ctx.SaveChanges();

                return result > 0
                           ? new RepositoryActionResult<Country>(country, RepositoryActionStatus.Created)
                           : new RepositoryActionResult<Country>(country, RepositoryActionStatus.NothingModified, null);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Country>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<Country> DeleteCountry(int id)
        {
            try
            {
                var country = _ctx.Countries.Where(c => c.CountryID == id).FirstOrDefault();

                if (country != null)
                {
                    _ctx.Countries.Remove(country);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<Country>(null, RepositoryActionStatus.Deleted);
                }

                return new RepositoryActionResult<Country>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<Country>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public AreaPostalCode GetPostalCode(int id)
        {
            return _ctx.AreaPostalCodes.FirstOrDefault(p => p.PostalCodeID == id);
        }

        public IQueryable<AreaPostalCode> GetPostalCodes()
        {
            return _ctx.AreaPostalCodes;
        }

        public IQueryable<AreaPostalCode> GetPostalCodes(int countryId)
        {
            return _ctx.AreaPostalCodes.Where(p => p.CountryID == countryId);
        }

        public RepositoryActionResult<AreaPostalCode> UpdatePostalCode(AreaPostalCode postalCode)
        {
            try
            {
                var existingPostalCode =
                    _ctx.AreaPostalCodes.FirstOrDefault(p => p.PostalCodeID == postalCode.PostalCodeID);
                if (existingPostalCode == null)
                {
                    return new RepositoryActionResult<AreaPostalCode>(postalCode, RepositoryActionStatus.NotFound);
                }

                Utils.FillCommonFields(postalCode);

                // set original entity state to detached
                _ctx.Entry(existingPostalCode).State = EntityState.Detached;

                // attach & save
                _ctx.AreaPostalCodes.Attach(postalCode);

                // set the updated entity state to modified, so it gets updated.
                _ctx.Entry(postalCode).State = EntityState.Modified;

                var result = _ctx.SaveChanges();
                if (result > 0)
                {
                    return new RepositoryActionResult<AreaPostalCode>(postalCode, RepositoryActionStatus.Updated);
                }

                return new RepositoryActionResult<AreaPostalCode>(
                           postalCode,
                           RepositoryActionStatus.NothingModified,
                           null);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<AreaPostalCode>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<AreaPostalCode> InsertPostalCode(AreaPostalCode postalCode)
        {
            try
            {
                Utils.FillCommonFields(postalCode);
                postalCode.SourceType = 1;
                _ctx.AreaPostalCodes.Add(postalCode);
                var result = this._ctx.SaveChanges();

                return result > 0
                           ? new RepositoryActionResult<AreaPostalCode>(postalCode, RepositoryActionStatus.Created)
                           : new RepositoryActionResult<AreaPostalCode>(postalCode, RepositoryActionStatus.NothingModified, null);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<AreaPostalCode>(null, RepositoryActionStatus.Error, ex);
            }
        }

        public RepositoryActionResult<AreaPostalCode> DeletePostalCode(int id)
        {
            try
            {
                var postalCode = _ctx.AreaPostalCodes.Where(c => c.PostalCodeID == id).FirstOrDefault();

                if (postalCode != null)
                {
                    _ctx.AreaPostalCodes.Remove(postalCode);
                    _ctx.SaveChanges();
                    return new RepositoryActionResult<AreaPostalCode>(null, RepositoryActionStatus.Deleted);
                }

                return new RepositoryActionResult<AreaPostalCode>(null, RepositoryActionStatus.NotFound);
            }
            catch (Exception ex)
            {
                return new RepositoryActionResult<AreaPostalCode>(null, RepositoryActionStatus.Error, ex);
            }
        }
    }
}
