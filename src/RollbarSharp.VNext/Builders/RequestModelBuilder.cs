namespace RollbarSharp.VNext.Builders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.AspNet.Http;
    using RollbarSharp.Serialization;

    public class RequestModelBuilder
    {
        public static RequestModel CreateFromHttpRequest(HttpRequest request, string[] scrubParams = null)
        {
            var m = new RequestModel
            {
                Url = string.Concat(request.Scheme, "://", request.Host, request.Path),
                Method = request.Method,
                Headers = request.Headers.ToDictionary(kvp => kvp.Key, kvp => string.Join(",", kvp.Value))
            };
            
            m.Session = request.HttpContext.Session.ToDictionary(kvp => kvp.Key, kvp => Encoding.UTF8.GetString(kvp.Value));

            if (request.QueryString.HasValue)
            {
                var rawQueryString = request.QueryString.Value.TrimStart('?');

                var queryStringParameters =
                    rawQueryString.Split('&').Select(qs => new KeyValuePair<string, string>(qs.Split('=').First(), qs.Split('=').Last()));

                m.QueryStringParameters = queryStringParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            try
            {
                m.PostParameters =
                    request.Form.ToDictionary(kvp => kvp.Key, kvp => string.Join(",", kvp.Value));
            }
            catch (InvalidOperationException)
            {
                // Swallow IOE.  ASP.Net throws this is no form exists.  Add the request body instead.
                m.PostParameters.Add("Body", new StreamReader(request.Body).ReadToEnd());
            }

            m.UserIp = request.HttpContext.GetFeature<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

            if (scrubParams != null)
            {
                m.Headers = Scrub(m.Headers, scrubParams);
                m.Session = Scrub(m.Session, scrubParams);
                m.QueryStringParameters = Scrub(m.QueryStringParameters, scrubParams);
                m.PostParameters = Scrub(m.PostParameters, scrubParams);
            }

            return m;
        }

        /// <summary>
        /// Finds dictionary keys in the <see cref="scrubParams"/> list and replaces their values
        /// with asterisks. Key comparison is case insensitive.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="scrubParams"></param>
        /// <returns></returns>
        protected static IDictionary<string, string> Scrub(IDictionary<string, string> dict, string[] scrubParams)
        {
            if (dict == null || !dict.Any())
                return dict;

            if (scrubParams == null || !scrubParams.Any())
                return dict;

            var itemsToUpdate = dict.Keys
                                    .Where(k => scrubParams.Contains(k, StringComparer.CurrentCultureIgnoreCase))
                                    .ToArray();

            if (itemsToUpdate.Any())
            {
                foreach (var key in itemsToUpdate)
                {
                    var len = dict[key] == null ? 8 : dict[key].Length;
                    dict[key] = new string('*', len);
                }
            }

            return dict;
        }
    }
}
