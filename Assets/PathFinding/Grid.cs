using UnityEngine;

namespace PathFind {

    public class Grid {
        
        private Node[] nodes;
        private int[] nodelinks;
        public Node[] neighbours;
        
        public int width;
        public int height;
        
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
            neighbours = new Node[NUM_NEIGHBOURS];
        }

        public int UpdateNeighboursCache(int nodeIndex) {
            int n = 0;
            // Check coordinate to the south of nodeIndex
            int ni = nodeIndex - width;
            // Make sure the south coordinate is not outside the map
            if (ni >= 0) {
                neighbours[n++] = nodes[ni];
            }
            
            // West
            ni = nodeIndex - 1;
            // Don't bother checking for west if nodeIndex
            // is positioned along the left edge of the map
            if (nodeIndex % width > 0) {
                neighbours[n++] = nodes[ni];
            }
            
            // East
            ni = nodeIndex + 1;
            // Check for east coordinate as long as nodeIndex
            // is on a coordinate at least one column to the 
            // left of the right edge of the map
            if (nodeIndex % width < width - 1) {
                neighbours[n++] = nodes[ni];
            }
            
            // North
            ni = nodeIndex + width;
            // Make sure north coordinate is not outside the map
            if (ni < size) {
                neighbours[n++] = nodes[ni];
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
