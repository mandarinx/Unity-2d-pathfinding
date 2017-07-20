using Mandarin;
using PathFind;

public class TwoDeePathfinder : IPathfinder {

    private bool[] tilemap;
    private Grid grid;
    private Point2[] path;
    private int pathLength;
    private PathfindingBuffer buffer;
    
    public void Create(int width, int height) {
        int gridsize = width * height;
        tilemap = new bool[gridsize];

        for (int i = 0; i < tilemap.Length; ++i) {
            tilemap[i] = true;
        }

        grid = new Grid(width, height, tilemap);
        path = new Point2[width * height];
        buffer = new PathfindingBuffer(gridsize);
    }

    public int FindPath(Point2 fromPoint, Point2 toPoint) {
        buffer.Clear();
        pathLength = Pathfinding.FindPath(grid, fromPoint, toPoint, buffer, ref path);
        return pathLength;
    }

    public void SetWalkable(int x, int y, bool walkable) {
        Grid.SetWalkable(grid, x, y, walkable);
    }

    public bool IsWalkable(int x, int y) {
        return Grid.IsWalkable(grid, x, y);
    }

    public int GetPathLength() {
        return pathLength;
    }

    public Point2 GetPathCoord(int n) {
        // iterate in reverse order
        int len = pathLength - 1;
        return path[len - n];
    }

    public Point2[] GetPath() {
        return path;
    }
}
