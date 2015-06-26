namespace RollbarSharp.Builders
{
    using Microsoft.AspNet.Http;
    using RollbarSharp.Serialization;

    public class PersonModelBuilder
    {
        /// <summary>
        /// Use the current identity from the <see cref="HttpContext"/>.
        /// Email address won't be set.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static PersonModel CreateFromHttpRequest(HttpContext request)
        {
            var username = request.User.Identity.IsAuthenticated ? request.User.Identity.Name : string.Empty;

            return new PersonModel { Id = username, Username = username };
        }
    }
}