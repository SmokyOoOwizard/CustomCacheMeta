using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace CustomCacheMeta.Editor.Meta
{
    public static class CustomMeta
    {
        public static string GetMetaFilePath(string assetPath)
        {
            var fileName = Path.GetFileName(assetPath);

            var metaFileName = $".{fileName}.customMeta";

            var dirName = Path.GetDirectoryName(assetPath);

            var metaFilePath = Path.Combine(dirName!, metaFileName);

            return metaFilePath;
        }

        private static FileStream GetOrCreateMetaFile(Object assetObject)
        {
            var assetPath = AssetDatabase.GetAssetPath(assetObject);

            var metaFilePath = GetMetaFilePath(assetPath);

            var fullPath = Path.Combine(Application.dataPath, "../", metaFilePath);

            return new FileStream(fullPath, FileMode.OpenOrCreate);
        }

        private static FileStream GetMetaFile(Object assetObject)
        {
            var assetPath = AssetDatabase.GetAssetPath(assetObject);

            var metaFilePath = GetMetaFilePath(assetPath);

            var fullPath = Path.Combine(Application.dataPath, "../", metaFilePath);

            return new FileStream(fullPath, FileMode.Open);
        }


        public static void SetMetaProperty<T>(Object assetObject, T data, string name) where T : struct
        {
            using var metaFile = GetOrCreateMetaFile(assetObject);
            using var reader = new StreamReader(metaFile);

            var metaJson = reader.ReadToEnd();

            AssetCustomMeta meta;
            if (!string.IsNullOrWhiteSpace(metaJson))
                meta = JsonUtility.FromJson<AssetCustomMeta>(metaJson);
            else
                meta = new AssetCustomMeta
                {
                    Properties = new()
                };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            if (meta.Properties == null)
                meta.Properties = new();

            meta.Properties[name] = json;

            metaFile.SetLength(0);
            metaFile.Flush();

            using var writer = new StreamWriter(metaFile);

            writer.Write(JsonConvert.SerializeObject(meta, Formatting.Indented));
        }

        public static T? GetMetaProperty<T>(Object assetObject, string name) where T : struct
        {
            try
            {
                using var metaFile = GetMetaFile(assetObject);

                using var reader = new StreamReader(metaFile);

                var metaJson = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(metaJson))
                    return default;

                var meta = JsonConvert.DeserializeObject<AssetCustomMeta>(metaJson);

                if (!meta.Properties.TryGetValue(name, out var json))
                    return default;

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static void RemoveMetaProperty(Object assetObject, string name)
        {
            using var metaFile = GetOrCreateMetaFile(assetObject);
            using var reader = new StreamReader(metaFile);

            var metaJson = reader.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(metaJson))
                return;

            var meta = JsonUtility.FromJson<AssetCustomMeta>(metaJson);
            
            if (meta.Properties == null)
                return;
            
            meta.Properties.Remove(name);

            metaFile.SetLength(0);
            metaFile.Flush();

            using var writer = new StreamWriter(metaFile);

            writer.Write(JsonConvert.SerializeObject(meta, Formatting.Indented));
        }
    }
}