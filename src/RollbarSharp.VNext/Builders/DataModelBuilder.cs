namespace RollbarSharp.VNext.Builders
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using RollbarSharp.Serialization;
    using RollbarSharp.Builders;
    using Microsoft.AspNet.Http;
    using System.Threading.Tasks;
    using Microsoft.Framework.ConfigurationModel;

    public class DataModelBuilder : RollbarSharp.Builders.DataModelBuilder
    {
        public DataModelBuilder(IConfiguration config)
            :this(JsonConfiguration.CreateFromConfig(config))
        {
        }

        public DataModelBuilder(Configuration configuration)
        {
            Configuration = configuration;
        }

        public DataModel CreateExceptionNotice(HttpContext context, Exception ex, string message = null, string level = "error")
        {
            var body = BodyModelBuilder.CreateExceptionBody(ex);
            var model = Create(context, level, body);

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

        public DataModel CreateMessageNotice(HttpContext context, string message, string level = "info", IDictionary<string, object> customData = null)
        {
            return Create(context, level, BodyModelBuilder.CreateMessageBody(message, customData));
        }

        /// <summary>
        /// Create the best stub of a request that we can using the message level and body
        /// </summary>
        /// <param name="level"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        protected DataModel Create(HttpContext context, string level, BodyModel body)
        {
            var model = new DataModel(level, body);

            model.CodeVersion = Configuration.CodeVersion;
            model.Environment = Configuration.Environment;
            model.Platform = Configuration.Platform;
            model.Language = Configuration.Language;
            model.Framework = Configuration.Framework;

            model.Timestamp = (ulong)Now();

            model.Notifier = NotifierModelBuilder.CreateFromAssemblyInfo();

            model.Request = RequestModelBuilder.CreateFromHttpRequest(context.Request, Configuration.ScrubParams).Result;
            model.Server = ServerModelBuilder.CreateFromHttpRequest(context.Request);
            model.Person = PersonModelBuilder.CreateFromHttpRequest(context.Request);

            model.Server.GitSha = Configuration.GitSha;
            
            return model;
        }
    }
}
