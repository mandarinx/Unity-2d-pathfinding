/**
* Provide simple path-finding algorithm with support in penalties.
* Heavily based on code from this tutorial: https://www.youtube.com/watch?v=mZfyt03LDH4
* This is just a Unity port of the code from the tutorial + option to set penalty + nicer API.
*
* Original Code author: Sebastian Lague.
* Modifications & API by: Ronen Ness.
* Since: 2016.
*/
using UnityEngine;
using System.Collections.Generic;
using Mandarin;

namespace PathFind {
    
    /**
    * Main class to find the best path from A to B.
    * Use like this:
    * Grid grid = new Grid(width, height, tiles_costs);
    * List<Point> path = Pathfinding.FindPath(grid, from, to);
    */
    public class Pathfinding {
        
        // The API you should use to get path
        // grid: grid to search in.
        // startPos: starting position.
        // targetPos: ending position.
        public static int FindPath(Grid              grid, 
                                   Point2            startPos, 
                                   Point2            targetPos, 
                                   PathfindingBuffer buffer, 
                                   ref Point2[]      path) {

            Node startNode = Grid.GetNode(grid, startPos.x, startPos.y);
            Node targetNode = Grid.GetNode(grid, targetPos.x, targetPos.y);

            buffer.openSet.Add(startNode);

            while (buffer.openSet.Count > 0) {
                Node currentNode = buffer.openSet.RemoveFirst();
                buffer.closedSet.Add(currentNode.coord, true);

                if (currentNode.Equals(targetNode)) {
                    return RetracePath(grid, startNode, targetNode, ref path);
                }

                int nodeIndex = PFUtils.GetPosIndex(currentNode.coord.x, currentNode.coord.y, grid.width);
                int numNeighbours = grid.UpdateNeighboursCache(nodeIndex);
                for (int i = 0; i < numNeighbours; ++i) {
                    Node neighbour = grid.neighbours[i];

                    if (!neighbour.walkable || buffer.closedSet.ContainsKey(neighbour.coord)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode.coord, neighbour.coord);
                    bool openContainsNeighbour = buffer.openSet.Contains(neighbour);
                    if (newMovementCostToNeighbour >= neighbour.gCost && openContainsNeighbour) {
                        continue;
                    }
                    
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour.coord, targetNode.coord);
                    neighbour.fCost = neighbour.gCost + neighbour.hCost;

                    Grid.LinkNode(grid, 
                                  Grid.GetIndex(grid, neighbour.coord), 
                                  Grid.GetIndex(grid, currentNode.coord));

                    if (!openContainsNeighbour) {
                        buffer.openSet.Add(neighbour);
                    }
                }
            }

            return 0;
        }

        private static int RetracePath(Grid grid, Node startNode, Node endNode, ref Point2[] path) {
            Node currentNode = endNode;
            int len = 0;
            while (!Equals(currentNode, startNode)) {
                path[len] = currentNode.coord;
                ++len;
                currentNode = Grid.GetParentNode(grid, currentNode);
            }
            return len;
        }

        private static int GetDistance(Point2 a, Point2 b) {
            int dstX = a.x - b.x;
            dstX = dstX < 0 ? -dstX : dstX;
            int dstY = a.y - b.y;
            dstY = dstY < 0 ? -dstY : dstY;

            if (dstX > dstY) {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    public class PathfindingBuffer {

        public Heap<Node> openSet;
        public Dictionary<Point2, bool> closedSet;
        
        public PathfindingBuffer(int gridSize) {
            openSet = new Heap<Node>(gridSize, new NodeComparer());
            closedSet = new Dictionary<Point2, bool>(gridSize, new Point2Comparer());
        }

        public void Clear() {
            openSet.Clear();
            closedSet.Clear();
        }
    }
}
