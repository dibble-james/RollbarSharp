namespace RollbarSharp.Builders
{
    using System;
    using RollbarSharp.Serialization;

    public static class FrameModelBuilder
    {
        /// <summary>
        /// Try to include file names in the stack trace rather than just method names with internal offsets
        /// </summary>
        public static bool UseFileNames = true;

        /// <summary>
        /// Converts the Exception's stack trace to simpler models.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        /// <remarks>If we want to get fancier, we can rip off ideas from <see cref="StackTrace.ToString()"/></remarks>
        public static FrameModel[] CreateFramesFromException(Exception exception)
        {
            // .Net Core currently has no Stack Trace objects.  Exceptions only contain a stack trace string.
            return new FrameModel[] { new FrameModel(exception.Source) { Code = exception.StackTrace } };
        }
    }
}