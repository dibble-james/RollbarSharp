namespace RollbarSharp.VNext.Builders
{
    using Microsoft.AspNet.Http;
    using RollbarSharp.Serialization;

    public class PersonModelBuilder
    {
        /// <summary>
        /// Find just the username from server vars: AUTH_USER, LOGON_USER, REMOTE_USER
        /// Sets both the ID and Username to this username since ID is required.
        /// Email address won't be set.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static PersonModel CreateFromHttpRequest(HttpRequest request)
        {
            var username = request.HttpContext.User?.Identity.Name;

            return new PersonModel { Id = username, Username = username };
        }
    }
}