using System;
using PathFind;
using UnityEngine;

[Serializable]
public class TilemapCache {
    
    [SerializeField]
    public bool[] tilemap;

    [SerializeField]
    private int w;
    public int width { get { return w; } private set { w = value; } }

    [SerializeField]
    private int h;
    public int height { get { return h; } private set { h = value; } }

    public void Create(int width, int height) {
        this.width = width;
        this.height = height;
        tilemap = new bool[width * height];
        for (int i = 0; i < tilemap.Length; ++i) {
            tilemap[i] = true;
        }
    }

    public void SetWalkable(int x, int y, bool walkable) {
        int n = Grid.GetIndex(width, x, y);
        tilemap[n] = walkable;
    }

    public bool IsWalkable(int x, int y) {
        int n = Grid.GetIndex(width, x, y);
        return tilemap[n];
    }
}
