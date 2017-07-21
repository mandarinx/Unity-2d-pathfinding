using UnityEngine;

namespace PathFind {

    public class Grid {
        
        private Node[] nodes;
        private int[] nodelinks;
        public Node[] neighbours;
        
        public int width;
        public int height;
        private int[] neighbourDirs;
        
        private const int NUM_NEIGHBOURS = 4;

        public Grid(int width, int height, bool[] walkable_tiles) {
            Init(width, height);

            for (int i = 0; i < nodes.Length; ++i) {
                int x = -1;
                int y = -1;
                
                GetPoint(width, i, ref x, ref y);
                nodes[i] = new Node(walkable_tiles[i], x, y);
            }
        }

        public static void SetWalkable(Grid grid, int x, int y, bool walkable) {
            int n = GetIndex(grid.width, x, y);
            grid.nodes[n].walkable = walkable;
        }

        public static bool IsWalkable(Grid grid, int x, int y) {
            int n = GetIndex(grid.width, x, y);
            return grid.nodes[n].walkable;
        }

        public static Node GetNode(Grid grid, int x, int y) {
            return grid.nodes[GetIndex(grid.width, x, y)];
        }

        public static void GetPoint(int gridwidth, int index, ref int x, ref int y) {
            x = index % gridwidth; 
            y = Mathf.FloorToInt((float)index / gridwidth);
        }

        public static int GetIndex(int gridwidth, int x, int y) {
            return y * gridwidth + x;
        }

        private void Init(int gridWidth, int gridHeight) {
            width = gridWidth;
            height = gridHeight;
            nodes = new Node[width * height];
            nodelinks = new int[width * height];
            // Use Von Neumann neighbourhood.
            // Sequence: N > E > S > W
            neighbourDirs = new [] {width, 1, -width, -1};
            neighbours = new Node[NUM_NEIGHBOURS];
        }

        // It would be better if this function populated
        // the neighbour cache with the data needed by
        // FindPath.
        // - walkable
        // - coord or index
        // - f/g/hCost
        public int UpdateNeighboursCache(int nodeIndex) {
            int n = 0;
            for (int i = 0; i < NUM_NEIGHBOURS; ++i) {
                int ni = nodeIndex + neighbourDirs[i];

                if (ni < 0 || ni >= size) {
                    continue;
                }

                neighbours[n] = nodes[ni];
                ++n;
            }
            return n;
        }

        public int size {
            get { return width * height; }
        }

        public static void LinkNode(Grid grid, int child, int parent) {
            grid.nodelinks[child] = parent;
        }

        public static Node GetParentNode(Grid grid, Node child) {
            int ci = GetIndex(grid.width, child.x, child.y);
            int pi = grid.nodelinks[ci];
            return grid.nodes[pi];
        }
    }
}
