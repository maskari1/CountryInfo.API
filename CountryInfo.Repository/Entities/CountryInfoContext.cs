namespace CountryInfo.Repository.Entities
{
    using System.Data.Entity;

    public partial class CountryInfoContext : DbContext
    {
        public CountryInfoContext()
            : base("name=CountryInfoContext")
        {
        }

        /// <summary>
        /// On model creating.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CountryDivision>().HasKey(c => c.GUID).ToTable("CV3CountryDivision");
            modelBuilder.Entity<Country>().HasKey(c => c.CountryID).ToTable("SXAAMCountry");
            modelBuilder.Entity<AreaPostalCode>().HasKey(c => c.PostalCodeID).ToTable("SXAAMPostalCode");

            // Foreign key relationship : Country and PostalCode
            modelBuilder.Entity<AreaPostalCode>()
                .HasRequired(p => p.Country)
                .WithMany(c => c.PostalCodes)
                .HasForeignKey(c => c.CountryID);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<CountryDivision> CountryDivisions { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<AreaPostalCode> AreaPostalCodes { get; set; }
    }
}
