using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Pathfinding
{
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
    public enum TileType
    {
        Floor = 1,
        Forest = 5,
        Wall = System.Int32.MaxValue
    }

// Now that all of our classes are in place, we get get
// down to the business of actually finding a path.
    public class AStarSearch
    {
        // Someone suggested making this a 2d field.
        // That will be worth looking at if you run into performance issues.
        public Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        public Dictionary<Vector3Int, float> costSoFar = new Dictionary<Vector3Int, float>();

        private Vector3Int start;
        private Vector3Int goal;

        public static float Heuristic(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        // Conduct the A* search
        public AStarSearch(SquareGrid graph, Vector3Int start, Vector3Int goal)
        {
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

            while (frontier.Count > 0f)
            {
                // Get the Vector3Int from the frontier that has the lowest
                // priority, then remove that Vector3Int from the frontier
                Vector3Int current = frontier.Dequeue();

                // If we're at the goal Vector3Int, stop looking.
                if (current.Equals(goal)) break;

                // Neighbors will return a List of valid tile Vector3Ints
                // that are next to, diagonal to, above or below current
                foreach (var neighbor in graph.Neighbors(current))
                {
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
                    if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                    {
                        // If we're replacing the previous cost, remove it
                        if (costSoFar.ContainsKey(neighbor))
                        {
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
        public List<NodeWithCost<Vector3Int>> FindPath()
        {
            var path = new List<NodeWithCost<Vector3Int>>();
            Vector3Int current = goal;
            // path.Add(current);

            while (!current.Equals(start))
            {
                if (!cameFrom.ContainsKey(current))
                {
                    MonoBehaviour.print("cameFrom does not contain current.");
                    return new List<NodeWithCost<Vector3Int>>();
                }

                path.Add(new NodeWithCost<Vector3Int>(current, costSoFar[current]));
                current = cameFrom[current];
            }

            // path.Add(start);
            path.Reverse();
            return path;
        }
    }
}