// Copyright (c) to owners found in https://github.com/arlm/Extensions/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Windows.Core.Extensions
{
    using System;
    using Microsoft.Win32;

    /// <summary>
    /// Windows registry helpers
    /// </summary>
    public static class WinReg
    {
        /// <summary>
        /// Returns a value from a registry key
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="registryKey">The registry root key</param>
        /// <param name="key">The key to return the value</param>
        /// <param name="valueName">The name of the value to be returned</param>
        /// <returns>The key value casted to the appropriate type</returns>
        public static T GetValue<T>(this RegistryKey registryKey, string key, string valueName)
        {
            if (registryKey == null)
            {
                throw new ArgumentNullException(nameof(registryKey));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (valueName == null)
            {
                throw new ArgumentNullException(nameof(valueName));
            }

            using (var subKey = registryKey.OpenSubKey(key))
            {
                if (subKey == null)
                {
                    return default(T);
                }

                return (T)registryKey.GetValue(valueName, default(T));
            }
        }

        /// <summary>
        /// Returns a value from a registry key
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="registryKey">The registry root key</param>
        /// <param name="key">The key with the value name to look for</param>
        /// <returns>The key value casted to the appropriate type</returns>
        public static T GetValue<T>(this RegistryKey registryKey, string key)
        {
            if (registryKey == null)
            {
                throw new ArgumentNullException(nameof(registryKey));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var keys = key.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return GetValue<T>(registryKey, subKeys, keys[keys.Length - 1]);
        }

        /// <summary>
        /// Sets a value in a registry key
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="registryKey">The registry root key</param>
        /// <param name="key">The key to set the value</param>
        /// <param name="valueName">The name of the value to be set</param>
        /// <param name="value">The value to be placed on the registry key</param>
        /// <returns>True if the value could be set, false otherwise</returns>
        public static bool SetValue<T>(this RegistryKey registryKey, string key, string valueName, T value)
        {
            if (registryKey == null)
            {
                throw new ArgumentNullException(nameof(registryKey));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (valueName == null)
            {
                throw new ArgumentNullException(nameof(valueName));
            }

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

        /// <summary>
        /// Sets a value in a registry key
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="registryKey">The registry root key</param>
        /// <param name="key">The key with the value name to set the value</param>
        /// <param name="value">The value to be placed on the registry key</param>
        /// <returns>True if the value could be set, false otherwise</returns>
        public static bool SetValue<T>(this RegistryKey registryKey, string key, T value)
        {
            if (registryKey == null)
            {
                throw new ArgumentNullException(nameof(registryKey));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var keys = key.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return SetValue(registryKey, subKeys, keys[keys.Length - 1], value);
        }

        /// <summary>
        /// Returns a string value from a registry key
        /// </summary>
        /// <param name="registryKey">The registry root key</param>
        /// <param name="key">The key with the value name to look for</param>
        /// <returns>The string key value or null if it not found</returns>
        public static string TryGetRegKeyValue(this RegistryKey registryKey, string key)
        {
            if (registryKey == null)
            {
                throw new ArgumentNullException(nameof(registryKey));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

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