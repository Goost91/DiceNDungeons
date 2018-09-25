using UnityEngine;

namespace Utilities
{
    public static class Vector3IntExtensions
    {
        public static float GetDistanceTo(this Vector3Int v, Vector3Int o, bool useDiagonals = true)
        {
            var dist = 9999999;

            if (!useDiagonals)
            {
                var diff = o - v;
                dist = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            }
            else
            {
                bool arrived = false;
                var dist2 = 0;
                while (!arrived)
                {
                    if (o.x - v.x != 0 && o.y - v.y != 0)
                    {
                        var diff = v - o;
                        var direction = new Vector3Int((int) Mathf.Sign(diff.x), (int) Mathf.Sign(diff.y), 0);

                        o = o + direction;
                        dist2++;
                    }
                    else if (o.x - v.x == 0)
                    {
                        dist2 += Mathf.Abs(o.y - v.y);
                        arrived = true;
                    }
                    else if (o.y - v.y == 0)
                    {
                        dist2 += Mathf.Abs(o.x - v.x);
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
}