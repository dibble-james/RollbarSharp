namespace RollbarSharp.VNext.Builders
{
    using System;
    using Microsoft.AspNet.Http;
    using Microsoft.Framework.Runtime;
    using RollbarSharp.Serialization;

    public static class ServerModelBuilder
    {
        /// <summary>
        /// Creates a <see cref="ServerModel"/> using data from the given
        /// <see cref="HttpRequest"/>. Finds: HTTP Host, Server Name, Application Physical Path
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ServerModel CreateFromHttpRequest(HttpRequest request)
        {
            var m = new ServerModel
            {
                Host = request.Host.Value,
                Machine = Environment.MachineName,
                Software = Environment.OSVersion.ToString(),
                Root = 
                (request.HttpContext.ApplicationServices.GetService(typeof(IApplicationEnvironment)) as IApplicationEnvironment)?.ApplicationBasePath
            };

            return m;
        }
    }
}