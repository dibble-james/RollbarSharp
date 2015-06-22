namespace RollbarSharp.VNext
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Http;
    using RollbarSharp;

    public class RollbarMiddleware
    {
        private readonly Lazy<RollbarClient> _rollbar;
        private readonly RequestDelegate _next;

        public RollbarMiddleware(RequestDelegate next, Configuration config)
        {
            this._rollbar = new Lazy<RollbarClient>(() => BuildClientWithConfig(config));

            this._next = next;
        }

        public RollbarMiddleware(RequestDelegate next, string accessToken)
        {
            this._rollbar = new Lazy<RollbarClient>(() => BuildClient(accessToken));

            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this._next(context);
            }
            catch (Exception ex)
            {
                Task.WaitAll(this._rollbar.Value.SendException(ex, context));

                throw;
            }
        }

        private static RollbarClient BuildClient(string accessToken)
        {
            return BuildClientWithConfig(new Configuration(accessToken));
        }

        private static RollbarClient BuildClientWithConfig(Configuration config)
        {
            var client = new RollbarClient(config);

            return client;
        }
    }
}