// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace CLR.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helper functions to aid on Reflection
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the first occurence of the specified assembly attribute
        /// </summary>
        /// <typeparam name="T">The attribute type to look for</typeparam>
        /// <param name="assembly">The assembly to inspect for attributes</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Assembly assembly)
            where T : Attribute
        {
            return GetAttributes<T>(assembly).FirstOrDefault();
        }

        /// <summary>
        /// Gets all occurences of the specified assembly attribute
        /// </summary>
        /// <typeparam name="T">The attribute type to look for</typeparam>
        /// <param name="assembly">The assembly to inspect for attributes</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(this Assembly assembly)
            where T : Attribute
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
           
            var attributes = assembly.GetCustomAttributes<T>();

            return attributes;
        }

        /// <summary>
        /// Gets a specified meta-data value from an assembly
        /// </summary>
        /// <param name="assembly">The assembly to inspect for meta-data</param>
        /// <param name="key">Meta-data key</param>
        /// <returns>Meta-data value</returns>
        public static string GetMetadata(this Assembly assembly, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var attributes = GetAttributes<AssemblyMetadataAttribute>(assembly);

            var values = from item in attributes
                         where item.Key == key
                         select item.Value;

            return values.FirstOrDefault();
        }
    }
}