namespace RollbarSharp.Builders
{
    using RollbarSharp.Serialization;

    public static class NotifierModelBuilder
    {
        /// <summary>
        /// Creates a model representing this notifier binding itself.
        /// </summary>
        /// <returns></returns>
        public static NotifierModel CreateFromAssemblyInfo()
        {
            return new NotifierModel("RollbarSharp.VNext", "0.0.0-alpha");
        }
    }
}