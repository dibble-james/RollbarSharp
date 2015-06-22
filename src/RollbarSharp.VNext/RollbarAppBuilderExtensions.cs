namespace RollbarSharp.VNext
{
    using Microsoft.AspNet.Builder;

    public static class RollbarAppBuilderExtensions
    {
        public static IApplicationBuilder UseRollbar(this IApplicationBuilder app, Configuration config)
        {
            return app.UseMiddleware<RollbarMiddleware>(config);
        }

        public static IApplicationBuilder UseRollbar(this IApplicationBuilder app, string accessToken)
        {
            return app.UseMiddleware<RollbarMiddleware>(accessToken);
        }
    }
}