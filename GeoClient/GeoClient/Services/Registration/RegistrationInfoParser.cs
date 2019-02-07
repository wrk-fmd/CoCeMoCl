using GeoClient.Models;
using System;
using GeoClient.Services.Boundary;

namespace GeoClient.Services.Registration
{
    public class RegistrationInfoParser
    {
        public static RegistrationInfo ParseRegistrationInfo(string url)
        {
            RegistrationInfo info = null;
            if (url != null && url.StartsWith(RestService.ServerBaseUrl))
            {
                var startIndexOfId = url.IndexOf("id=", StringComparison.Ordinal) + 3;
                var indexOfToken = url.IndexOf("&token=", StringComparison.Ordinal);

                var id = url.Substring(startIndexOfId, indexOfToken - startIndexOfId);
                var token = url.Substring(indexOfToken + 7);

                info = new RegistrationInfo(id, token);
            }
            else
            {
                Console.WriteLine("Cannot parse registration info from invalid URL. URL: " + url);
            }

            return info;
        }
    }
}