using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {

    public class Grid {
        
        private readonly Node[]             neighbours;
        private readonly int                width;
        private readonly int                height;
        private readonly int                size;
        private readonly Heap<Node>         openSet;
        private readonly HashSet<int>       closedSet;
        
        private readonly Node[]             nodes;
        private readonly int[]              nodelinks;
        
        private const int                   NUM_NEIGHBOURS = 4;

        public Grid(int width, int height, int[] walkable_tiles) {
            this.width = width;
            this.height = height;
            size = width * height;
            
            nodes = new Node[size];
            nodelinks = new int[size];
            neighbours = new Node[NUM_NEIGHBOURS];

            openSet = new Heap<Node>(size, new NodeComparer());
            closedSet = new HashSet<int>();

            // Preallocate hashset
            for (int i = 0; i < size; ++i) {
                closedSet.Add(i);
            }
            closedSet.Clear();

            for (int i = 0; i < nodes.Length; ++i) {
                int x = -1;
                int y = -1;

                GetPoint(width, i, out x, out y);
                nodes[i] = new Node(x, y, walkable_tiles[i]);
            }
        }

        public static void ClearBuffer(Grid grid) {
            grid.openSet.Clear();
            grid.closedSet.Clear();
        }

        public static int UpdateNeighboursCache(Grid grid, int nodeIndex) {
            int n = 0;
            // Check coordinate to the south of nodeIndex
            int ni = nodeIndex - grid.width;
            // Make sure the south coordinate is not outside the map
            if (ni >= 0) {
                grid.neighbours[n++] = grid.nodes[ni];
            }
            
            // West
            ni = nodeIndex - 1;
            // Don't bother checking for west if nodeIndex
            // is positioned along the left edge of the map
            if (nodeIndex % grid.width > 0) {
                grid.neighbours[n++] = grid.nodes[ni];
            }
            
            // East
            ni = nodeIndex + 1;
            // Check for east coordinate as long as nodeIndex
            // is on a coordinate at least one column to the 
            // left of the right edge of the map
            if (nodeIndex % grid.width < grid.width - 1) {
                grid.neighbours[n++] = grid.nodes[ni];
            }
            
            // North
            ni = nodeIndex + grid.width;
            // Make sure north coordinate is not outside the map
            if (ni < grid.size) {
                grid.neighbours[n++] = grid.nodes[ni];
            }
            return n;
        }

        public static int GetSize(Grid grid) {
            return grid.size;
        }

        public static int GetWidth(Grid grid) {
            return grid.width;
        }

        public static int GetHeight(Grid grid) {
            return grid.height;
        }

        public static void LinkNode(Grid grid, int child, int parent) {
            grid.nodelinks[child] = parent;
        }

        public static Node GetParentNode(Grid grid, Node child) {
            int ci = GetIndex(grid.width, child.x, child.y);
            int pi = grid.nodelinks[ci];
            return grid.nodes[pi];
        }

        public static Node GetNode(Grid grid, int x, int y) {
            return grid.nodes[GetIndex(grid.width, x, y)];
        }

        public static void GetPoint(int gridwidth, int index, out int x, out int y) {
            x = index % gridwidth; 
            y = Mathf.FloorToInt((float)index / gridwidth);
        }

        public static int GetIndex(int gridwidth, int x, int y) {
            return y * gridwidth + x;
        }

        public static void AddToOpenSet(Grid grid, Node node) {
            grid.openSet.Add(node);
        }

        public static void AddToClosedSet(Grid grid, int val) {
            grid.closedSet.Add(val);
        }

        public static int GetOpenSetCount(Grid grid) {
            return grid.openSet.Count;
        }

        public static bool ClosedSetContains(Grid grid, int n) {
            return grid.closedSet.Contains(n);
        }

        public static bool OpenSetContains(Grid grid, Node node) {
            return grid.openSet.Contains(node);
        }

        public static Node RemoveFirstFromOpenSet(Grid grid) {
            return grid.openSet.RemoveFirst();
        }

        public static Node GetNeighbour(Grid grid, int n) {
            return grid.neighbours[n];
        }
    }
}
