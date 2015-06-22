namespace RollbarSharp.VNext
{
    using Microsoft.Framework.ConfigurationModel;
    using RollbarSharp;
    using RollbarSharp.Builders;

    public class RollbarClient : RollbarSharp.RollbarClient
    {
        public RollbarClient(Configuration configuration)
        {
            Configuration = configuration;
            NoticeBuilder = new RollbarSharp.DataModelBuilder(Configuration);
        }

        /// <summary>
        /// Creates a new RollbarClient using the given access token
        /// and all default <see cref="Configuration"/> values
        /// </summary>
        /// <param name="accessToken"></param>
        public RollbarClient(string accessToken)
            : this(new Configuration(accessToken))
        {
        }

        public RollbarClient(IConfiguration config)
            : this(JsonConfiguration.CreateFromConfig(config))
        {
        }
    }
}