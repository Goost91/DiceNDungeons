using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
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
        public bool isActive;
        
        public Vector3Int tilePos;
        private bool isDead;

        public void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            positionFix = new Vector3(spriteRenderer.bounds.extents.x, 0);
            transform.position += positionFix;
            SetZPosition();
        }
        
        
        public void ModifyHp(int dieValue)
        {
            hp -= dieValue;
            if (hp < 0)
            {
                Die();
            }
        }

        public void Die()
        {
            if (this is PlayerScript) return;
            isDead = true;
        }

        public virtual void Update()
        {
            
            tilePos = LevelManager.Instance.floorMap.WorldToCell(transform.position);

            if (isDead) {LevelManager.Instance.activeEnemies.Remove(this);
                //LevelManager.Instance.activeEnemies.Remove(this);
                //DestroyImmediate(this.gameObject);
                transform.gameObject.SetActive(false);
                Destroy(this.gameObject);}
        }

        public bool Move(MoveDirection dir)
        {
            var target = dir.ToVec3Int();

            target += LevelManager.Instance.floorMap.WorldToCell(transform.position);

            if (LevelManager.Instance.floorMap.HasTile(target))
            {
                transform.position = LevelManager.Instance.floorMap.CellToWorld(target) + positionFix + Vector3.back;
                tilePos = GetCurrentPosition();
                return true;
            }

            return false;
        }

        public void SetZPosition()
        {
            transform.position.Set(transform.position.x, transform.position.y, -1);
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

        public List<NodeWithCost<Vector3Int>> FindPath(Vector3Int targetTile)
        {
            return FindPathFrom(GetCurrentPosition(), targetTile);
        }

        public List<NodeWithCost<Vector3Int>> FindPathFrom(Vector3Int fromTile, Vector3Int targetTile)
        {
            AStarSearch search = new AStarSearch(LevelManager.Instance.grid,
                fromTile, targetTile);
            return search.FindPath();
        }

        public void TryMove(List<NodeWithCost<Vector3Int>> path)
        {
            if (path.Count > 0)
            {
                if (moves > 0)
                {
                    path.Reverse();
                    path.Add(new NodeWithCost<Vector3Int>(GetCurrentPosition(), 0));
                    path.Reverse();
                    StartCoroutine(MoveToTarget(path));
                }
            }
        }

        public IEnumerator MoveToTarget(List<NodeWithCost<Vector3Int>> path)
        {
            if (moves <= 0) yield break;
            done = false;
            while (moves > 0 && path.Count > 0)
            {
                NodeWithCost<Vector3Int> from;
                NodeWithCost<Vector3Int> to;

                if (path.Count <= 1)
                {
                    from =  new NodeWithCost<Vector3Int>(GetCurrentPosition(),0);
                    to = path[0];
                }
                else
                {
                    from = path[0];
                    to = path[1];
                }
                
                if(to.cost > 10) yield break;

                Move((to.node - from.node).ToMoveDirection());
                path.RemoveAt(0);
                
                moves--;
                
                if (moves <= 0)
                {
                 
                    yield break;
                }

                if(this is PlayerScript) GameManager.Instance.UpdateUI();
                yield return new WaitForSeconds(0.3f);
            }
        }
        
        
        public void SetGridPosition(Vector3Int pos)
        {
            transform.position = LevelManager.Instance.floorMap.CellToWorld(pos) + positionFix;
        }

        public void OnGUI()
        {
            if (this is PlayerScript) return;
            Vector3 pos = GetCurrentPosition();
            GUI.Label(new Rect(pos, new Vector2(30,30)), $"{hp}");
        }
    }
}