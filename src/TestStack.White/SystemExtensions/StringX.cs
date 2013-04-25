using System;

namespace White.Core.SystemExtensions
{
    public static class StringX
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        /// <summary>
        /// Extension - indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or String.Empty, or if value consists exclusively of white-space characters. </returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            Func<char, bool> IsConsistOfWhiteSpaces = p => p.Equals(' ');
            //return value.All(IsConsistOfWhiteSpaces);
            return string.IsNullOrEmpty(value.Trim());
        }
    }
}
