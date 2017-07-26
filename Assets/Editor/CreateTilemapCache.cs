using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreateTilemapCache {

    [MenuItem("Assets/Create/Tilemap Cache")]
    public static void CreateTilemapCacheAsset() {
        TilemapCache cache = new TilemapCache(0,0,0);
        string json = JsonUtility.ToJson(cache);
        File.WriteAllText(Application.dataPath + "/TilemapCache.json", json);
        AssetDatabase.Refresh();
    }
}
