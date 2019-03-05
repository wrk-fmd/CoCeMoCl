using GeoClient.Models;
using System;

namespace GeoClient.Services.Registration
{
    public class RegistrationInfoParser
    {
        public static RegistrationInfo ParseRegistrationInfo(string urlString)
        {
            var uri = GetUri(urlString);

            RegistrationInfo info = null;
            if (uri != null)
            {
                var query = uri.Query;
                var id = GetValueOfQueryParameter("id", query);
                var token = GetValueOfQueryParameter("token", query);

                var baseUrl = GetBaseUrl(uri);

                if (id != null && token != null && baseUrl != null)
                    info = new RegistrationInfo(id, token, baseUrl);
            }
            else
            {
                Console.WriteLine("Cannot parse registration info from invalid URL. URL: " + urlString);
            }

            return info;
        }

        private static string GetBaseUrl(Uri uri)
        {
            string baseUrl = null;
            try
            {
                baseUrl = uri.Scheme + "://" + uri.Authority;
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Cannot get scheme or url of " + uri);
            }

            return baseUrl;
        }

        private static Uri GetUri(string uriString)
        {
            Uri uri = null;

            if (uriString == null)
                return null;

            try
            {
                uri = new Uri(uriString);
            }
            catch (UriFormatException)
            {
                Console.WriteLine("Cannot read URI from string: " + uriString);
            }

            return uri;
        }

        private static string GetValueOfQueryParameter(string queryParameter, string queryString)
        {
            var queryParameterString = queryParameter + "=";
            var startIndexOfParameter = queryString.IndexOf(queryParameterString, StringComparison.Ordinal);
            if (startIndexOfParameter == -1)
                return null;

            var substringStartingWithParameter =
                queryString.Substring(startIndexOfParameter + queryParameterString.Length);

            var indexOfNextParameter = substringStartingWithParameter.IndexOf("&", StringComparison.Ordinal);

            string queryValue;
            if (indexOfNextParameter >= 0)
            {
                queryValue = substringStartingWithParameter.Substring(0, indexOfNextParameter);
            }
            else
            {
                queryValue = substringStartingWithParameter;
            }

            return queryValue;
        }
    }
}