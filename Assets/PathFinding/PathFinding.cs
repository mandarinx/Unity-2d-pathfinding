﻿/**
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
            int gridwidth = grid.width;

            buffer.openSet.Add(startNode);

            while (buffer.openSet.Count > 0) {
                Node currentNode = buffer.openSet.RemoveFirst();
                int ci = Grid.GetIndex(gridwidth, currentNode.x, currentNode.y);
                buffer.closedSet.Add(ci);

                if (currentNode.Equals(targetNode)) {
                    return RetracePath(grid, startNode, targetNode, ref path);
                }

                int nodeIndex = Grid.GetIndex(gridwidth, currentNode.x, currentNode.y);
                int numNeighbours = grid.UpdateNeighboursCache(nodeIndex);
                for (int i = 0; i < numNeighbours; ++i) {
                    Node neighbour = grid.neighbours[i];
                    int ni = Grid.GetIndex(gridwidth, neighbour.x, neighbour.y);

                    if (!neighbour.walkable || buffer.closedSet.Contains(ni)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode.x, currentNode.y, neighbour.x, neighbour.y);
                    bool openContainsNeighbour = buffer.openSet.Contains(neighbour);
                    if (newMovementCostToNeighbour >= neighbour.gCost && openContainsNeighbour) {
                        continue;
                    }
                    
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour.x, neighbour.y, targetNode.x, targetNode.y);
                    neighbour.fCost = neighbour.gCost + neighbour.hCost;

                    Grid.LinkNode(grid, 
                                  Grid.GetIndex(gridwidth, neighbour.x, neighbour.y), 
                                  Grid.GetIndex(gridwidth, currentNode.x, currentNode.y));

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
                path[len] = new Point2(currentNode.x, currentNode.y);
                ++len;
                currentNode = Grid.GetParentNode(grid, currentNode);
            }
            return len;
        }

        private static int GetDistance(int ax, int ay, int bx, int by) {
            int dstX = ax - bx;
            dstX = dstX < 0 ? -dstX : dstX;
            int dstY = ay - by;
            dstY = dstY < 0 ? -dstY : dstY;

            if (dstX > dstY) {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    public class PathfindingBuffer {

        public Heap<Node> openSet;
        public HashSet<int> closedSet;
        
        public PathfindingBuffer(int gridSize) {
            openSet = new Heap<Node>(gridSize, new NodeComparer());
            closedSet = new HashSet<int>();

            // Preallocate hashset
            for (int i = 0; i < gridSize; ++i) {
                closedSet.Add(i);
            }
            closedSet.Clear();
        }

        public void Clear() {
            openSet.Clear();
            closedSet.Clear();
        }
    }
}
