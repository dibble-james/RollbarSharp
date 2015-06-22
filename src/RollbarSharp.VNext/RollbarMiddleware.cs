namespace RollbarSharp.VNext
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Http;

    public class RollbarMiddleware
    {
        private readonly Lazy<RollbarClient> _rollbar;
        private readonly RequestDelegate _next;

        public RollbarMiddleware(RequestDelegate next, Configuration config)
        {
            this._rollbar = new Lazy<RollbarClient>(() => BuildClientFromConfig(config));

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
            return BuildClientFromConfig(new Configuration(accessToken));
        }

        private static RollbarClient BuildClientFromConfig(Configuration config)
        {
            var client = new RollbarClient(config);

            return client;
        }
    }
}