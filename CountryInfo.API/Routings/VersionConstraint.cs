namespace CountryInfo.API.Routings
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Web.Http.Routing;

    public class VersionConstraint : IHttpRouteConstraint
    {
        public const string VersionHeaderName = "api-version";

        private const int DefaultVersion = 1;

        public VersionConstraint(int allowedVersion)
        {
            AllowedVersion = allowedVersion;
        }

        public int AllowedVersion { get; }
  

        public bool Match(HttpRequestMessage request, IHttpRoute route,
            string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                // try custom request header "api-version"
                var version = GetVersionFromCustomRequestHeader(request);

                // not found?  Try custom content type in accept header
                if (version == null)
                {
                    version = GetVersionFromCustomContentType(request);
                }


                return (version ?? DefaultVersion) == AllowedVersion;
            }

            return true;
        }

        // Accept:application/vnd.countryinfoapi.v2+json

        private int? GetVersionFromCustomContentType(HttpRequestMessage request)
        {
            // get the accept header.
            var mediaTypes = request.Headers.Accept.Select(h => h.MediaType);

            // find the one with the version number - match through regex
            var regEx = new Regex(@"application\/vnd\.countryinfoapi\.v([\d]+)\+json");

            var matchingMediaType = mediaTypes.FirstOrDefault(mediaType => regEx.IsMatch(mediaType));

            if (matchingMediaType == null)
            {
                return null;
            }

            // extract the version number
            var m = regEx.Match(matchingMediaType);
            var versionAsString = m.Groups[1].Value;

            // ... and return
            int version;
            if (int.TryParse(versionAsString, out version))
            {
                return version;
            }

            return null;
        }

        // api-version:2
        private int? GetVersionFromCustomRequestHeader(HttpRequestMessage request)
        {
            string versionAsString;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(VersionHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }

            int version;
            if (versionAsString != null && int.TryParse(versionAsString, out version))
            {
                return version;
            }

            return null;
        }
    }
}