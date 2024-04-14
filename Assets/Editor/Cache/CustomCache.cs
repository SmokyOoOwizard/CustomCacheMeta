using System.IO;
using UnityEngine;

namespace CustomCacheMeta.Editor.Cache
{
    public static class CustomCache
    {
        private const string CACHE_FOLDER_NAME = "CustomCache";

        private static string GetCachePath()
        {
            var basePath = Application.dataPath;

            var cachePath = basePath + $"../Library/{CACHE_FOLDER_NAME}/";

            if (!Directory.Exists(cachePath)) 
                Directory.CreateDirectory(cachePath);

            return cachePath;
        }

        public static FileStream GetOrCreateCacheFile(string @namespace,string cacheName)
        {
            var cachePath = GetCachePath();

            var path = Path.Combine(cachePath, @namespace, cacheName);
        
            return new FileStream(path, FileMode.OpenOrCreate);
        }
        
        public static FileStream GetCacheFile(string @namespace,string cacheName)
        {
            var cachePath = GetCachePath();

            var path = Path.Combine(cachePath, @namespace, cacheName);
        
            return new FileStream(path, FileMode.Open);
        }

        public static void ClearCache(string @namespace, string cacheName)
        {
            var cachePath = GetCachePath();
        
            var path = Path.Combine(cachePath, @namespace, cacheName);
        
            File.Delete(path);
        }

        public static void ClearCacheNamespace(string @namespace)
        {
            var cachePath = GetCachePath();

            var path = Path.Combine(cachePath, @namespace);
        
            Directory.Delete(path, true);
        }
    }
}

