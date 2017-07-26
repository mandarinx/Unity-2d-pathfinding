#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mandarin;
using Pathfinding;
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
    public int     paintMask;
    public int     walkMask;

    [Space(10f)]
    public TextAsset cacheAsset;
    
    [Space(10f)]
    public Toggle btnPath;
    public Toggle[] btnPaints;
    public Toggle[] btnMasks;

    [Space(10f)]
    public Sprite tileSprite;

    [Space(10f)]
    public int gridColumns;
    public int gridRows;

    [Space(10f)]
    public Font labelFont;

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

    private const int FLOOR = 1;
    
    private double ns;
    private long ms;
    private Stopwatch sw;

    void Awake() {
        Assert.IsNotNull(btnPath);
        Assert.IsNotNull(tileSprite);
        Assert.IsNotNull(cacheAsset);

        switch (state) {
            case PFState.PATH:
                btnPath.GetComponent<Toggle>().isOn = true;
                break;
            case PFState.PAINT:
                btnPaints[(int)Mathf.Log(paintMask)].GetComponent<Toggle>().isOn = true;
                break;
        }
        
        btnPath.onValueChanged.AddListener(OnClickPath);
        btnPaints[0].onValueChanged.AddListener(OnClickPaint0);
        btnPaints[1].onValueChanged.AddListener(OnClickPaint1);
        btnPaints[2].onValueChanged.AddListener(OnClickPaint2);
        btnPaints[3].onValueChanged.AddListener(OnClickPaint4);
        btnMasks[0].onValueChanged.AddListener(OnClickMask0);
        btnMasks[1].onValueChanged.AddListener(OnClickMask1);
        btnMasks[2].onValueChanged.AddListener(OnClickMask2);

        SetMask();

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
        
        sw = new Stopwatch();
    }

    private void OnApplicationQuit() {
        string json = JsonUtility.ToJson(cache);
        File.WriteAllText(Application.dataPath + "/TilemapCache.json", json);
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }

    private void OnEnable() {
        cache = JsonUtility.FromJson<TilemapCache>(cacheAsset.text);
        if (cache == null) {
            cache = new TilemapCache(gridColumns, gridRows, FLOOR);
        }
        
        if (cache.width != gridColumns || 
            cache.height != gridRows ||
            cache.tilemap.Length != gridColumns * gridRows) {
            cache = new TilemapCache(gridColumns, gridRows, FLOOR);
        }

        pathfinder = new TwoDeePathfinder();
        pathfinder.Create(gridColumns, gridRows, FLOOR);

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
            
            GameObject label = new GameObject("Label");
            
            label.AddComponent<Canvas>();
            label.AddComponent<CanvasRenderer>();
            
            CanvasScaler cs = label.AddComponent<CanvasScaler>();
            cs.dynamicPixelsPerUnit = 100f;
            
            Text lText = label.AddComponent<Text>();
            lText.fontSize = 0;
            lText.supportRichText = false;
            lText.alignment = TextAnchor.MiddleCenter;
            lText.color = new Color(105f/255f, 105f/255f, 105f/255f);
            lText.raycastTarget = false;
            lText.resizeTextForBestFit = true;
            lText.resizeTextMinSize = 0;
            lText.font = labelFont;
            
            RectTransform rt = label.GetComponent<RectTransform>();
            rt.sizeDelta = Vector2.one * 0.5f;
            rt.position = new Vector3(0.5f, 0.5f, 0f);
            
            label.transform.SetParent(tile.transform);
            
            tile.transform.SetParent(transform);
            tile.transform.localScale = new Vector3(tileWidth, tileHeight, 1f);

            int x = -1;
            int y = -1;
            Grid.GetPoint(gridColumns, i, out x, out y);
            tile.transform.localPosition = new Vector3(
                x * tileWidth - hWorldUnits * 0.5f, 
                y * tileHeight - vWorldUnits * 0.5f, 
                0);

            int type = cache.tilemap[i];
            pathfinder.SetType(x, y, type);
            if (type > 0) {
                lText.text = type.ToString();
            }
            SetTileColor(sr, type == 0 ? Color.black : Color.white);
        }
    }

    private void OnDisable() {
        for (int c = transform.childCount - 1; c >= 0; --c) {
            DestroyImmediate(transform.GetChild(c).gameObject);
        }
    }

    private void ActionPaint(int x, int y, int n) {
        Text label = transform.GetChild(n).GetChild(0).GetComponent<Text>();
        label.text = label.text != "0" && paintMask == 0
            ? ""
            : paintMask.ToString();

        SetTileColor(n, paintMask == 0 ? Color.black : Color.white);
        pathfinder.SetType(x, y, paintMask);
        cache.SetType(x, y, paintMask);
    }

    private void ActionPath(int x, int y, int n) {
        Transform tile = transform.GetChild(n);
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();

        int len = pathfinder.GetPathLength();
        
        if (settingFrom) {
            // Reset
            for (int i=0; i<len; ++i) {
                SetTileColor(pathfinder.GetPathData(i), Color.white);
            }
            
            if (posFrom.x >= 0 && posFrom.y >= 0) {
                int fi = Grid.GetIndex(gridColumns, posFrom.x, posFrom.y);
                SetTileColor(fi, pathfinder.GetType(posFrom.x, posFrom.y) > 0 ? Color.white : Color.black);
            }
                
            if (posTo.x >= 0 && posTo.y >= 0) {
                int ti = Grid.GetIndex(gridColumns, posTo.x, posTo.y);
                SetTileColor(ti, Color.white);
            }
                
            posTo = new Point2(-1, -1);
            posFrom = new Point2(x, y);
            settingFrom = false;
            sr.color = Color.red;
            return;
        }

        if (!settingFrom) {
            posTo = new Point2(x, y);
            settingFrom = true;
            
            #if UNITY_EDITOR
            Profiler.BeginSample("Pathfind");
            #endif

            sw.Start();
            len = pathfinder.FindPath(posFrom, posTo, walkMask);
            sw.Stop();
            ns = sw.Elapsed.TotalMilliseconds * 1000000;
            ms = sw.ElapsedMilliseconds;
            sw.Reset();
            
            #if UNITY_EDITOR
            Profiler.EndSample();
            #endif

            for (int i=0; i<len; ++i) {
                SetTileColor(pathfinder.GetPathData(i), Color.green);
            }
            
            sr.color = Color.blue;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        Vector3 pos = eventData.pointerCurrentRaycast.worldPosition 
                      + new Vector3(hWorldUnits * 0.5f, vWorldUnits * 0.5f, 0f);
        int x = Mathf.FloorToInt(pos.x / tileWidth);
        int y = Mathf.FloorToInt(pos.y / tileHeight);
        
        stateCallback[(int)state].Invoke(x, y, Grid.GetIndex(gridColumns, x, y));
    }

    private void SetTileColor(PathData data, Color col) {
        SetTileColor(Grid.GetIndex(gridColumns, data.x, data.y), col);
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
    
    private void SetTileColor(SpriteRenderer sr, Color col) {
        sr.color = col;
    }

    private void SetMask() {
        walkMask = 0;
        for (int i = 0; i < btnMasks.Length; ++i) {
            walkMask += btnMasks[i].isOn ? (int)Mathf.Pow(2, i) : 0;
        }
    }

    public void OnClickMask0(bool toggled) {
        SetMask();
    }

    public void OnClickMask1(bool toggled) {
        SetMask();
    }

    public void OnClickMask2(bool toggled) {
        SetMask();
    }

    public void OnClickPaint0(bool toggled) {
        state = PFState.PAINT;
        paintMask = 0;
    }

    public void OnClickPaint1(bool toggled) {
        state = PFState.PAINT;
        paintMask = 1;
    }

    public void OnClickPaint2(bool toggled) {
        state = PFState.PAINT;
        paintMask = 2;
    }

    public void OnClickPaint4(bool toggled) {
        state = PFState.PAINT;
        paintMask = 4;
    }

    public void OnClickPath(bool toggled) {
        state = PFState.PATH;
    }
    
    void OnGUI() {
        GUI.color = Color.black;
        GUI.Label(new Rect(10, 20, 700, 20), 
            "State: " + stateLabels[(int)state] 
            + " Pathfind: " + ns.ToString("000 000 000 000") + " ns"
            + " (" + ms + " ms)");
    }
}
