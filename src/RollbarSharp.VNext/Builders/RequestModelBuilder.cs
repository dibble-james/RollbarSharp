namespace RollbarSharp.VNext.Builders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Http;
    using RollbarSharp.Serialization;

    public class RequestModelBuilder : RollbarSharp.Builders.RequestModelBuilder
    {
        public static async Task<RequestModel> CreateFromHttpRequest(HttpRequest request, string[] scrubParams = null)
        {
            var m = new RequestModel
            {
                Url = string.Concat(request.Scheme, "://", request.Host, request.Path),
                Method = request.Method,
                Headers = request.Headers.Keys.Select(
                    k => new KeyValuePair<string, string>(k, request.Headers.Get(k))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };

            // TODO: Add session

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
                    request.Form.Keys.Select(
                        k => new KeyValuePair<string, string>(k, request.Form.Get(k))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (InvalidOperationException)
            {
                // Swallow IOE.  ASP.Net throws this is no form exists.
            }

            m.PostParameters.Add("Body", await new StreamReader(request.Body).ReadToEndAsync());

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
    }
}
