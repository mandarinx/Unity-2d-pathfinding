using Mandarin;
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
                Point2 p = GetPoint(this, i);
                nodes[i] = new Node(walkable_tiles[i], p.x, p.y);
            }
        }

        public static void SetWalkable(Grid grid, int x, int y, bool walkable) {
            int n = PFUtils.GetPosIndex(x, y, grid.width);
            grid.nodes[n].walkable = walkable;
        }

        public static bool IsWalkable(Grid grid, int x, int y) {
            int n = PFUtils.GetPosIndex(x, y, grid.width);
            return grid.nodes[n].walkable;
        }

        public static Node GetNode(Grid grid, int x, int y) {
            return grid.nodes[PFUtils.GetPosIndex(x, y, grid.width)];
        }

        public static Point2 GetPoint(Grid grid, int index) {
            return new Point2(index % grid.width, Mathf.FloorToInt((float)index / grid.width));
        }

        public static int GetIndex(Grid grid, int x, int y) {
            return y * grid.width + x;
        }

        public static int GetIndex(Grid grid, Point2 p) {
            return p.y * grid.width + p.x;
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
            int ci = GetIndex(grid, child.coord);
            int pi = grid.nodelinks[ci];
            return grid.nodes[pi];
        }
    }
}
