namespace RollbarSharp.VNext.Builders
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using RollbarSharp.Serialization;
    using RollbarSharp.Builders;
    using Microsoft.AspNet.Http;
    using System.Threading.Tasks;
    using Microsoft.Framework.ConfigurationModel;

    public class DataModelBuilder
    {
        public Configuration Configuration { get; protected set; }

        public DataModelBuilder(IConfiguration config)
            :this(JsonConfiguration.CreateFromConfig(config))
        {
        }

        public DataModelBuilder(Configuration configuration)
        {
            Configuration = configuration;
        }

        public async Task<DataModel> CreateExceptionNotice(HttpContext context, Exception ex, string message = null, string level = "error")
        {
            var body = BodyModelBuilder.CreateExceptionBody(ex);
            var model = await Create(context, level, body);

            //merge exception data dictionaries to list of keyValues pairs
            var keyValuePairs = body.TraceChain.Where(tm => tm.Exception.Data != null).SelectMany(tm => tm.Exception.Data);
                        
            foreach (var keyValue in keyValuePairs)
            {
                //the keys in keyValuePairs aren't necessarily unique, so don't add but overwrite
                model.Custom[keyValue.Key.ToString()] = keyValue.Value;
            }

            model.Title = message;

            return model;
        }

        public async Task<DataModel> CreateMessageNotice(HttpContext context, string message, string level = "info", IDictionary<string, object> customData = null)
        {
            return await Create(context, level, BodyModelBuilder.CreateMessageBody(message, customData));
        }

        /// <summary>
        /// Create the best stub of a request that we can using the message level and body
        /// </summary>
        /// <param name="level"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected async Task<DataModel> Create(HttpContext context, string level, BodyModel body)
        {
            var model = new DataModel(level, body);

            model.CodeVersion = Configuration.CodeVersion;
            model.Environment = Configuration.Environment;
            model.Platform = Configuration.Platform;
            model.Language = Configuration.Language;
            model.Framework = Configuration.Framework;

            model.Timestamp = (ulong)Now();

            model.Notifier = NotifierModelBuilder.CreateFromAssemblyInfo();

            model.Request = await RequestModelBuilder.CreateFromHttpRequest(context.Request, Configuration.ScrubParams);
            model.Server = ServerModelBuilder.CreateFromHttpRequest(context.Request);
            model.Person = PersonModelBuilder.CreateFromHttpRequest(context.Request);

            model.Server.GitSha = Configuration.GitSha;
            
            return model;
        }

        /// <summary>
        /// Current UTC date time as a UNIX timestamp
        /// </summary>
        /// <returns></returns>
        private static double Now()
        {
            var epoch = new DateTime(1970, 1, 1);
            return (DateTime.UtcNow - epoch).TotalSeconds;
        }

        public static string FingerprintHash(params object[] fields)
        {
            return FingerprintHash(string.Join(",", fields));
        }

        /// <summary>
        /// To make sure fingerprints are the correct length and don't
        /// contain any problematic characters, SHA1 the fingerprint.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string FingerprintHash(string data)
        {
            using (var sha = new SHA1Managed())
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
