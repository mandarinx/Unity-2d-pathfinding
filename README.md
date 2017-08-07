# Notes about this fork

This is a highly specialized and optimized version of the pathfinder. It's meant to be used in a specific game.

- Nodes have a type (int) field for masking off areas of the grid. Use the mask argument in Pathfinder.Find() to tell the pathfinder which areas of the grid should be considered walkable for the path you're looking for.
- Movement is allowed only along the cardinal axis.
- The API has been changed a bit to get rid of memory allocations.
- It's meant to be fast on small grids. It's not tested on large grids.

On the first run, the pathfinder will spend a couple of milliseconds getting the path. But on succeeding runs, it will find the path in about 0.05 milliseconds. These numbers were reported by the profiler in Unity 2017.1.0f3. The tested were run in editor in an iOS project. The grid I used for testing was 7 x 27 (189) tiles.

On an iPhone 6, with the same grid, app built for release, the first path found is returned in about 0.2 - 0.3 ms. Most paths are found within 0.05 - 0.3 ms.

The pathfinder alloces 0 bytes at runtime.

# Unity-2d-pathfinding
A very simple 2d tile-based pathfinding for unity, with tiles price supported.

## About

This code is mostly based on the code from [this tutorial](https://www.youtube.com/watch?v=mZfyt03LDH4), with the following modifications:

- Removed all rendering and debug components.
- Converted it into a script-only solution, that relay on grid input via code.
- Separated into files, some docs.
- A more simple straight-forward API.
- Added support in tiles price, eg tiles that cost more to walk on.

But overall most of the credit belongs to [Sebastian Lague](https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ), so show him your love.

## How to use

First, copy the folder 'PathFinding' to anywhere you want your asset scripts folder. Once you have it use pathfinding like this:

```C#
// create the tiles map
float[,] tilesmap = new float[width, height];
// set values here....
// every float in the array represent the cost of passing the tile at that position.
// use 0.0f for blocking tiles.

// create a grid
PathFind.Grid grid = new PathFind.Grid(width, height, tilesmap);

// create source and target points
PathFind.Point _from = new PathFind.Point(1, 1);
PathFind.Point _to = new PathFind.Point(10, 10);

// get path
// path will either be a list of Points (x, y), or an empty list if no path is found.
List<PathFind.Point> path = PathFind.Pathfinding.FindPath(grid, _from, _to);
```

If you don't care about price of tiles (eg tiles can only be walkable or blocking), you can also pass a 2d array of *booleans* when creating the grid:
```C#
// create the tiles map
bool[,] tilesmap = new bool[width, height];
// set values here....
// true = walkable, false = blocking

// create a grid
PathFind.Grid grid = new PathFind.Grid(width, height, tilesmap);

// rest is the same..
```
