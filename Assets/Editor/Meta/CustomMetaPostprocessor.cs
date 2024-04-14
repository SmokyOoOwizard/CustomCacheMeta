using System.IO;
using UnityEditor;

namespace CustomCacheMeta.Editor.Meta
{
    class CustomMetaPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            foreach (var str in deletedAssets)
            {
                var metaPath = CustomMeta.GetMetaFilePath(str);

                if (File.Exists(metaPath))
                    File.Delete(metaPath);
            }

            for (var i = 0; i < movedAssets.Length; i++)
            {
                var oldMeta = CustomMeta.GetMetaFilePath(movedFromAssetPaths[i]);

                if (!File.Exists(oldMeta))
                    continue;

                var newMeta = CustomMeta.GetMetaFilePath(movedAssets[i]);

                File.Move(oldMeta, newMeta);
            }
        }
    }
}