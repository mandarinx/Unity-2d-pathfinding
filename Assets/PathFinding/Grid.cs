using Mandarin;

namespace PathFind {

    public class Grid {
        
        private Node[] nodes;
        public Node[] neighbours;
        
        public int width;
        public int height;
        private int[] neighbourDirs;
        
        private const int NUM_NEIGHBOURS = 4;

        public Grid(int width, int height, bool[] walkable_tiles) {
            Init(width, height);

            for (int i = 0; i < nodes.Length; ++i) {
                Point2 p = PFUtils.GetPos(i, width);
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

        private void Init(int gridWidth, int gridHeight) {
            width = gridWidth;
            height = gridHeight;
            nodes = new Node[width * height];
            // Use Von Neumann neighbourhood.
            // Sequence: N > E > S > W
            neighbourDirs = new [] {width, 1, -width, -1};
            neighbours = new Node[NUM_NEIGHBOURS];
        }

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
    }
}
