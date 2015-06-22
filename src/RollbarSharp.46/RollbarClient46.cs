namespace RollbarSharp
{
    using Microsoft.Framework.ConfigurationModel;
    using RollbarSharp.Builders;

    public class RollbarClient46 : RollbarClient
    {
        public RollbarClient46(Configuration configuration)
        {
            Configuration = configuration;
            NoticeBuilder = new DataModelBuilder(Configuration);
        }

        /// <summary>
        /// Creates a new RollbarClient using the given access token
        /// and all default <see cref="Configuration"/> values
        /// </summary>
        /// <param name="accessToken"></param>
        public RollbarClient46(string accessToken)
            : this(new Configuration(accessToken))
        {
        }

        public RollbarClient46(IConfiguration config)
            : this(JsonConfiguration.CreateFromConfig(config))
        {
        }
    }
}