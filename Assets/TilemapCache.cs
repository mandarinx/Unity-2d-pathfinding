using System;
using Pathfinding;
using UnityEngine;

[Serializable]
public class TilemapCache {
    
    [SerializeField]
    public int[] tilemap;

    [SerializeField]
    private int w;
    public int width { get { return w; } private set { w = value; } }

    [SerializeField]
    private int h;
    public int height { get { return h; } private set { h = value; } }

    public TilemapCache(int width, int height, int type) {
        this.width = width;
        this.height = height;
        tilemap = new int[width * height];
        for (int i = 0; i < tilemap.Length; ++i) {
            tilemap[i] = type;
        }
    }

    public void SetType(int x, int y, int type) {
        int n = Grid.GetIndex(width, x, y);
        tilemap[n] = type;
    }

    public int GetType(int x, int y) {
        int n = Grid.GetIndex(width, x, y);
        return tilemap[n];
    }

    public bool IsType(int x, int y, int type) {
        int n = Grid.GetIndex(width, x, y);
        return (type & tilemap[n]) > 0;
    }
}
