using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Pathfinding;
using UnityEngine;

public class SquareGrid
{
    // DIRS is directions
    // I added diagonals to this but noticed it can create problems--
    // like the path will go through obstacles that are diagonal from each other.
    public static readonly MoveDirection[] DIRS =
    {
        MoveDirection.East, // to right of tile
        MoveDirection.South, // below tile
        MoveDirection.West, // to left of tile
        MoveDirection.North, // above tile
        /*
    MoveDirection.NorthWest, // diagonal top right
    MoveDirection.NorthEast, // diagonal top left
    MoveDirection.SouthEast, // diagonal bottom right
    MoveDirection.SouthWest // diagonal bottom left
  */
    };

    // The x and y here represent the grid's starting point, 0,0.
    // And width and height are how many units wide and high the grid is.
    // See how I use this in TileManager.cs to see how you can keep
    // your Unity GameObjects in sync with this abstracted grid.
    public SquareGrid(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        tiles = new TileType[this.width, this.height];
    }

    public int x, y, width, height;

    // This is a 2d array that stores each tile's type and movement cost
    // using the TileType enum defined above
    public TileType[,] tiles;

    // Check if a location is within the bounds of this grid.
    public bool InBounds(Vector3Int id)
    {
        return (x <= id.x) && (id.x < width) && (y <= id.y) && (id.y < height);
    }

    // Everything that isn't a Wall is Passable
    public bool Passable(Vector3Int id)
    {
        return (int) tiles[id.x, id.y] < Int32.MaxValue;
    }

    // If the heuristic = 2f, the movement is diagonal
    public float Cost(Vector3Int a, Vector3Int b)
    {
        if (Math.Abs(AStarSearch.Heuristic(a, b) - 2f) < 0.001f)
        {
            return (int) tiles[b.x, b.y] * Mathf.Sqrt(2f);
        }
        

        return (int) tiles[b.x, b.y] == 0 || LevelManager.Instance.activeEnemies.Any(e=>e.tilePos == new Vector3Int(b.x,b.y,0)) ? 999999 : (int)tiles[b.x,b.y];
    }

    // Check the tiles that are next to, above, below, or diagonal to
    // this tile, and return them if they're within the game bounds and passable
    public IEnumerable<Vector3Int> Neighbors(Vector3Int id)
    {
        foreach (var dir in DIRS.Select(d => MoveDirectionExtensions.ToVec3Int(d)))
        {
            var next = new Vector3Int(id.x + dir.x, id.y + dir.y, 0);
            if (InBounds(next) && Passable(next))
            {
                yield return next;
            }
        }
    }
}