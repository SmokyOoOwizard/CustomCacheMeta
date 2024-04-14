using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CustomCacheMeta.Editor.Cache
{
    public static class CustomAssetCache
    {
        private static void GetCachePath(GUID uid, string @namespace, out string finalNamespace, out string cacheName)
        {
            var strUid = uid.ToString();

            var prefix = strUid[..2];

            finalNamespace = Path.Combine(@namespace, prefix);

            cacheName = strUid;
        }

        public static void SetCache<T>(string name, T data, string @namespace) where T : struct
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "name cannot be null or empty");
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException(nameof(@namespace), "namespace cannot be null or empty");

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using var file = CustomCache.GetOrCreateCacheFile(@namespace, name);
            file.SetLength(0);
            file.Flush();
            
            using var writer = new StreamWriter(file);

            writer.Write(json);
        }

        public static void SetCache<T>(GUID uid, T data, string @namespace) where T : struct
        {
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException(nameof(@namespace), "namespace cannot be null or empty");
            
            GetCachePath(uid, @namespace, out var finalNamespace, out var strUid);

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using var file = CustomCache.GetOrCreateCacheFile(finalNamespace, strUid);
            file.SetLength(0);
            file.Flush();
            
            using var writer = new StreamWriter(file);

            writer.Write(json);
        }

        public static void SetCache<T>(Object assetObject, T data, string @namespace) where T : struct
        {
            if (string.IsNullOrEmpty(@namespace))
                throw new ArgumentNullException(nameof(@namespace), "namespace cannot be null or empty");
            
            var assetPath = AssetDatabase.GetAssetPath(assetObject);
            var uid = AssetDatabase.GUIDFromAssetPath(assetPath);

            SetCache(uid, data, @namespace);
        }

        public static T? GetCache<T>(string name, string @namespace) where T : struct
        {
            try
            {
                using var file = CustomCache.GetCacheFile(@namespace, name);
                using var reader = new StreamReader(file);

                var json = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T? GetCache<T>(GUID uid, string @namespace) where T : struct
        {
            try
            {
                GetCachePath(uid, @namespace, out var finalNamespace, out var strUid);

                using var file = CustomCache.GetCacheFile(finalNamespace, strUid);
                using var reader = new StreamReader(file);

                var json = reader.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T? GetCache<T>(Object assetObject, string @namespace) where T : struct
        {
            try
            {
                var assetPath = AssetDatabase.GetAssetPath(assetObject);
                var uid = AssetDatabase.GUIDFromAssetPath(assetPath);

                return GetCache<T>(uid, @namespace);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}