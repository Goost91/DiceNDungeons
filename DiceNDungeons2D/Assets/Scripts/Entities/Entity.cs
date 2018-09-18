using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        public int hp;
        public int maxHp;

        public int moves;
        public int maxMoves;

        public Vector3 positionFix;
        public SpriteRenderer spriteRenderer;
        public bool done;

        public void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            positionFix = new Vector3(spriteRenderer.bounds.extents.x, 0);
        }

        public bool Move(MoveDirection dir)
        {
            var target = dir.ToVec3Int();

            target += LevelManager.Instance.floorMap.WorldToCell(transform.position);

            if (LevelManager.Instance.floorMap.HasTile(target))
            {
                transform.position = LevelManager.Instance.floorMap.CellToWorld(target) + positionFix;
                return true;
            }

            return false;
        }

        public Vector3Int GetCurrentPosition()
        {
            return
                LevelManager.Instance.floorMap.WorldToCell(transform.position);
        }

        public TileBase GetCurrentTile()
        {
            return LevelManager.Instance.floorMap.GetTile(
                LevelManager.Instance.floorMap.WorldToCell(transform.position));
        }

        public virtual void DoTurn()
        {
        }

        public List<Vector3Int> FindPath(Vector3Int targetTile)
        {
            return FindPathFrom(GetCurrentPosition(), targetTile);
        }

        public List<Vector3Int> FindPathFrom(Vector3Int fromTile, Vector3Int targetTile)
        {
            AStarSearch search = new AStarSearch(LevelManager.Instance.grid,
                GetCurrentPosition(), targetTile);
            return search.FindPath();
        }

        protected void TryMove(List<Vector3Int> path)
        {
            if (path.Count > 0)
            {
                if (moves > 0)
                {
                    path.Reverse();
                    path.Add(GetCurrentPosition());
                    path.Reverse();
                    StartCoroutine((IEnumerator) MoveToTarget(path));
                }
            }
        }

        public IEnumerator MoveToTarget(List<Vector3Int> path)
        {
            if (moves <= 0) yield break;
            done = false;
            while (moves > 0 && path.Count > 0)
            {
                var from = Vector3Int.zero;
                var to = Vector3Int.zero;

                if (path.Count <= 1)
                {
                    from = GetCurrentPosition();
                    to = path[0];
                }
                else
                {
                    from = path[0];
                    to = path[1];
                }

                Move((to - from).ToMoveDirection());
                path.RemoveAt(0);
                moves--;
                if (moves <= 0)
                {
                    done = true;
                    moves = maxMoves;
                    yield break;
                }

                GameManager.Instance.UpdateUI();
                yield return new WaitForSeconds(0.3f);
            }
        }

        public void OnGUI()
        {
            if (this is PlayerScript) return;
            Vector3 pos = GetCurrentPosition();
            GUI.Label(new Rect(pos, new Vector2(30,30)), $"{hp}");
        }
    }
}