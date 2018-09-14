using System;

namespace Entities
{
    [Flags]
    public enum MoveDirection
    {
        None = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8,
        
        NorthWest = North | West,
        NorthEast = North | East,
        SouthWest = South | West,
        SouthEast = South | East
    }
} 