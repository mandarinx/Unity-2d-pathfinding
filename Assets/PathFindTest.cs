using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mandarin;
using PathFind;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;

public enum PFState {
    PAINT = 0,
    PATH = 1,
}

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class PathFindTest : MonoBehaviour, IPointerClickHandler {

    public PFState state;

    [Space(10f)]
    public TextAsset cacheAsset;
    
    [Space(10f)]
    public Button btnPaint;
    public Button btnPath;

    [Space(10f)]
    public Sprite tileSprite;

    [Space(10f)]
    public int gridColumns;
    public int gridRows;

    private float vWorldUnits;
    private float hWorldUnits;
    private float tileWidth;
    private float tileHeight;

    private Dictionary<int, Action<int, int, int>> stateCallback;
    private Dictionary<int, string>                stateLabels;

    private Point2         posFrom;
    private Point2         posTo;
    private bool           settingFrom;
    private IPathfinder    pathfinder;

    private TilemapCache   cache;

    void Awake() {
        Assert.IsNotNull(btnPaint);
        Assert.IsNotNull(btnPath);
        Assert.IsNotNull(tileSprite);
        Assert.IsNotNull(cacheAsset);
        
        SetState(state);
        
        btnPaint.onClick.AddListener(OnClickPaint);
        btnPath.onClick.AddListener(OnClickPath);

        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        bc.size = new Vector2(
            Camera.main.orthographicSize * 2 * Camera.main.aspect,
            Camera.main.orthographicSize * 2);

        settingFrom = true;
        posTo = new Point2(-1, -1);
        posFrom = new Point2(-1, -1);

        stateCallback = new Dictionary<int, Action<int, int, int>> {
            { 0, ActionPaint },
            { 1, ActionPath },
        };

        cache = JsonUtility.FromJson<TilemapCache>(cacheAsset.text);
        if (cache.width != gridColumns || 
            cache.height != gridRows ||
            cache.tilemap.Length == 0) {
            cache.Create(gridColumns, gridRows);
        }
    }

    private void OnApplicationQuit() {
        string json = JsonUtility.ToJson(cache);
        File.WriteAllText(Application.dataPath + "/TilemapCache.json", json);
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }

    private void OnEnable() {
        pathfinder = new TwoDeePathfinder();
        pathfinder.Create(gridColumns, gridRows);

        stateLabels = new Dictionary<int, string> {
            { 0, "Paint" },
            { 1, "Path" },
        };

        if (Application.isPlaying) {
            for (int c = transform.childCount - 1; c >= 0; --c) {
                Destroy(transform.GetChild(c).gameObject);
            }
        }

        vWorldUnits = Camera.main.orthographicSize * 2;
        hWorldUnits = vWorldUnits * Camera.main.aspect;
        tileWidth = hWorldUnits / gridColumns;
        tileHeight = vWorldUnits / gridRows;
        
        int tot = gridColumns * gridRows;
        for (int i = 0; i < tot; ++i) {
            GameObject tile = new GameObject("Tile_" + i.ToString("000"));
            SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
            sr.sprite = tileSprite;
            tile.transform.SetParent(transform);
            tile.transform.localScale = new Vector3(tileWidth, tileHeight, 1f);

            int x = -1;
            int y = -1;
            Grid.GetPoint(gridColumns, i, ref x, ref y);
            tile.transform.localPosition = new Vector3(
                x * tileWidth - hWorldUnits * 0.5f, 
                y * tileHeight - vWorldUnits * 0.5f, 
                0);

            bool isWalkable = cache.tilemap[i];
            sr.color = isWalkable ? Color.white: Color.gray;
            pathfinder.SetWalkable(x, y, isWalkable);
        }
    }

    private void OnDisable() {
        for (int c = transform.childCount - 1; c >= 0; --c) {
            DestroyImmediate(transform.GetChild(c).gameObject);
        }
    }

    private void ActionPaint(int x, int y, int n) {
        bool isWalkable = pathfinder.IsWalkable(x, y);
        Color col = isWalkable ? Color.gray : Color.white;
        Transform tile = transform.GetChild(n);
        tile.GetComponent<SpriteRenderer>().color = col;
        pathfinder.SetWalkable(x, y, !isWalkable);
        cache.SetWalkable(x, y, !isWalkable);
    }

    private void ActionPath(int x, int y, int n) {
        Transform tile = transform.GetChild(n);
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

        int len = pathfinder.GetPathLength();
        
        if (settingFrom) {
            // Reset
            for (int i=0; i<len; ++i) {
                SetTileColor(pathfinder.GetPathCoord(i), Color.white);
            }
            int fi = Grid.GetIndex(gridColumns, posFrom.x, posFrom.y);
            SetTileColor(fi, Color.white);
            int ti = Grid.GetIndex(gridColumns, posTo.x, posTo.y);
            SetTileColor(ti, Color.white);
            posTo = new Point2(-1, -1);
            posFrom = new Point2(x, y);
            settingFrom = false;
            sr.color = Color.red;
            return;
        }

        if (!settingFrom) {
            posTo = new Point2(x, y);
            settingFrom = true;
            sr.color = Color.blue;
            
            Profiler.BeginSample("Pathfind");
            len = pathfinder.FindPath(posFrom, posTo);
            Profiler.EndSample();

            for (int i=0; i<len; ++i) {
                SetTileColor(pathfinder.GetPathCoord(i), Color.green);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        Vector3 pos = eventData.pointerCurrentRaycast.worldPosition 
                      + new Vector3(hWorldUnits * 0.5f, vWorldUnits * 0.5f, 0f);
        int x = Mathf.FloorToInt(pos.x / tileWidth);
        int y = Mathf.FloorToInt(pos.y / tileHeight);
        
        stateCallback[(int)state].Invoke(x, y, Grid.GetIndex(gridColumns, x, y));
    }

    private void SetTileColor(Point2 pos, Color col) {
        SetTileColor(Grid.GetIndex(gridColumns, pos.x, pos.y), col);
    }
    
    private void SetTileColor(int n, Color col) {
        if (n >= transform.childCount || n < 0) {
            return;
        }
        transform
            .GetChild(n)
            .GetComponent<SpriteRenderer>()
            .color = col;
    }

    private void SetState(PFState pfstate) {
        state = pfstate;
        switch (pfstate) {
            case PFState.PAINT:
                btnPaint.GetComponent<Image>().color = Color.gray;
                btnPath.GetComponent<Image>().color = Color.white;
                return;
            case PFState.PATH:
                btnPaint.GetComponent<Image>().color = Color.white;
                btnPath.GetComponent<Image>().color = Color.gray;
                return;
        }
    }

    public void OnClickPaint() {
        SetState(PFState.PAINT);
    }

    public void OnClickPath() {
        SetState(PFState.PATH);
    }
    
    void OnGUI() {
        GUI.Label(new Rect(10, 20, 100, 20), "State: " + stateLabels[(int)state]);
    }
}
