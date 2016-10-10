// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace ExceptionHandling.Extensions
{
    using System;
    using System.Text;

    /// <summary>
    /// Exception handling and logging helpers
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        /// A separator to be included on exception logs
        /// </summary>
        public const string EXCEPTION_SEPARATOR = "----------------------------------------";

        /// <summary>
        /// Flattens an exception, returning all inner exceptions and related stack-traces
        /// </summary>
        /// <param name="exception">Exception to be flattened</param>
        /// <returns>A dump of the exception and inner-exceptions, with stack-traces</returns>
        public static string Flatten(this Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);
                stringBuilder.AppendLine(EXCEPTION_SEPARATOR);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Flattens an aggregated exception, returning all inner exceptions and related stack-traces
        /// </summary>
        /// <param name="exception">Exception to be flattened</param>
        /// <returns>A dump of the aggregated exception and inner-exceptions, base exception and aggregated inner-exceptions with stack-traces</returns>
        public static string ToFlattenedString(this AggregateException exception)
        {
            var stringBuilder = new StringBuilder();
            var flattenedException = exception?.Flatten();

            stringBuilder.AppendLine(flattenedException.Message);
            stringBuilder.AppendLine(flattenedException.StackTrace);
            stringBuilder.AppendLine(EXCEPTION_SEPARATOR);

            stringBuilder.AppendLine("BASE EXCEPTION");
            stringBuilder.AppendLine(flattenedException.GetBaseException().Flatten());

            stringBuilder.AppendLine("INNER EXCEPTION");
            stringBuilder.AppendLine(flattenedException.InnerException.Flatten());

            stringBuilder.AppendLine("AGGREGATED INNER EXCEPTIONS");
            stringBuilder.AppendLine(EXCEPTION_SEPARATOR);

            foreach (var ex in flattenedException.InnerExceptions)
            {
                stringBuilder.AppendLine(ex.Flatten());
            }

            return stringBuilder.ToString();
        }
    }
}