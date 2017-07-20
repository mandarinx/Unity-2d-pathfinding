﻿using System;
using Mandarin;

namespace PathFind {

    public class Node : IHeapItem<Node> {

        public bool walkable;
        
        // calculated values while finding path
        // gCost = distance to start node
        public int gCost;
        // hCost (heuristic cost) = distance to end node
        public int hCost;
        public int fCost;

        public readonly Point2 coord;

        public int heapIndex;

        public Node(bool isWalkable, int _gridX, int _gridY) {
            walkable = isWalkable;
            coord = new Point2(_gridX, _gridY);
        }

        public override bool Equals(Object obj) {
            Node n = obj as Node;
            if (n == null) {
                return false;
            }

            return (coord.x == n.coord.x) && (coord.y == n.coord.y);
        }
        
        public override int GetHashCode() {
            return coord.GetHashCode() ^ coord.GetHashCode();
        }

        public int GetHeapIndex() {
            return heapIndex;
        }

        public void SetHeapIndex(int value) {
            heapIndex = value;
        }

        public override string ToString() {
            return "<Node> x:"+coord.x+" y:"+coord.y+" walkable:"+(walkable ? "yes" : "no");
        }
    }

    public class NodeComparer : IHeapItemComparer<Node> {
        public int Compare(Node a, Node b) {
            int diff = b.fCost - a.fCost;
            if (diff == 0) {
                diff = b.hCost - a.hCost;
            }
            if (diff > 0) {
//            return coord == n.coord;
                return 1;
            }
            if (diff < 0) {
                return -1;
            }
            return 0;
        }

    }
}
