

namespace CountryInfo.Repository.Entities
{
    using System;

    public static class Utils
    {
        private static string UserName { get; set; }

        static Utils()
        {
            var fullUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            var tokens = fullUserName.Split('\\');
            UserName = tokens.Length > 1 ? tokens[1] : tokens[0];
        }

        public static void FillCommonFields(EntityBase entity)
        {
            entity.Build = 1234;
            entity.SiteID = 1;
            entity.Active = true;
            entity.TouchedBy = UserName;
            entity.TouchedWhen = DateTime.Now;
            entity.CreatedBy = UserName;
            entity.CreatedWhen = DateTime.Now;
        }
    }
}
