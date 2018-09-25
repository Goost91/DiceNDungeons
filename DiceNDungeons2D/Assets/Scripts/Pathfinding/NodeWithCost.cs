using System;
using UnityEngine;

namespace Pathfinding
{
    public struct NodeWithCost<T>
    {
        public T node;
        public float cost;

        public NodeWithCost(T val, float i)
        {
            node = val;
            cost = i;
        }
    }
}