using Mandarin;

namespace Pathfinding {
    public class TwoDeePathfinder : IPathfinder {
    
        private int[]               tilemap;
        private Grid                grid;
        private PathData[]          path;
        private int                 pathLength;
        
        public void Create(int width, int height, int type) {
            int gridsize = width * height;
            tilemap = new int[gridsize];
    
            for (int i = 0; i < tilemap.Length; ++i) {
                tilemap[i] = type;
            }
    
            grid = new Grid(width, height, tilemap);
            path = new PathData[width * height];
        }
    
        public int FindPath(Point2 fromPoint, Point2 toPoint, int mask) {
            pathLength = Pathfinder.Find(grid, fromPoint, toPoint, mask, ref path);
            return pathLength;
        }
    
        public void SetType(int x, int y, int type) {
            Node.SetType(Grid.GetNode(grid, x, y), type);
        }
    
        public int GetType(int x, int y) {
            return Node.GetType(Grid.GetNode(grid, x, y));
        }
    
        public int GetPathLength() {
            return pathLength;
        }
    
        public PathData GetPathData(int n) {
            // iterate in reverse order
            int len = pathLength - 1;
            return path[len - n];
        }
    }
}
