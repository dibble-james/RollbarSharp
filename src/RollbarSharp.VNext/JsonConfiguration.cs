namespace RollbarSharp.VNext
{
    using System;
    using Microsoft.Framework.ConfigurationModel;

    public class JsonConfiguration : Configuration
    {
        public JsonConfiguration(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Creates a <see cref="Configuration"/>, reading values from <see cref="IConfiguration"/>.
        /// Rollbar.AccessToken
        /// Rollbar.CodeVersion
        /// Rollbar.Endpoint
        /// Rollbar.Environment
        /// Rollbar.Platform
        /// Rollbar.CodeLanguage
        /// Rollbar.Framework
        /// Rollbar.GitSha
        /// </summary>
        /// <returns></returns>
        public static Configuration CreateFromConfig(IConfiguration config)
        {
            var token = GetSettingFromJson(config, "Rollbar:AccessToken");

            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Missing access token at Rollbar:AccessToken");

            var conf = new Configuration(token);

            conf.CodeVersion = GetSettingFromJson(config, "Rollbar:CodeVersion") ?? conf.CodeVersion;
            conf.Endpoint = GetSettingFromJson(config, "Rollbar:Endpoint") ?? conf.Endpoint;
            conf.Environment = GetSettingFromJson(config, "Rollbar:Environment") ?? conf.Environment;
            conf.Platform = GetSettingFromJson(config, "Rollbar:Platform") ?? conf.Platform;
            conf.Language = GetSettingFromJson(config, "Rollbar:CodeLanguage") ?? conf.Language;
            conf.Framework = GetSettingFromJson(config, "Rolllbar:Framework") ?? conf.Framework;
            conf.GitSha = GetSettingFromJson(config, "Rollbar:GitSha");

            var scrubParams = GetSettingFromJson(config, "Rollbar:ScrubParams");
            conf.ScrubParams = scrubParams == null ? DefaultScrubParams : scrubParams.Split(',');

            return conf;
        }

        protected static string GetSettingFromJson(IConfiguration config, string name, string fallback = null)
        {
            string setting;

            config.TryGet(name, out setting);

            return string.IsNullOrEmpty(setting) ? fallback : setting;
        }
    }
}
