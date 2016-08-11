// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace CLR.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static T GetAttribute<T>(this Assembly assembly)
            where T : Attribute
        {
            return GetAttributes<T>(assembly).FirstOrDefault();
        }

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

        public static string GetMetaData(this Assembly assembly, string key)
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