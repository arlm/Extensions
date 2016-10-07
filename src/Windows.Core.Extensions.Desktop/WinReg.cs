// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Windows.Core.Extensions
{
    using System;
    using Microsoft.Win32;

    public static class WinReg
    {
        public static T GetValue<T>(this RegistryKey registryKey, string key, string valueName)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (valueName == null)
                throw new ArgumentNullException(nameof(valueName));

            using (var subKey = registryKey.OpenSubKey(key))
            {
                if (subKey == null)
                {
                    return default(T);
                }

                return (T)registryKey.GetValue(valueName, default(T));
            }
        }

        public static T GetValue<T>(this RegistryKey registryKey, string key)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = key.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return GetValue<T>(registryKey, subKeys, keys[keys.Length - 1]);
        }

        public static bool SetValue<T>(this RegistryKey registryKey, string key, string valueName, T value)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (valueName == null)
                throw new ArgumentNullException(nameof(valueName));

            using (var subKey = registryKey.OpenSubKey(key))
            {
                if (subKey == null)
                {
                    return false;
                }

                registryKey.SetValue(valueName, value);
                registryKey.Flush();
                return true;
            }
        }

        public static bool SetValue<T>(this RegistryKey registryKey, string key, T value)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = key.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return SetValue(registryKey, subKeys, keys[keys.Length - 1], value);
        }

        public static string TryGetRegKeyValue(this RegistryKey registryKey, string key)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            using (var subKey = registryKey.OpenSubKey(key))
            {
                if (subKey != null)
                {
                    var value = subKey.GetValue(string.Empty);

                    if (value == null)
                    {
                        return null;
                    }

                    var stringValue = value as string;

                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        return stringValue.Substring(0, stringValue.Length - 4);
                    }

                    var dwordValue = value as int?;

                    if (dwordValue.HasValue)
                    {
                        return dwordValue.ToString();
                    }
                }

                return null;
            }
        }
    }
}