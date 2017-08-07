/**
* Provide simple path-finding algorithm with support in penalties.
* Heavily based on code from this tutorial: https://www.youtube.com/watch?v=mZfyt03LDH4
* This is just a Unity port of the code from the tutorial + option to set penalty + nicer API.
*
* Heavily modified, cleaned up and optimized by Thomas Viktil.
*
* Original Code author: Sebastian Lague.
* Modifications & API by: Ronen Ness.
* Since: 2016.
*/
using Mandarin;

namespace Pathfinding {
    
    public class Pathfinder {
        
        // grid:         grid to search in
        // startPos:     starting position
        // targetPos:    ending position
        // mask:         the walkable nodes for this path
        // path:         an array for holding the path
        public static int Find(Grid              grid, 
                               Point2            startPos, 
                               Point2            targetPos, 
                               int               mask, 
                               ref PathData[]    path) {

            Node startNode = Grid.GetNode(grid, startPos.x, startPos.y);
            Node targetNode = Grid.GetNode(grid, targetPos.x, targetPos.y);
            int gridwidth = Grid.GetWidth(grid);

            Grid.ClearBuffer(grid);
            Grid.AddToOpenSet(grid, startNode);

            while (Grid.GetOpenSetCount(grid) > 0) {
                Node currentNode = Grid.RemoveFirstFromOpenSet(grid);
                int ci = Grid.GetIndex(gridwidth, currentNode.x, currentNode.y);
                Grid.AddToClosedSet(grid, ci);
                
                // Return when looking for a path that goes from and to the
                // same node
                if (currentNode.Equals(targetNode)) {
                    return RetracePath(grid, startNode, targetNode, ref path);
                }

                int nodeIndex = Grid.GetIndex(gridwidth, currentNode.x, currentNode.y);
                
                int numNeighbours = Grid.UpdateNeighboursCache(grid, nodeIndex);
                for (int i = 0; i < numNeighbours; ++i) {
                    Node neighbour = Grid.GetNeighbour(grid, i);
                    int ni = Grid.GetIndex(gridwidth, neighbour.x, neighbour.y);
                    int nt = Node.GetType(neighbour);
                    bool isWalkable = (mask & nt) == 0;
                    
                    // Continue searching when neighbour is not walkable,
                    // or already checked
                    if (!isWalkable || Grid.ClosedSetContains(grid, ni)) {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode.x, 
                                                                                     currentNode.y, 
                                                                                     neighbour.x, 
                                                                                     neighbour.y);
                    bool openContainsNeighbour = Grid.OpenSetContains(grid, neighbour);
                    if (newMovementCostToNeighbour >= neighbour.gCost && openContainsNeighbour) {
                        continue;
                    }
                    
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour.x, neighbour.y, targetNode.x, targetNode.y);
                    neighbour.fCost = neighbour.gCost + neighbour.hCost;

                    Grid.LinkNode(grid, ni, ci);

                    if (!openContainsNeighbour) {
                        Grid.AddToOpenSet(grid, neighbour);
                    }
                }
            }

            return 0;
        }

        private static int RetracePath(Grid grid, Node startNode, Node endNode, ref PathData[] path) {
            Node currentNode = endNode;
            int len = 0;
            while (!Equals(currentNode, startNode)) {
                path[len] = new PathData(currentNode.x, 
                                         currentNode.y,
                                         Node.GetType(currentNode));
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
}
