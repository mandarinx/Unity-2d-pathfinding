using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreateTilemapCache {

    [MenuItem("Assets/Create/Tilemap Cache")]
    public static void CreateTilemapCacheAsset() {
        TilemapCache cache = new TilemapCache();
        string json = JsonUtility.ToJson(cache);
        File.WriteAllText(Application.dataPath + "/TilemapCache.json", json);
        AssetDatabase.Refresh();
    }
}
