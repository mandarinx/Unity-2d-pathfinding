using Mandarin;
using Pathfinding;

public interface IPathfinder {
    void Create(int width, int height, int type);
    int FindPath(Point2 fromPoint, Point2 toPoint, int mask);
    void SetType(int x, int y, int type);
    int GetType(int x, int y);
    PathData GetPathData(int n);
    int GetPathLength();
}
