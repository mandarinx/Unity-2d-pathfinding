/**
* Provide simple path-finding algorithm with support in penalties.
* Heavily based on code from this tutorial: https://www.youtube.com/watch?v=mZfyt03LDH4
* This is just a Unity port of the code from the tutorial + option to set penalty + nicer API.
*
* Original Code author: Sebastian Lague.
* Modifications & API by: Ronen Ness.
* Since: 2016.
*/
using Mandarin;

namespace Pathfinding {
    
    /**
    * Main class to find the best path from A to B.
    * Use like this:
    * Grid grid = new Grid(width, height, tiles_costs);
    * List<Point> path = Pathfinding.FindPath(grid, from, to);
    */
    public class Pathfinder {
        
        // The API you should use to get path
        // grid: grid to search in.
        // startPos: starting position.
        // targetPos: ending position.
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
                
                // Postpone till after the while loop? Will that
                // make it so that the path is returned, even when
                // we couldn't reach the target position?
                if (currentNode.Equals(targetNode)) {
                    return RetracePath(grid, startNode, targetNode, ref path);
                }

                int nodeIndex = Grid.GetIndex(gridwidth, currentNode.x, currentNode.y);
                
                int numNeighbours = Grid.UpdateNeighboursCache(grid, nodeIndex);
                for (int i = 0; i < numNeighbours; ++i) {
                    Node neighbour = Grid.GetNeighbour(grid, i);
                    int ni = Grid.GetIndex(gridwidth, neighbour.x, neighbour.y);

                    if ((mask & Node.GetType(neighbour)) == 0 || 
                    Grid.ClosedSetContains(grid, ni)) {
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
