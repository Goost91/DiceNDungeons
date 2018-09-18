using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class DungeonFloorTileComponent : MonoBehaviour
{
    public DungeonFloorTile tile;
    public Vector3Int tilePosition;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame

    public void Update()
    {
    }

    public float GetDistanceToPlayer(bool useDiagonals = true)
    {
        var target = LevelManager.Instance.GetPlayerTileCoords();
        var dist = 9999999;

        if (!useDiagonals)
        {
            var diff = target - tilePosition;
            dist = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
        }
        else
        {
            bool arrived = false;
            var dist2 = 0;
            while (!arrived)
            {
                if (target.x - tilePosition.x != 0 && target.y - tilePosition.y != 0)
                {
                    var diff = tilePosition - target;
                    var direction = new Vector3Int((int) Mathf.Sign(diff.x), (int) Mathf.Sign(diff.y), 0);

                    target = target + direction;
                    dist2++;
                }
                else if (target.x - tilePosition.x == 0)
                {
                    dist2 += Mathf.Abs(target.y - tilePosition.y);
                    arrived = true;
                }
                else if (target.y - tilePosition.y == 0)
                {
                    dist2 += Mathf.Abs(target.x - tilePosition.x);
                    arrived = true;
                }

                if (dist2 > 1000)
                {
                    arrived = true;
                }
            }

            dist = dist2;
        }

        return dist;
    }
}