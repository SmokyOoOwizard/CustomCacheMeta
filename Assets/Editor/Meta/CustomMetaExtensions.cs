using UnityEngine;

namespace CustomCacheMeta.Editor.Meta
{
    public static class CustomMetaExtensions
    {
        public static void SetMetaProperty<T>(this Object assetObject, string name, T value) where T : struct
        {
            CustomMeta.SetMetaProperty(assetObject, value, name);
        }

        public static T? GetMetaProperty<T>(this Object assetObject, string name) where T : struct
        {
            return CustomMeta.GetMetaProperty<T>(assetObject, name);
        }
    }
}