using Mandarin;

public interface IPathfinder {
    void Create(int width, int height);
    int FindPath(Point2 fromPoint, Point2 toPoint);
    void SetWalkable(int x, int y, bool walkable);
    bool IsWalkable(int x, int y);
    Point2 GetPathCoord(int n);
    int GetPathLength();
}
