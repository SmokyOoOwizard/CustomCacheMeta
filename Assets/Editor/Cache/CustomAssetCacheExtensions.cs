using UnityEngine;

namespace CustomCacheMeta.Editor.Cache
{
    public static class CustomAssetCacheExtensions
    {
        public static void SetCache<T>(this Object assetObject, T data, string @namespace) where T : struct
        {
            CustomAssetCache.SetCache(assetObject, data, @namespace);
        }

        public static T? GetCache<T>(this Object assetObject, string @namespace) where T : struct
        {
            return CustomAssetCache.GetCache<T>(assetObject, @namespace);
        }
    }
}