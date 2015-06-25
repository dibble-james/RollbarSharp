﻿namespace RollbarSharp.VNext.Builders
{
    using System;
    using Microsoft.AspNet.Http;
    using Microsoft.Framework.Runtime;
    using RollbarSharp.Serialization;

    public class ServerModelBuilder
    {
        /// <summary>
        /// Creates a <see cref="ServerModel"/> using data from the given
        /// <see cref="HttpRequest"/>. Finds: HTTP Host, Server Name, Application Physical Path
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ServerModel CreateFromHttpRequest(HttpRequest request, IApplicationEnvironment environment)
        {
            var host = request.Host.ToUriComponent();

            var root = environment.ApplicationBasePath;
                        
            var machine = Environment.GetEnvironmentVariable("COMPUTERNAME");
            var software = environment.RuntimeFramework.FullName;

            return new ServerModel { Host = host, Root = root, Machine = machine, Software = software };
        }
    }
}