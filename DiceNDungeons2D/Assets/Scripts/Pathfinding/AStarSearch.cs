using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Entities;

// This script is adapted from these,
// but has been heavily modified in some areas:
// http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
// https://gist.github.com/DanBrooker/1f8855367ae4add40792

// I'm continuing to optimize and change things here. I would not use this
// in production as it is.

// Note that Floor, Forest and Wall are somewhat arbitrary,
// but also represent three differnt types of tiles, which are
// all handled differently by A*. Floors are Passable, walls are not,
// and forests are passable but with a higher movement cost.
public enum TileType {
  Floor = 1,
  Forest = 5,
  Wall = System.Int32.MaxValue
}

public class SquareGrid {
  // DIRS is directions
  // I added diagonals to this but noticed it can create problems--
  // like the path will go through obstacles that are diagonal from each other.
  public static readonly MoveDirection[] DIRS = {
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
  public SquareGrid(int x, int y, int width, int height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    tiles = new TileType[this.width,this.height];
  }

  public int x, y, width, height;

  // This is a 2d array that stores each tile's type and movement cost
  // using the TileType enum defined above
  public TileType[,] tiles;

  // Check if a location is within the bounds of this grid.
  public bool InBounds(Vector3Int id) {
    return (x <= id.x) && (id.x < width) && (y <= id.y) && (id.y < height);
  }

  // Everything that isn't a Wall is Passable
  public bool Passable(Vector3Int id) {
    return (int)tiles[id.x,id.y] < Int32.MaxValue;
  }

  // If the heuristic = 2f, the movement is diagonal
  public float Cost(Vector3Int a, Vector3Int b) {
    if (Math.Abs(AStarSearch.Heuristic(a,b) - 2f) < 0.001f) {
      return (int)tiles[b.x,b.y] * Mathf.Sqrt(2f);
    }
    return (int)tiles[b.x,b.y];
  }

  // Check the tiles that are next to, above, below, or diagonal to
  // this tile, and return them if they're within the game bounds and passable
  public IEnumerable<Vector3Int> Neighbors(Vector3Int id) {
    foreach (var dir in DIRS.Select(d => d.ToVec3Int())) {
      var next = new Vector3Int(id.x + dir.x, id.y + dir.y,0);
      if (InBounds(next) && Passable(next)) {
        yield return next;
      }
    }
  }
}

public class PriorityQueue<T> {
  // From Red Blob: I'm using an unsorted array for this example, but ideally this
  // would be a binary heap. Find a binary heap class:
  // * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
  // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
  // * http://xfleury.github.io/graphsearch.html
  // * http://stackoverflow.com/questions/102398/priority-queue-in-net

  private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

  public int Count => elements.Count;

  public void Enqueue(T item, float priority) {
    elements.Add(new KeyValuePair<T, float>(item,priority));
  }

  // Returns the Vector3Int that has the lowest priority
  public T Dequeue() {
    int bestIndex = 0;

    for (int i = 0; i < elements.Count; i++) {
      if (elements[i].Value < elements[bestIndex].Value) {
        bestIndex = i;
      }
    }

    T bestItem = elements[bestIndex].Key;
    elements.RemoveAt(bestIndex);
    return bestItem;
  }
}

// Now that all of our classes are in place, we get get
// down to the business of actually finding a path.
public class AStarSearch {
  // Someone suggested making this a 2d field.
  // That will be worth looking at if you run into performance issues.
  public Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
  public Dictionary<Vector3Int, float> costSoFar = new Dictionary<Vector3Int, float>();

  private Vector3Int start;
  private Vector3Int goal;

  public static float Heuristic(Vector3Int a, Vector3Int b) {
    return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
  }

  // Conduct the A* search
  public AStarSearch(SquareGrid graph, Vector3Int start, Vector3Int goal) {
    // start is current sprite Vector3Int
    this.start = start;
    // goal is sprite destination eg tile user clicked on
    this.goal = goal;

    // add the cross product of the start to goal and tile to goal vectors
    // Vector3 startToGoalV = Vector3.Cross(start.vector3,goal.vector3);
    // Vector3Int startToGoal = new Vector3Int(startToGoalV);
    // Vector3 neighborToGoalV = Vector3.Cross(neighbor.vector3,goal.vector3);

    // frontier is a List of key-value pairs:
    // Vector3Int, (float) priority
    var frontier = new PriorityQueue<Vector3Int>();
    // Add the starting location to the frontier with a priority of 0
    frontier.Enqueue(start, 0f);

    cameFrom.Add(start, start); // is set to start, None in example
    costSoFar.Add(start, 0f);

    while (frontier.Count > 0f) {
      // Get the Vector3Int from the frontier that has the lowest
      // priority, then remove that Vector3Int from the frontier
      Vector3Int current = frontier.Dequeue();

      // If we're at the goal Vector3Int, stop looking.
      if (current.Equals(goal)) break;

      // Neighbors will return a List of valid tile Vector3Ints
      // that are next to, diagonal to, above or below current
      foreach (var neighbor in graph.Neighbors(current)) {

        // If neighbor is diagonal to current, graph.Cost(current,neighbor)
        // will return Sqrt(2). Otherwise it will return only the cost of
        // the neighbor, which depends on its type, as set in the TileType enum.
        // So if this is a normal floor tile (1) and it's neighbor is an
        // adjacent (not diagonal) floor tile (1), newCost will be 2,
        // or if the neighbor is diagonal, 1+Sqrt(2). And that will be the
        // value assigned to costSoFar[neighbor] below.
        float newCost = costSoFar[current] + graph.Cost(current, neighbor);

        // If there's no cost assigned to the neighbor yet, or if the new
        // cost is lower than the assigned one, add newCost for this neighbor
        if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) {

          // If we're replacing the previous cost, remove it
          if (costSoFar.ContainsKey(neighbor)) {
            costSoFar.Remove(neighbor);
            cameFrom.Remove(neighbor);
          }

          costSoFar.Add(neighbor, newCost);
          cameFrom.Add(neighbor, current);
          float priority = newCost + Heuristic(neighbor, goal);
          frontier.Enqueue(neighbor, priority);
        }
      }
    }

  }

  // Return a List of Vector3Ints representing the found path
  public List<Vector3Int> FindPath() {

    List<Vector3Int> path = new List<Vector3Int>();
    Vector3Int current = goal;
    // path.Add(current);

    while (!current.Equals(start)) {
      if (!cameFrom.ContainsKey(current)) {
        MonoBehaviour.print("cameFrom does not contain current.");
        return new List<Vector3Int>();
			}
      path.Add(current);
      current = cameFrom[current];
    }
    // path.Add(start);
    path.Reverse();
    return path;
  }
}