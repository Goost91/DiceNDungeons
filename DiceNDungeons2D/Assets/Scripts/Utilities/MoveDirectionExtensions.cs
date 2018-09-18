using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public static class MoveDirectionExtensions
{
    public static Vector3Int ToVec3Int(this MoveDirection dir)
    {
        return new Vector3Int(Math.Sign(((int) dir & 12) << 28), Math.Sign((int) dir << 30), 0);
    }

    public static Vector2Int ToVec2Int(this MoveDirection dir)
    {
        return new Vector2Int(Math.Sign(((int) dir & 12) << 28), Math.Sign((int) dir << 30));
    }

    public static MoveDirection ToMoveDirection(this Vector3Int v)
    {
        var extractNormalisedComponent = new Func<int, int>(input => input < 0 ? 2 : Math.Sign(input));

        var x = extractNormalisedComponent(v.x);
        var y = extractNormalisedComponent(v.y);

        return (MoveDirection) (x << 2 | y);
    }
}