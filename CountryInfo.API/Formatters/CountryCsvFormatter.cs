namespace CountryInfo.API.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    public class CountryCsvFormatter : BufferedMediaTypeFormatter
    {
        public CountryCsvFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv-country"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(DTO.Country) || typeof(IEnumerable<DTO.Country>).IsAssignableFrom(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                var countries = value as IEnumerable<DTO.Country>;
                if (countries != null)
                {
                    foreach (var country in countries)
                    {
                        WriteItem(country, writer);
                    }
                }
                else
                {
                    var country = value as DTO.Country;
                    if (country == null)
                    {
                        throw new InvalidOperationException("Type not supported.");
                    }

                    WriteItem(country, writer);
                }
            }
        }

        private static void WriteItem(DTO.Country country, TextWriter writer)
        {
            writer.WriteLine("{0}, {1}, {2}, {3}", 
                Escape(country.CountryId),
                Escape(country.Name),
                Escape(country.Abbreviation),
                Escape(country.PostalCodeFormat));          
        }

        static char[] _specialChars = { ',', '\n', '\r', '"' };

        private static string Escape(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            var field = obj.ToString();
            if (field.IndexOfAny(_specialChars) != -1)
            {
                // delimit the entire field with quotes and replace embedded quotes with "".
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
