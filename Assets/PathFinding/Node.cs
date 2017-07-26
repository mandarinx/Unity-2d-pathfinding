using System;

namespace Pathfinding {

    public class Node : IHeapItem<Node> {

        private int type;
        
        // calculated values while finding path
        // gCost = distance to start node
        public int gCost;
        // hCost (heuristic cost) = distance to end node
        public int hCost;
        public int fCost;

        public readonly int x;
        public readonly int y;

        public int heapIndex;

        public Node(int _gridX, int _gridY, int _type) {
            type = _type;
            x = _gridX;
            y = _gridY;
        }

        public override bool Equals(Object obj) {
            Node n = obj as Node;
            if (n == null) {
                return false;
            }
            return (x == n.x) && (y == n.y);
        }
        
        public override int GetHashCode() {
            return (x ^ y) ^ (x ^ y);
        }

        public int GetHeapIndex() {
            return heapIndex;
        }

        public void SetHeapIndex(int value) {
            heapIndex = value;
        }

        public override string ToString() {
            return "<Node> x:"+x+" y:"+y;
        }

        public static void SetType(Node node, int type) {
            node.type = type;
        }

        public static int GetType(Node node) {
            return node.type;
        }
    }

    public class NodeComparer : IHeapItemComparer<Node> {
        public int Compare(Node a, Node b) {
            int diff = b.fCost - a.fCost;
            if (diff == 0) {
                diff = b.hCost - a.hCost;
            }
            if (diff > 0) {
                return 1;
            }
            if (diff < 0) {
                return -1;
            }
            return 0;
        }

    }
}
