namespace RollbarSharp.VNext
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Http;
    using Microsoft.Framework.ConfigurationModel;
    using RollbarSharp;
    using RollbarSharp.Serialization;
    using RollbarSharp.VNext.Builders;

    public class RollbarClient : RollbarSharp.RollbarClient
    {
        /// <summary>
        /// Builds Rollbar requests from <see cref="Exception"/>s or text messages
        /// </summary>
        /// <remarks>This only builds the body of the request, not the whole notice payload</remarks>
        public new DataModelBuilder NoticeBuilder { get; protected set; }

        public RollbarClient(Configuration configuration)
        {
            Configuration = configuration;
            NoticeBuilder = new DataModelBuilder(Configuration);
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

        /// <summary>
        /// Sends an exception using the "critical" level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendCriticalException(Exception ex, HttpContext context, string title = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendException(ex, context, title, "critical", modelAction);
        }

        /// <summary>
        /// Sends an exception using the "error" level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendErrorException(Exception ex, HttpContext context, string title = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendException(ex, context, title, "error", modelAction);
        }

        /// <summary>
        /// Sents an exception using the "warning" level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendWarningException(Exception ex, HttpContext context,  string title = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendException(ex, context, title, "warning", modelAction);
        }

        /// <summary>
        /// Sends the given <see cref="Exception"/> to Rollbar including
        /// the stack trace. 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        /// <param name="level">Default is "error". "critical" and "warning" may also make sense to use.</param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendException(Exception ex, HttpContext context, string title = null, string level = "error", Action<DataModel> modelAction = null, object userParam = null)
        {
            var notice = NoticeBuilder.CreateExceptionNotice(context, ex, title, level);
            if (modelAction != null)
            {
                modelAction(notice);
            }
            return Send(notice, userParam);
        }

        /// <summary>
        /// Sends a text notice using the "critical" level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="customData"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendCriticalMessage(string message, HttpContext context, IDictionary<string, object> customData = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendMessage(message, context, "critical", customData, modelAction, userParam);
        }

        /// <summary>
        /// Sents a text notice using the "error" level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="customData"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendErrorMessage(string message, HttpContext context, IDictionary<string, object> customData = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendMessage(message, context, "error", customData, modelAction, userParam);
        }

        /// <summary>
        /// Sends a text notice using the "warning" level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="customData"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendWarningMessage(string message, HttpContext context, IDictionary<string, object> customData = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendMessage(message, context, "warning", customData, modelAction, userParam);
        }

        /// <summary>
        /// Sends a text notice using the "info" level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="customData"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendInfoMessage(string message, HttpContext context, IDictionary<string, object> customData = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendMessage(message, context, "info", customData, modelAction, userParam);
        }

        /// <summary>
        /// Sends a text notice using the "debug" level
        /// </summary>
        /// <param name="message"></param>
        /// <param name="customData"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendDebugMessage(string message, HttpContext context, IDictionary<string, object> customData = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            return SendMessage(message, context, "debug", customData, modelAction);
        }

        /// <summary>
        /// Sents a text notice using the given level of severity
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="customData"></param>
        /// <param name="modelAction"></param>
        /// <param name="userParam"></param>
        public Task SendMessage(string message, HttpContext context, string level, IDictionary<string, object> customData = null, Action<DataModel> modelAction = null, object userParam = null)
        {
            var notice = NoticeBuilder.CreateMessageNotice(context, message, level, customData);
            if (modelAction != null)
            {
                modelAction(notice);
            }
            return Send(notice, userParam);
        }
    }
}